using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CollAction.Services.Newsletter
{
    public class NewsletterSubscriptionService : INewsletterSubscriptionService
    {
        private class MailChimpErrorResponse
        {
            public string type { get; set; }
            public string title { get; set; }
            public int status { get; set; }
            public string detail { get; set; }
            public string instance { get; set; }
            public string[] errors { get; set; }

            public override string ToString()
            {
                StringBuilder errorResponse = new StringBuilder();
                errorResponse.AppendFormat("ERROR RESPONSE...\r", type);
                errorResponse.AppendFormat("type: {0}\r", type);
                errorResponse.AppendFormat("title: {0}\r", title);
                errorResponse.AppendFormat("status: {0}\r", status);
                errorResponse.AppendFormat("detail: {0}\r", detail);
                errorResponse.AppendFormat("instance: {0}\r", instance);
                errorResponse.AppendFormat("errors: {0}\r", string.Join(", ", errors ?? new string[0]));
                errorResponse.AppendFormat("\r");
                return errorResponse.ToString();
            }
        }

        public enum SubscriptionStatus { NotFound, Pending, Subscribed, Unknown };

        #pragma warning disable CS0649 // This field is deserialized to, the warning is incorrect
        private struct ListMemberInfo { public string status; }
        #pragma warning restore CS0649

        private readonly string _apiKey;
        private readonly string _dataCenter;
        private readonly string _newsletterListId;
        private readonly string _userId;
        private readonly IBackgroundJobClient _jobClient;
        private readonly ILogger<NewsletterSubscriptionService> _logger;

        private Uri RootUri { get { return new Uri(string.Format("https://{0}.api.mailchimp.com/3.0", _dataCenter)); } }

        public NewsletterSubscriptionService(IOptions<NewsletterSubscriptionServiceOptions> options, ILogger<NewsletterSubscriptionService> logger, IBackgroundJobClient jobClient)
        {
            _apiKey = options.Value.MailChimpKey;
            _dataCenter = _apiKey.Split('-')[1];
            _newsletterListId = options.Value.MailChimpNewsletterListId;
            _jobClient = jobClient;
            _logger = logger;
            _userId = options.Value.MailChimpUserId;
        }

        public async Task<bool> IsSubscribedAsync(string email)
        {
            SubscriptionStatus status = await GetListMemberStatusAsync(_newsletterListId, email);
            return status == SubscriptionStatus.Pending || status == SubscriptionStatus.Subscribed;
        }

        public void SetSubscription(string email, bool wantsSubscription, bool requireEmailConfirmationIfSubscribing)
        {
            string job = wantsSubscription ? _jobClient.Enqueue(() => AddOrUpdateListMemberAsync(_newsletterListId, email, requireEmailConfirmationIfSubscribing)) :
                                             _jobClient.Enqueue(() => DeleteListMemberAsync(_newsletterListId, email));
            _logger.LogInformation("changed maillist subscription for {0} setting it to {1} with require email confirmation to {2} and hangfire job {3}", email, wantsSubscription, requireEmailConfirmationIfSubscribing, job);
        }

        public async Task<SubscriptionStatus> GetListMemberStatusAsync(string listId, string email)
        {
            using (var client = GetClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(GetListMemberUri(listId, email) + "?fields=status"))
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return SubscriptionStatus.NotFound;
                    }

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        string errorResponse = await GetMailChimpErrorResponse(response);
                        throw new Exception("Failed to get MailChimp list member status. Status code: " + response.StatusCode + " -> \r" + errorResponse);
                    }

                    ListMemberInfo info = JsonConvert.DeserializeObject<ListMemberInfo>(await response.Content.ReadAsStringAsync());
                    switch (info.status)
                    {
                        case "pending": return SubscriptionStatus.Pending;
                        case "subscribed": return SubscriptionStatus.Subscribed;
                        default: return SubscriptionStatus.Unknown;
                    }
                }
            }
        }

        public async Task AddOrUpdateListMemberAsync(string listId, string email, bool usePendingStatusIfNew = true)
        {
            using (var client = GetClient())
            {
                StringContent content = new StringContent(GetAddOrUpdateListMemberParametersJSON(email, usePendingStatusIfNew), Encoding.UTF8, "application/json");
                using (HttpResponseMessage response = await client.PutAsync(GetListMemberUri(listId, email), content))
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        string errorResponse = await GetMailChimpErrorResponse(response);
                        string httpRequestHeader = client.DefaultRequestHeaders.ToString();
                        string httpRequestBaseAddress = client.BaseAddress.ToString();
                        string diagnostics = string.Format("{0}httpRequestHeader: {1}\rhttpRequestBaseAddress:{2}", errorResponse, httpRequestHeader, httpRequestBaseAddress);
                        throw new Exception("Failed to add or update MailChimp list member. Status code: " + response.StatusCode + " -> \r" + diagnostics);
                    }
                }
            }
        }

        public async Task DeleteListMemberAsync(string listId, string email)
        {
            using (var client = GetClient())
            {
                using (HttpResponseMessage response = await client.DeleteAsync(GetListMemberUri(listId, email)))
                {
                    if (response.StatusCode != HttpStatusCode.NoContent && response.StatusCode != HttpStatusCode.NotFound)
                    {
                        string errorResponse = await GetMailChimpErrorResponse(response);
                        throw new Exception("Failed to delete MailChimp list member. Status code: " + response.StatusCode + " -> \r" + errorResponse);
                    }
                }
            }
        }

        private HttpClient GetClient()
        {
            HttpClient client = new HttpClient();

            var byteArray = Encoding.ASCII.GetBytes($"collaction:{_apiKey}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            client.BaseAddress = RootUri;
            return client;
        }

        private string GetListMemberUri(string listId, string email)
        {
            return String.Format("{0}/lists/{1}/members/{2}", RootUri, listId, GetMemberHash(email));
        }

        private string GetAddOrUpdateListMemberParametersJSON(string email, bool usePendingStatusIfNew)
        {
            var subscribeParameters = new
            {
                email_address = email,
                status_if_new = usePendingStatusIfNew ? "pending" : "subscribed"
            };
            return JsonConvert.SerializeObject(subscribeParameters);
        }

        private string GetMemberHash(string email)
        {
            string input = email.ToLower();
            using (MD5 cryptoService = MD5.Create())
            {
                byte[] data = cryptoService.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    builder.Append(data[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private async Task<string> GetMailChimpErrorResponse(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<MailChimpErrorResponse>(await response.Content.ReadAsStringAsync()).ToString();
        }
    }
}