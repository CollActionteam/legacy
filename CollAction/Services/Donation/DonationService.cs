using CollAction.Data;
using CollAction.Models;
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
    public class DonationService : IDonationService
    {
        const string StatusChargeable = "chargeable";
        const string StatusConsumed = "consumed";
        const string EventTypeChargeableSource = "source.chargeable";
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
        private readonly RequestOptions _requestOptions;
        private readonly SiteOptions _siteOptions;

        public DonationService(IOptions<RequestOptions> requestOptions, IOptions<SiteOptions> siteOptions, UserManager<ApplicationUser> userManager, ApplicationDbContext context, IBackgroundJobClient backgroundJobClient, IOptions<StripeSignatures> stripeSignatures)
        {
            _requestOptions = requestOptions.Value;
            _siteOptions = siteOptions.Value;
            _customerService = new CustomerService(_requestOptions.ApiKey);
            _sourceService = new SourceService(_requestOptions.ApiKey);
            _chargeService = new ChargeService(_requestOptions.ApiKey);
            _sessionService = new SessionService(_requestOptions.ApiKey);
            _subscriptionService = new SubscriptionService(_requestOptions.ApiKey);
            _planService = new PlanService(_requestOptions.ApiKey);
            _productService = new ProductService(_requestOptions.ApiKey);
            _backgroundJobClient = backgroundJobClient;
            _stripeSignatures = stripeSignatures.Value;
            _context = context;
            _userManager = userManager;
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
            Customer customer = await GetOrCreateCustomer(name, email);

            var sessionOptions = new SessionCreateOptions()
            {
                SuccessUrl = $"{_siteOptions.PublicAddress}/Donation/ThankYou",
                CancelUrl = $"{_siteOptions.PublicAddress}/Donation/Donate",
                PaymentMethodTypes = new List<string>
                {
                    "card",
                }
            };

            if (recurring)
            {
                sessionOptions.CustomerEmail = customer.Email; // TODO: Once supported, replace this with the customer id
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
         * Here we're initializing the part of the iDeal payment that has to happen on the backend. For now, that's only attaching an actual customer record to the payment source.
         * In the future to handle SCA, we might need to start using payment intents or checkout here. SCA starts from september the 14th. The support for iDeal is not there yet though, so we'll have to wait.
         */
        public async Task InitializeIdealCheckout(string sourceId, string name, string email, bool recurring)
        {
            // TODO: recurring

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
                string sourceId = ((Source)stripeEvent.Data.Object)?.Id;
                _backgroundJobClient.Enqueue(() => Charge(sourceId));
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
        }

        public async Task Charge(string sourceId)
        {
            Source source = await _sourceService.GetAsync(sourceId);
            if (source.Status == StatusChargeable)
            {
                Charge charge = await _chargeService.CreateAsync(new ChargeCreateOptions()
                {
                    Amount = source.Amount,
                    Currency = source.Currency,
                    SourceId = sourceId,
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

        private async Task<Plan> CreateRecurringPlan(int amount, string currency)
        {
            Product product = await GetOrCreateRecurringDonationProduct();
            return await _planService.CreateAsync(new PlanCreateOptions()
            {
                ProductId = product.Id,
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