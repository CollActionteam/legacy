using CollAction.Data;
using CollAction.Models;
using CollAction.Services.Email;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Services.Donation
{
    /// <summary>
    /// There are 4 donation flows:
    /// * Non-recurring iDeal payments
    ///   - The flow is started at DebitDetails.tsx where a source with/the relevant details is created client-side
    ///   - This source is sent to /Donation/InitializeIdealCheckout, where this source is attached to a customer-id (this can't be done with the public stripe API keys)
    ///   - After that, the DebitDetails component will redirect the user to the source redirect-url, where the user will get his bank iDeal dialog
    ///   - On success, the user will be redirected to the return page, which will redirect the user to the thank-you page if successfull, otherwise the user will be redirected to the donation-page
    ///   - Stripe will POST to the chargeable webhook (set to /Donation/Chargeable if the stripe settings are correct). This will finish the iDeal payment. If these settings aren't correct, we won't receive the payment.
    /// * Non-recurring credit card payments
    ///   - All the details are gathered in DonationBox.tsx, and are sent to /Donation/InitializeCreditCardCheckout
    ///   - Here we'll initiate a "Checkout" session for the credit card payment
    ///   - From /Donation/InitializeCreditCardCheckout, we return the checkout-id. In DonationBox.tsx we use stripe.js to redirect user to the checkout page
    ///   - If successfull, the user will be returned to the thank-you page, otherwise the user will be redirected to the donation-page
    ///   - Checkout will auto-charge, so the webhook won't be necessary
    /// * Recurring SEPA Direct payments
    ///   - The flow is started at DebitDetails.tsx where a source with the relevant details is created client-side
    ///   - This source is sent to /Donation/InitializeSepaDirect, where this source is attached to a auto-charged recurring subscription
    ///   - On success, the user will be redirected to the thank-you page, otherwise the user will be shown an error
    ///   - Checkout/Billing will auto-charge the subscription, so the webhook won't be necessary
    /// * Recurring credit card payments
    ///   - All the details are gathered in DonationBox.tsx, and are sent to /Donation/InitializeCreditCardCheckout
    ///   - Here we'll initiate a "Checkout" session with a monthly subscription + plan for the credit card payment
    ///   - From /Donation/InitializeCreditCardCheckout, we return the checkout-id. In DonationBox.tsx we use stripe.js to redirect user to the checkout page
    ///   - If successfull, the user will be returned to the thank-you page, otherwise the user will be redirected to the donation-page
    ///   - Checkout/Billing will auto-charge the subscription, so the webhook won't be necessary
    /// </summary>
    public class DonationService : IDonationService
    {
        private const string StatusChargeable = "chargeable";
        private const string StatusConsumed = "consumed";
        private const string EventTypeChargeableSource = "source.chargeable";
        private const string EventTypeChargeSucceeded = "charge.succeeded";
        private const string NameKey = "name";
        private const string RecurringDonationProduct = "Recurring Donation Stichting CollAction";

        private readonly CustomerService customerService;
        private readonly SourceService sourceService;
        private readonly ChargeService chargeService;
        private readonly SessionService sessionService;
        private readonly SubscriptionService subscriptionService;
        private readonly PlanService planService;
        private readonly ProductService productService;

        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly IEmailSender emailSender;
        private readonly ILogger<DonationService> logger;
        private readonly StripeSignatures stripeSignatures;
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RequestOptions requestOptions;
        private readonly SiteOptions siteOptions;

        public DonationService(
            IOptions<RequestOptions> requestOptions,
            IOptions<SiteOptions> siteOptions, 
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context, 
            IBackgroundJobClient backgroundJobClient,
            IOptions<StripeSignatures> stripeSignatures,
            IEmailSender emailSender,
            ILogger<DonationService> logger)
        {
            this.requestOptions = requestOptions.Value;
            this.siteOptions = siteOptions.Value;
            this.backgroundJobClient = backgroundJobClient;
            this.stripeSignatures = stripeSignatures.Value;
            this.context = context;
            this.userManager = userManager;
            this.emailSender = emailSender;
            this.logger = logger;
            customerService = new CustomerService(this.requestOptions.ApiKey);
            sourceService = new SourceService(this.requestOptions.ApiKey);
            chargeService = new ChargeService(this.requestOptions.ApiKey);
            sessionService = new SessionService(this.requestOptions.ApiKey);
            subscriptionService = new SubscriptionService(this.requestOptions.ApiKey);
            planService = new PlanService(this.requestOptions.ApiKey);
            productService = new ProductService(this.requestOptions.ApiKey);
        }

        public async Task<bool> HasIDealPaymentSucceeded(string sourceId, string clientSecret, CancellationToken cancellationToken)
        {
            Source source = await sourceService.GetAsync(sourceId, cancellationToken: cancellationToken);
            return (source.Status == StatusChargeable || source.Status == StatusConsumed) && 
                   clientSecret == source.ClientSecret;
        }

        /*
         * Here we're initializing a stripe checkout session for paying with a credit card. The upcoming SCA regulations ensure we have to do it through this API, because it's the only one that /easily/ supports things like 3D secure.
         */
        public async Task<string> InitializeCreditCardCheckout(string currency, int amount, string name, string email, bool recurring, CancellationToken cancellationToken)
        {
            ValidateDetails(amount, name, email);

            ApplicationUser user = await userManager.FindByEmailAsync(email);

            var sessionOptions = new SessionCreateOptions()
            {
                SuccessUrl = $"{siteOptions.PublicAddress}/Donation/ThankYou",
                CancelUrl = $"{siteOptions.PublicAddress}/Donation/Donate",
                PaymentMethodTypes = new List<string>
                {
                    "card",
                }
            };

            if (recurring)
            {
                logger.LogInformation("Initializing recurring credit card checkout session");
                sessionOptions.CustomerEmail = email; // TODO: sessionOptions.CustomerId = customer.Id; // Once supported
                sessionOptions.SubscriptionData = new SessionSubscriptionDataOptions()
                {
                    Items = new List<SessionSubscriptionDataItemOptions>()
                    {
                        new SessionSubscriptionDataItemOptions()
                        {
                            PlanId = (await CreateRecurringPlan(amount, currency, cancellationToken)).Id,
                            Quantity = 1
                        }
                    }
                };
            }
            else
            {
                logger.LogInformation("Initializing credit card checkout session");
                Customer customer = await GetOrCreateCustomer(name, email, cancellationToken);
                sessionOptions.CustomerId = customer.Id;
                sessionOptions.LineItems = new List<SessionLineItemOptions>()
                {
                    new SessionLineItemOptions()
                    {
                        Amount = amount * 100,
                        Currency = currency,
                        Name = "donation",
                        Description = "A donation to Stichting CollAction",
                        Quantity = 1
                    }
                };
            }

            Session session = await sessionService.CreateAsync(sessionOptions, cancellationToken: cancellationToken);

            context.DonationEventLog.Add(new DonationEventLog()
            {
                UserId = user?.Id,
                Type = DonationEventType.Internal,
                EventData = session.ToJson()
            });
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Done initializing credit card checkout session");

            return session.Id;
        }

        /*
         * Here we're initializing a stripe SEPA subscription on a source with sepa/iban data. This subscription should auto-charge.
         */
        public async Task InitializeSepaDirect(string sourceId, string name, string email, int amount, CancellationToken cancellationToken)
        {
            ValidateDetails(amount, name, email);

            logger.LogInformation("Initializing sepa direct");
            ApplicationUser user = await userManager.FindByEmailAsync(email);
            Customer customer = await GetOrCreateCustomer(name, email, cancellationToken);
            Source source = await sourceService.AttachAsync(
                customer.Id,
                new SourceAttachOptions()
                {
                    Source = sourceId
                },
                cancellationToken: cancellationToken);
            Plan plan = await CreateRecurringPlan(amount, "eur", cancellationToken);
            Subscription subscription = await subscriptionService.CreateAsync(
                new SubscriptionCreateOptions()
                {
                    DefaultSource = source.Id,
                    Billing = Billing.ChargeAutomatically,
                    CustomerId = customer.Id,
                    Items = new List<SubscriptionItemOption>()
                    {
                        new SubscriptionItemOption()
                        {
                            PlanId = plan.Id,
                            Quantity = 1
                        }
                    }
                },
                cancellationToken: cancellationToken);

            context.DonationEventLog.Add(new DonationEventLog()
            {
                UserId = user?.Id,
                Type = DonationEventType.Internal,
                EventData = subscription.ToJson()
            });
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Done initializing sepa direct");
        }

        /*
         * Here we're initializing the part of the iDeal payment that has to happen on the backend. For now, that's only attaching an actual customer record to the payment source.
         * In the future to handle SCA, we might need to start using payment intents or checkout here. SCA starts from september the 14th. The support for iDeal is not there yet though, so we'll have to wait.
         */
        public async Task InitializeIdealCheckout(string sourceId, string name, string email, CancellationToken cancellationToken)
        {
            logger.LogInformation("Initializing iDeal");
            ValidateDetails(name, email);

            ApplicationUser user = await userManager.FindByEmailAsync(email);
            Customer customer = await GetOrCreateCustomer(name, email, cancellationToken);
            Source source = await sourceService.AttachAsync(
                customer.Id, 
                new SourceAttachOptions()
                {
                    Source = sourceId
                },
                cancellationToken: cancellationToken);

            context.DonationEventLog.Add(
                new DonationEventLog()
                {
                    UserId = user?.Id,
                    Type = DonationEventType.Internal,
                    EventData = source.ToJson()
                });
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Done initializing iDeal");
        }

        /*
         * We're receiving an event from the stripe webhook, an payment source can be charge. We're queueing it up so we can retry it as much as possible.
         * In the future to handle SCA, we might need to start using payment intents or checkout here. SCA starts from september the 14th. The support for iDeal is not there yet though, so we'll have to wait.
         */
        public void HandleChargeable(string json, string signature)
        {
            logger.LogInformation("Received chargeable");
            Event stripeEvent = EventUtility.ConstructEvent(json, signature, stripeSignatures.StripeChargeableWebhookSecret);
            if (stripeEvent.Type == EventTypeChargeableSource)
            {
                Source source = (Source)stripeEvent.Data.Object;
                backgroundJobClient.Enqueue(() => Charge(source.Id));
            }
            else
            {
                throw new InvalidOperationException($"invalid event sent to source.chargeable webhook: {stripeEvent.ToJson()}");
            }
        }

        /*
         * We're logging all stripe events here. For audit purposes, and maybe the dwh team can make something pretty out of this data.
         */
        public async Task LogPaymentEvent(string json, string signature, CancellationToken cancellationToken)
        {
            logger.LogInformation("Received payment event");
            Event stripeEvent = EventUtility.ConstructEvent(json, signature, stripeSignatures.StripePaymentEventWebhookSecret);

            context.DonationEventLog.Add(new DonationEventLog()
            {
                Type = DonationEventType.External,
                EventData = stripeEvent.ToJson()
            });
            await context.SaveChangesAsync(cancellationToken);

            if (stripeEvent.Type == EventTypeChargeSucceeded)
            {
                Charge charge = (Charge)stripeEvent.Data.Object;
                var subscriptions = await subscriptionService.ListAsync(
                    new SubscriptionListOptions()
                    {
                        CustomerId = charge.CustomerId
                    },
                    cancellationToken: cancellationToken);
                Customer customer = await customerService.GetAsync(charge.CustomerId, cancellationToken: cancellationToken);
                await SendDonationThankYou(customer, subscriptions.Any());
            }
        }

        /*
         * Charge a source here (only used for iDeal right now). It's a background job so it can be restarted.
         */
        public async Task Charge(string sourceId)
        {
            logger.LogInformation("Processing chargeable");
            Source source = await sourceService.GetAsync(sourceId);
            if (source.Status == StatusChargeable)
            {
                Charge charge;
                try
                {
                    charge = await chargeService.CreateAsync(new ChargeCreateOptions()
                    {
                        Amount = source.Amount,
                        Currency = source.Currency,
                        SourceId = sourceId,
                        CustomerId = source.Customer,
                        Description = "A donation to Stichting CollAction"
                    });
                }
                catch (StripeException e)
                {
                    logger.LogError(e, "Error processing chargeable");
                    throw;
                }

                Customer customer = await customerService.GetAsync(source.Customer);
                ApplicationUser user = customer != null ? await userManager.FindByEmailAsync(customer.Email) : null;
                context.DonationEventLog.Add(new DonationEventLog()
                {
                    UserId = user?.Id,
                    Type = DonationEventType.Internal,
                    EventData = charge.ToJson()
                });
                await context.SaveChangesAsync();
            }
            else
            {
                logger.LogError("Invalid chargeable received");
                throw new InvalidOperationException($"source: {source.Id} is not chargeable, something went wrong in the payment flow");
            }
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptionsFor(ApplicationUser userFor, CancellationToken cancellationToken)
        {
            var customers = await customerService.ListAsync(
                new CustomerListOptions()
                {
                    Email = userFor.Email
                }, cancellationToken: cancellationToken);

            var subscriptions = await Task.WhenAll(
                customers.Select(c =>
                    subscriptionService.ListAsync(
                        new SubscriptionListOptions()
                        {
                            CustomerId = c.Id
                        },
                        cancellationToken: cancellationToken)));

            return subscriptions.SelectMany(s => s);
        }

        public async Task CancelSubscription(string subscriptionId, ClaimsPrincipal userFor, CancellationToken cancellationToken)
        {
            var user = await userManager.GetUserAsync(userFor);
            Subscription subscription = await subscriptionService.GetAsync(subscriptionId, cancellationToken: cancellationToken);
            Customer customer = await customerService.GetAsync(subscription.CustomerId, cancellationToken: cancellationToken);

            if (!customer.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"User {user.Email} doesn't match subscription e-mail {subscription.Customer.Email}");
            }

            subscription = await subscriptionService.CancelAsync(subscriptionId, new SubscriptionCancelOptions(), cancellationToken: cancellationToken);

            context.DonationEventLog.Add(new DonationEventLog()
            {
                UserId = user.Id,
                Type = DonationEventType.Internal,
                EventData = subscription.ToJson()
            });
            await context.SaveChangesAsync(cancellationToken);
        }

        private async Task<Plan> CreateRecurringPlan(int amount, string currency, CancellationToken cancellationToken)
        {
            Product product = await GetOrCreateRecurringDonationProduct(cancellationToken);
            return await planService.CreateAsync(
                new PlanCreateOptions()
                {
                    ProductId = product.Id,
                    Active = true,
                    Amount = amount * 100,
                    Currency = currency,
                    Interval = "month",
                    BillingScheme = "per_unit",
                    UsageType = "licensed",
                    IntervalCount = 1
                },
                cancellationToken: cancellationToken);
        }

        private async Task<Product> GetOrCreateRecurringDonationProduct(CancellationToken cancellationToken)
        {
            var products = await productService.ListAsync(
                new ProductListOptions()
                {
                    Active = true,
                    Type = "service"
                },
                cancellationToken: cancellationToken);
            Product product = products.FirstOrDefault(p => p.Name == RecurringDonationProduct);
            if (product == null)
            {
                product = await productService.CreateAsync(
                    new ProductCreateOptions()
                    {
                        Active = true,
                        Name = RecurringDonationProduct,
                        StatementDescriptor = "Donation CollAction",
                        Type = "service"
                    },
                    cancellationToken: cancellationToken);
            }

            return product;
        }

        private async Task<Customer> GetOrCreateCustomer(string name, string email, CancellationToken cancellationToken)
        {
            Customer customer = (await customerService.ListAsync(new CustomerListOptions() { Email = email, Limit = 1 }, requestOptions, cancellationToken)).FirstOrDefault();
            string metadataName = null;
            customer?.Metadata?.TryGetValue(NameKey, out metadataName);
            var metadata = new Dictionary<string, string>() { { NameKey, name } };
            if (customer == null)
            {
                customer = await customerService.CreateAsync(
                    new CustomerCreateOptions()
                    {
                        Email = email,
                        Metadata = metadata
                    },
                    cancellationToken: cancellationToken);
            }
            else if (!name.Equals(metadataName, StringComparison.Ordinal))
            {
                customer = await customerService.UpdateAsync(
                    customer.Id, 
                    new CustomerUpdateOptions()
                    {
                        Metadata = metadata
                    },
                    cancellationToken: cancellationToken);
            }

            return customer;
        }

        private void ValidateDetails(string name, string email)
        {
            if (string.IsNullOrEmpty(name) || !email.Contains("@", StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Invalid user-details");
            }
        }

        private Task SendDonationThankYou(Customer customer, bool hasSubscriptions)
            => emailSender.SendEmailTemplated(customer.Email, "Thank you for your donation", "DonationThankYou", hasSubscriptions);

        private void ValidateDetails(int amount, string name, string email)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException($"Invalid amount requested: {amount}");
            }

            ValidateDetails(name, email);
        }
    }
}