using AutoMapper;
using Hangfire;
using Microsoft.Extensions.Options;
using Stripe;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.Services.Donation
{
    public class DonationService : IDonationService
    {
        private readonly RequestOptions _options;
        private readonly CustomerService _customerService;
        private readonly ChargeService _chargeService;
        /*
        private readonly ProductService _productService;
        private readonly PlanService _planService;
        private readonly SubscriptionService _subscriptionService;
        private Product _donationProduct;

        private readonly static ProductCreateOptions DonationProductSettings = 
            new ProductCreateOptions()
            {
                Id = "COLL_RECUR_PROD",
                Name = "CollAction Recurring Donation",
                Caption = "A recurring donation to Stichting CollAction",
                Description = "A recurring donation to Stichting CollAction",
                StatementDescriptor = "A recurring donation to Stichting CollAction",
                Shippable = false,
                UnitLabel = "Amount",
                Type = "service",
                Active = true
            };
        */

        public DonationService(IOptions<RequestOptions> options)
        {
            _options = options.Value;
            _customerService = new CustomerService(_options.ApiKey);
            _chargeService = new ChargeService(_options.ApiKey);
            /*
            _subscriptionService = new SubscriptionService(_options.ApiKey);
            _productService = new ProductService(_options.ApiKey);
            _planService = new PlanService(_options.ApiKey);
            */
        }

        /*
        public async Task Initialize()
        {
            _donationProduct = await _productService.GetAsync(DonationProductSettings.Id, _options);
            if (_donationProduct == null)
            {
                _donationProduct = await _productService.CreateAsync(DonationProductSettings);
            }
            else
            {
                _donationProduct = await _productService.UpdateAsync(DonationProductSettings.Id, _mapper.Map<ProductUpdateOptions>(DonationProductSettings), _options);
            }
        }

        public async Task ChargeRepeating(string email, string token, long amount, string period, string currency)
        {
            Customer customer = (await _customerService.ListAsync(new CustomerListOptions() { Email = email, Limit = 1 }, _options)).FirstOrDefault();
            if (customer == null)
            {
                customer = await _customerService.CreateAsync(new CustomerCreateOptions()
                {
                    Email = email,
                    SourceToken = token
                });
            }

            string planId = $"COLL_PLAN_{currency}_{period}_{amount}";
            Plan plan = await _planService.GetAsync(planId);
            if (plan == null)
            {
                plan = await _planService.CreateAsync(new PlanCreateOptions() { Active = true, Currency = currency, Amount = amount });
            }

            await _subscriptionService.CreateAsync(new SubscriptionCreateOptions() { });

            Charge charge = await _chargeService.CreateAsync(new ChargeCreateOptions()
            {
                Amount = amount,
                Description = "Donation to CollAction",
                Currency = currency,
                CustomerId = customer.Id,
            });
        }*/

        public async Task Charge(string email, string token, long amount, string currency)
        {
            Customer customer = (await _customerService.ListAsync(new CustomerListOptions() { Email = email, Limit = 1 }, _options)).FirstOrDefault();
            if (customer == null)
            {
                customer = await _customerService.CreateAsync(new CustomerCreateOptions()
                {
                    Email = email,
                    SourceToken = token
                });
            }

            Charge charge = await _chargeService.CreateAsync(new ChargeCreateOptions()
            {
                Amount = amount,
                Description = "Donation to CollAction",
                Currency = currency,
                CustomerId = customer.Id,
            });
        }
    }
}