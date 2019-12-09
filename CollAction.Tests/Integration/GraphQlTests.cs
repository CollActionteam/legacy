using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Tests.Integration
{
    [TestClass]
    [TestCategory("Integration")]
    public class GraphQlTests : IntegrationTestBase
    {
        [TestMethod]
        public Task TestProjectList()
            => WithTestServer(
                   async (scope, testServer) =>
                   {
                       const string Query = @"
                           query {
                               projects {
                                   id
                               }
                           }";

                       var response = await PerformGraphQlQuery(testServer, Query, null);
                       Assert.IsTrue(response.IsSuccessStatusCode);
                   });

        private static async Task<HttpResponseMessage> PerformGraphQlQuery(TestServer server, string query, dynamic variables)
        {
            using (var httpClient = server.CreateClient())
            {
               // Test with columns provided
               string jsonBody =
                   JsonSerializer.Serialize(
                       new
                       {
                           Query = query,
                           Variables = variables
                       });
               var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

               using (content)
               {
                   return await httpClient.PostAsync(new Uri("/graphql", UriKind.Relative), content, CancellationToken.None).ConfigureAwait(false);
               }
           }
        }
    }
}
