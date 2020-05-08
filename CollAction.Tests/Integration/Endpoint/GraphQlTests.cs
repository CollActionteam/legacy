using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using CollAction.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace CollAction.Tests.Integration.Endpoint
{
    [TestClass]
    [TestCategory("Integration")]
    public sealed class GraphQlTests : IntegrationTestBase
    {
        [TestMethod]
        public Task TestCrowdactionList()
            => WithTestServer(
                   async (scope, testServer) =>
                   {
                       const string QueryCrowdactions = @"
                           query {
                               crowdactions {
                                   id
                                   name
                                   categories {
                                     category
                                   }
                                   descriptiveImage {
                                       filepath
                                   }
                               }
                           }";

                       HttpResponseMessage response = await PerformGraphQlQuery(testServer, QueryCrowdactions, null).ConfigureAwait(false);
                       string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                       Assert.IsTrue(response.IsSuccessStatusCode, content);
                       JsonDocument result = JsonDocument.Parse(content);
                       Assert.ThrowsException<KeyNotFoundException>(() => result.RootElement.GetProperty("errors"), content);
                       JsonElement.ArrayEnumerator crowdactions = result.RootElement.GetProperty("data").GetProperty("crowdactions").EnumerateArray();
                       Assert.IsTrue(crowdactions.Any(), content);

                       string crowdactionId = crowdactions.First().GetProperty("id").GetString();
                       const string QueryCrowdaction = @"
                           query($crowdactionId : ID!) {
                               crowdaction(id: $crowdactionId) {
                                   id
                                   name
                                   descriptiveImage {
                                       filepath
                                   }
                               }
                           }";
                       dynamic variables = new { crowdactionId };
                       response = await PerformGraphQlQuery(testServer, QueryCrowdaction, variables);
                       content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                       Assert.IsTrue(response.IsSuccessStatusCode, content);
                       result = JsonDocument.Parse(content);
                       Assert.ThrowsException<KeyNotFoundException>(() => result.RootElement.GetProperty("errors"), content);
                       JsonElement crowdaction = result.RootElement.GetProperty("data").GetProperty("crowdaction");
                       Assert.AreEqual(crowdactionId.ToString(CultureInfo.InvariantCulture), crowdaction.GetProperty("id").GetString());
                   });

        [TestMethod]
        public Task TestAuthorization()
            => WithTestServer(
                   async (scope, testServer) =>
                   {
                       const string QueryCrowdactions = @"
                           query {
                               crowdactions {
                                   id
                                   name
                                   participants {
                                       user {
                                           userName
                                       }
                                   }
                                   isSuccessfull
                                   isFailed
                                   canSendCrowdactionEmail
                             }
                           }";

                       HttpResponseMessage response = await PerformGraphQlQuery(testServer, QueryCrowdactions, null).ConfigureAwait(false);
                       string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                       Assert.IsTrue(response.IsSuccessStatusCode, content);
                       JsonDocument result = JsonDocument.Parse(content);
                       Assert.IsNotNull(result.RootElement.GetProperty("errors"), content);

                       SeedOptions seedOptions = scope.ServiceProvider.GetRequiredService<IOptions<SeedOptions>>().Value;
                       using var httpClient = testServer.CreateClient();
                       // Retry call as admin
                       httpClient.DefaultRequestHeaders.Add("Cookie", await GetAuthCookie(httpClient, seedOptions).ConfigureAwait(false));
                       response = await PerformGraphQlQuery(httpClient, QueryCrowdactions, null).ConfigureAwait(false);
                       content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                       Assert.IsTrue(response.IsSuccessStatusCode, content);
                       result = JsonDocument.Parse(content);
                       Assert.ThrowsException<KeyNotFoundException>(() => result.RootElement.GetProperty("errors"), content);
                   });

        [TestMethod]
        public Task TestCreateCrowdaction()
            => WithTestServer(
                   async (scope, testServer) =>
                   {
                       string createCrowdaction = $@"
                           mutation {{
                               crowdaction {{
                                   createCrowdaction(crowdaction:
                                       {{
                                           name:""{Guid.NewGuid()}"",
                                           categories: [COMMUNITY, ENVIRONMENT],
                                           target: 55,
                                           proposal: ""44"",
                                           description: ""test"",
                                           goal: ""dd"",
                                           creatorComments: ""dd"",
                                           start: ""{DateTime.UtcNow.AddDays(10).ToString("o", CultureInfo.InvariantCulture)}"",
                                           end: ""{DateTime.UtcNow.AddDays(20).ToString("o", CultureInfo.InvariantCulture)}"",
                                           descriptionVideoLink: ""https://www.youtube-nocookie.com/watch?v=a1"",
                                           tags:[""b"", ""a""]
                                       }}) {{
                                           succeeded
                                           crowdaction {{
                                               id
                                           }}
                                           errors {{
                                               memberNames
                                               errorMessage
                                           }}
                                       }}
                                   }}
                               }}";

                       SeedOptions seedOptions = scope.ServiceProvider.GetRequiredService<IOptions<SeedOptions>>().Value;
                       using var httpClient = testServer.CreateClient();
                       httpClient.DefaultRequestHeaders.Add("Cookie", await GetAuthCookie(httpClient, seedOptions).ConfigureAwait(false));
                       HttpResponseMessage response = await PerformGraphQlQuery(httpClient, createCrowdaction, null).ConfigureAwait(false);
                       string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                       JsonDocument result = JsonDocument.Parse(content);
                       Assert.IsTrue(response.IsSuccessStatusCode, content);
                       Assert.ThrowsException<KeyNotFoundException>(() => result.RootElement.GetProperty("errors"), content);
                   });

        private static async Task<HttpResponseMessage> PerformGraphQlQuery(TestServer testServer, string query, dynamic variables)
        {
            using var httpClient = testServer.CreateClient();
            return await PerformGraphQlQuery(httpClient, query, variables);
        }

        private static async Task<HttpResponseMessage> PerformGraphQlQuery(HttpClient httpClient, string query, dynamic variables)
        {
            // Test with columns provided
            string jsonBody =
                JsonSerializer.Serialize(
                    new
                    {
                        query,
                        variables
                    });
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            using (content)
            {
                return await httpClient.PostAsync(new Uri("/graphql", UriKind.Relative), content, CancellationToken.None).ConfigureAwait(false);
            }
        }
    }
}
