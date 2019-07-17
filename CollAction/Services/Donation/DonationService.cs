﻿using CollAction.Data;
using CollAction.Models;
using CollAction.Services.Email;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.Services.Donation
{
    /// <summary>
    /// There are 4 donation flows:
    /// * Non-recurring iDeal payments
    ///   - The flow is started at DebitDetails.tsx where a source with the relevant details is created client-side
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
        const string StatusChargeable = "chargeable";
        const string StatusConsumed = "consumed";
        const string EventTypeChargeableSource = "source.chargeable";
        const string EventTypeChargeSucceeded = "charge.succeeded";
        const string NameKey = "name";
        const string RecurringDonationProduct = "Recurring Donation Stichting CollAction";

        private readonly CustomerService _customerService;
        private readonly SourceService _sourceService;
        private readonly ChargeService _chargeService;
        private readonly SessionService _sessionService;
        private readonly SubscriptionService _subscriptionService;
        private readonly PlanService _planService;
        private readonly ProductService _productService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly StripeSignatures _stripeSignatures;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly RequestOptions _requestOptions;
        private readonly SiteOptions _siteOptions;
        private readonly StripeClient _stripeClient;

        public DonationService(IOptions<RequestOptions> requestOptions, IOptions<SiteOptions> siteOptions, UserManager<ApplicationUser> userManager, ApplicationDbContext context, IBackgroundJobClient backgroundJobClient, IOptions<StripeSignatures> stripeSignatures, IEmailSender emailSender)
        {
            _requestOptions = requestOptions.Value;
            _siteOptions = siteOptions.Value;
            _stripeClient = new StripeClient(_requestOptions.ApiKey);
            _customerService = new CustomerService(_stripeClient);
            _sourceService = new SourceService(_stripeClient);
            _chargeService = new ChargeService(_stripeClient);
            _sessionService = new SessionService(_stripeClient);
            _subscriptionService = new SubscriptionService(_stripeClient);
            _planService = new PlanService(_stripeClient);
            _productService = new ProductService(_stripeClient);
            _backgroundJobClient = backgroundJobClient;
            _stripeSignatures = stripeSignatures.Value;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<bool> HasIdealPaymentSucceeded(string sourceId, string clientSecret)
        {
            Source source = await _sourceService.GetAsync(sourceId);
            return (source.Status == StatusChargeable || source.Status == StatusConsumed) && 
                   clientSecret == source.ClientSecret;
        }

        /*
         * Here we're initializing a stripe checkout session for paying with a credit card. The upcoming SCA regulations ensure we have to do it through this API, because it's the only one that /easily/ supports things like 3D secure.
         */
        public async Task<string> InitializeCreditCardCheckout(string currency, int amount, string name, string email, bool recurring)
        {
            ValidateDetails(amount, name, email);

            ApplicationUser user = await _userManager.FindByEmailAsync(email);

            var sessionOptions = new SessionCreateOptions()
            {
                SuccessUrl = $"{_siteOptions.PublicAddress}/Donation/ThankYou",
                CancelUrl = $"{_siteOptions.PublicAddress}/Donation/Donate",
                PaymentMethodTypes = new List<string>
                {
                    "card",
                }
            };

            Customer customer = await GetOrCreateCustomer(name, email);
            if (recurring)
            {
                sessionOptions.CustomerId = customer.Id;
                sessionOptions.SubscriptionData = new SessionSubscriptionDataOptions()
                {
                    Items = new List<SessionSubscriptionDataItemOptions>()
                    {
                        new SessionSubscriptionDataItemOptions()
                        {
                            PlanId = (await CreateRecurringPlan(amount, currency)).Id,
                            Quantity = 1
                        }
                    }
                };
            }
            else
            {
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

            Session session = await _sessionService.CreateAsync(sessionOptions);

            _context.DonationEventLog.Add(new DonationEventLog()
            {
                UserId = user?.Id,
                Type = DonationEventType.Internal,
                EventData = session.ToJson()
            });
            await _context.SaveChangesAsync();

            return session.Id;
        }

        /*
         * Here we're initializing a stripe SEPA subscription on a source with sepa/iban data. This subscription should auto-charge.
         */
        public async Task InitializeSepaDirect(string sourceId, string name, string email, int amount)
        {
            ValidateDetails(amount, name, email);

            ApplicationUser user = await _userManager.FindByEmailAsync(email);
            Customer customer = await GetOrCreateCustomer(name, email);
            Source source = await _sourceService.AttachAsync(customer.Id, new SourceAttachOptions()
            {
                Source = sourceId
            });
            Plan plan = await CreateRecurringPlan(amount, "eur");
            Subscription subscription = await _subscriptionService.CreateAsync(new SubscriptionCreateOptions()
            {
                DefaultSource = source.Id,
                CollectionMethod = "charge_automatically",
                CustomerId = customer.Id,
                Items = new List<SubscriptionItemOption>()
                {
                    new SubscriptionItemOption()
                    {
                        PlanId = plan.Id,
                        Quantity = 1
                    }
                }
            });

            _context.DonationEventLog.Add(new DonationEventLog()
            {
                UserId = user?.Id,
                Type = DonationEventType.Internal,
                EventData = subscription.ToJson()
            });
            await _context.SaveChangesAsync();
        }

        /*
         * Here we're initializing the part of the iDeal payment that has to happen on the backend. For now, that's only attaching an actual customer record to the payment source.
         * In the future to handle SCA, we might need to start using payment intents or checkout here. SCA starts from september the 14th. The support for iDeal is not there yet though, so we'll have to wait.
         */
        public async Task InitializeIdealCheckout(string sourceId, string name, string email)
        {
            ValidateDetails(name, email);

            ApplicationUser user = await _userManager.FindByEmailAsync(email);
            Customer customer = await GetOrCreateCustomer(name, email);
            Source source = await _sourceService.AttachAsync(customer.Id, new SourceAttachOptions()
            {
                Source = sourceId
            });

            _context.DonationEventLog.Add(new DonationEventLog()
            {
                UserId = user?.Id,
                Type = DonationEventType.Internal,
                EventData = source.ToJson()
            });
            await _context.SaveChangesAsync();
        }

        /*
         * We're receiving an event from the stripe webhook, an payment source can be charge. We're queueing it up so we can retry it as much as possible.
         * In the future to handle SCA, we might need to start using payment intents or checkout here. SCA starts from september the 14th. The support for iDeal is not there yet though, so we'll have to wait.
         */
        public void HandleChargeable(string json, string signature)
        {
            Event stripeEvent = EventUtility.ConstructEvent(json, signature, _stripeSignatures.StripeChargeableWebhookSecret);
            if (stripeEvent.Type == EventTypeChargeableSource)
            {
                Source source = (Source)stripeEvent.Data.Object;
                _backgroundJobClient.Enqueue(() => Charge(source.Id));
            }
            else
            {
                throw new InvalidOperationException($"invalid event sent to source.chargeable webhook: {stripeEvent.ToJson()}");
            }
        }

        /*
         * We're logging all stripe events here. For audit purposes, and maybe the dwh team can make something pretty out of this data.
         */
        public async Task LogPaymentEvent(string json, string signature)
        {
            Event stripeEvent = EventUtility.ConstructEvent(json, signature, _stripeSignatures.StripePaymentEventWebhookSecret);

            _context.DonationEventLog.Add(new DonationEventLog()
            {
                Type = DonationEventType.External,
                EventData = stripeEvent.ToJson()
            });
            await _context.SaveChangesAsync();

            if (stripeEvent.Type == EventTypeChargeSucceeded)
            {
                Charge charge = (Charge)stripeEvent.Data.Object;
                var subscriptions = await _subscriptionService.ListAsync(new SubscriptionListOptions() { CustomerId = charge.CustomerId });
                Customer customer = await _customerService.GetAsync(charge.CustomerId);
                await SendDonationThankYou(customer, subscriptions.Any());
            }
        }

        /*
         * Charge a source here (only used for iDeal right now). It's a background job so it can be restarted.
         */
        public async Task Charge(string sourceId)
        {
            Source source = await _sourceService.GetAsync(sourceId);
            if (source.Status == StatusChargeable)
            {
                Charge charge = await _chargeService.CreateAsync(new ChargeCreateOptions()
                {
                    Amount = source.Amount,
                    Currency = source.Currency,
                    Source = sourceId,
                    CustomerId = source.Customer,
                    Description = "A donation to Stichting CollAction"
                });

                Customer customer = await _customerService.GetAsync(source.Customer);
                ApplicationUser user = customer != null ? await _userManager.FindByEmailAsync(customer.Email) : null;
                _context.DonationEventLog.Add(new DonationEventLog()
                {
                    UserId = user?.Id,
                    Type = DonationEventType.Internal,
                    EventData = charge.ToJson()
                });
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException($"source: {source.Id} is not chargeable, something went wrong in the payment flow");
            }
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptionsFor(ApplicationUser userFor)
        {
            var customers = await _customerService.ListAsync(new CustomerListOptions()
            {
                Email = userFor.Email
            });

            var subscriptions = await Task.WhenAll(
                customers.Select(c =>
                    _subscriptionService.ListAsync(new SubscriptionListOptions()
                    {
                        CustomerId = c.Id
                    })));

            return subscriptions.SelectMany(s => s);
        }

        public async Task CancelSubscription(string subscriptionId, ApplicationUser userFor)
        {
            Subscription subscription = await _subscriptionService.GetAsync(subscriptionId);
            Customer customer = await _customerService.GetAsync(subscription.CustomerId);

            if (!customer.Email.Equals(userFor.Email, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"User {userFor.Email} doesn't match subscription e-mail {subscription.Customer.Email}");
            }

            subscription = await _subscriptionService.CancelAsync(subscriptionId, new SubscriptionCancelOptions());

            _context.DonationEventLog.Add(new DonationEventLog()
            {
                UserId = userFor.Id,
                Type = DonationEventType.Internal,
                EventData = subscription.ToJson()
            });
            await _context.SaveChangesAsync();
        }

        private async Task<Plan> CreateRecurringPlan(int amount, string currency)
        {
            Product product = await GetOrCreateRecurringDonationProduct();
            return await _planService.CreateAsync(new PlanCreateOptions()
            {
                Product = product.Id,
                Active = true,
                Amount = amount * 100,
                Currency = currency,
                Interval = "month",
                BillingScheme = "per_unit",
                UsageType = "licensed",
                IntervalCount = 1
            });
        }

        private async Task<Product> GetOrCreateRecurringDonationProduct()
        {
            var products = await _productService.ListAsync(new ProductListOptions()
            {
                Active = true,
                Type = "service"
            });
            Product product = products.FirstOrDefault(p => p.Name == RecurringDonationProduct);
            if (product == null)
            {
                product = await _productService.CreateAsync(new ProductCreateOptions()
                {
                    Active = true,
                    Name = RecurringDonationProduct,
                    StatementDescriptor = "Donation CollAction",
                    Type = "service"
                });
            }
            return product;
        }

        private async Task<Customer> GetOrCreateCustomer(string name, string email)
        {
            Customer customer = (await _customerService.ListAsync(new CustomerListOptions() { Email = email, Limit = 1 }, _requestOptions)).FirstOrDefault();
            string metadataName = null;
            customer?.Metadata?.TryGetValue(NameKey, out metadataName);
            var metadata = new Dictionary<string, string>() { { NameKey, name } };
            if (customer == null)
            {
                customer = await _customerService.CreateAsync(new CustomerCreateOptions()
                {
                    Email = email,
                    Metadata = metadata
                });
            }
            else if (!name.Equals(metadataName, StringComparison.Ordinal))
            {
                customer = await _customerService.UpdateAsync(customer.Id, new CustomerUpdateOptions()
                {
                    Metadata = metadata
                });
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
            => _emailSender.SendEmailTemplated(customer.Email, "Thank you for your donation", "DonationThankYou", hasSubscriptions);

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