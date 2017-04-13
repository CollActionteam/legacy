using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;

namespace CollAction.Helpers
{
    public class MailChimpManager
    {
        public struct ListMemberInfo { public string status; }

        private readonly string _apiKey;
        private readonly string _dataCenter;

        private Uri RootUri { get { return new Uri(String.Format("https://{0}.api.mailchimp.com/3.0", _dataCenter)); } }

        public MailChimpManager(string apiKey)
        {
            _apiKey = apiKey;
            _dataCenter = apiKey.Split('-')[1];
        }

        public async Task<String> GetListMemberStatusAsync(string listId, string email)
        {
            var client = PrepareHttpClient();
            HttpResponseMessage response = await client.GetAsync(GetListMemberUri(listId, email) + "?fields=status");

            if (response.StatusCode == HttpStatusCode.NotFound) { return null; }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Failed to add or update MailChimp list member. Status code: " + response.StatusCode);
            }

            ListMemberInfo info = JsonConvert.DeserializeObject<ListMemberInfo>(await response.Content.ReadAsStringAsync());
            return info.status;
        }

        public async Task AddOrUpdateListMemberAsync(string listId, string email, bool usePendingStatusIfNew = true)
        {
            var client = PrepareHttpClient();
            StringContent content = new StringContent(GetAddOrUpdateListMemberParametersJSON(email, usePendingStatusIfNew), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(GetListMemberUri(listId, email), content);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Failed to add or update MailChimp list member. Status code: " + response.StatusCode);
            }
        }

        public async Task DeleteListMemberAsync(string listId, string email)
        {
            var client = PrepareHttpClient();
            HttpResponseMessage response = await client.DeleteAsync(GetListMemberUri(listId, email));

            if (response.StatusCode != HttpStatusCode.NoContent && response.StatusCode != HttpStatusCode.NotFound)
            {
                throw new Exception("Failed to delete MailChimp list member. Status code: " + response.StatusCode);
            }
        }

        private HttpClient PrepareHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _apiKey);
            client.BaseAddress = RootUri;
            return client;
        }

        private string GetListMemberUri(string listId, string email)
        {
            return String.Format("{0}/lists/{1}/members/{2}", RootUri, listId, GetMemberHash(email));
        }

        private string GetAddOrUpdateListMemberParametersJSON(string email, bool usePendingStatusIfNew)
        {
            var subscribeParameters = new {
                email_address = email,
                status_if_new = usePendingStatusIfNew ? "pending" : "subscribed"
            };
            return JsonConvert.SerializeObject(subscribeParameters);
        }

        private string GetMemberHash(string email)
        {
            string input = email.ToLower();
            MD5 cryptoService = MD5.Create();
            byte[] data = cryptoService.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
