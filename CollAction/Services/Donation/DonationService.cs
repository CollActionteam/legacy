using CollAction.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CollAction.Services.Donation
{
    public class DonationService : IDonationService
    {
        private readonly CustomerService _customerService;
        private readonly SourceService _sourceService;
        private readonly RequestOptions _requestOptions;
        private readonly SiteOptions _siteOptions;

        public DonationService(IOptions<RequestOptions> requestOptions, IOptions<SiteOptions> siteOptions)
        {
            _requestOptions = requestOptions.Value;
            _siteOptions = siteOptions.Value;
            _customerService = new CustomerService(_requestOptions.ApiKey);
            _sourceService = new SourceService(_requestOptions.ApiKey);
        }

        public async Task<Customer> GetOrCreateCustomer(ApplicationUser applicationUser)
        {
            if (applicationUser == null)
            {
                return null;
            }

            Customer customer = (await _customerService.ListAsync(new CustomerListOptions() { Email = applicationUser.Email, Limit = 1 }, _requestOptions)).FirstOrDefault();
            if (customer == null)
            {
                customer = await _customerService.CreateAsync(new CustomerCreateOptions()
                {
                    Email = applicationUser.Email
                });
            }
            return customer;
        }

        public async Task<string> InitializeCreditCardCheckout(string currency, int amount, ApplicationUser user)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException($"Invalid amount requested: {amount}");
            }

            Customer customer = await GetOrCreateCustomer(user);

            using (var client = new HttpClient())
            {
                var requestContent = new Dictionary<string, string>()
                {
                    { "success_url", $"{_siteOptions.PublicAddress}/Donation/ThankYou" },
                    { "cancel_url", $"{_siteOptions.PublicAddress}/Donation/Donate" },
                    { "payment_method_types[]", "card" },
                    { "line_items[][amount]", (amount * 100).ToString() },
                    { "line_items[][currency]", currency },
                    { "line_items[][name]", "donation" },
                    { "line_items[][description]", "A donation to Stichting CollAction" },
                    { "line_items[][quantity]", "1" }
                };

                if (customer != null)
                {
                    requestContent["customer"] = customer.Id;
                }

                using (var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://api.stripe.com/v1/checkout/sessions"),
                    Method = HttpMethod.Post,
                    Content = new FormUrlEncodedContent(requestContent)
                })
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(_requestOptions.ApiKey + ":")));
                    request.Headers.Add("Stripe-Version", "2019-03-14; checkout_sessions_beta=v1");

                    HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
                    string content = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = JObject.Parse(content);
                        return ((JValue)responseContent["id"]).Value<string>();
                    }
                    else
                    {
                        throw new InvalidOperationException($"Stripe checkout returned: {response.StatusCode}: '{content}'");
                    }
                }
            }
        }

        /*
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
        }

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
        }*/
    }
}