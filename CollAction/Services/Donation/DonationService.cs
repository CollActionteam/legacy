using CollAction.Data;
using CollAction.Models;
using Microsoft.AspNetCore.Identity;
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
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RequestOptions _requestOptions;
        private readonly SiteOptions _siteOptions;

        public DonationService(IOptions<RequestOptions> requestOptions, IOptions<SiteOptions> siteOptions, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _requestOptions = requestOptions.Value;
            _siteOptions = siteOptions.Value;
            _customerService = new CustomerService(_requestOptions.ApiKey);
            _sourceService = new SourceService(_requestOptions.ApiKey);
            _context = context;
            _userManager = userManager;
        }

        public async Task<string> InitializeCreditCardCheckout(string currency, int amount, string name, string email)
        {
            ValidateDetails(amount, name, email);

            ApplicationUser user = await _userManager.FindByEmailAsync(email);
            Customer customer = await GetOrCreateCustomer(name, email);

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
                    { "line_items[][quantity]", "1" },
                    { "customer", customer.Id }
                };

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
                        string checkoutId = ((JValue)responseContent["id"]).Value<string>();

                        _context.DonationEventLog.Add(new DonationEventLog()
                        {
                            UserId = user?.Id,
                            Type = DonationEventType.Internal,
                            EventData = content
                        });
                        await _context.SaveChangesAsync();

                        return checkoutId;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Stripe checkout returned: {response.StatusCode}: '{content}'");
                    }
                }
            }
        }

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

        public Task LogExternalEvent(JObject stripeEvent)
        {
            _context.DonationEventLog.Add(new DonationEventLog()
            {
                Type = DonationEventType.External,
                EventData = stripeEvent.ToString()
            });
            return _context.SaveChangesAsync();
        }

        private async Task<Customer> GetOrCreateCustomer(string name, string email)
        {
            Customer customer = (await _customerService.ListAsync(new CustomerListOptions() { Email = email, Limit = 1 }, _requestOptions)).FirstOrDefault();
            string metadataName = null;
            customer?.Metadata?.TryGetValue("name", out metadataName);
            var metadata = new Dictionary<string, string>() { { "name", name } };
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