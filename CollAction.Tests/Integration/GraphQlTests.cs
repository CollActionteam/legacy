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
                       const string QueryProjects = @"
                           query {
                               projects {
                                   id
                                   name
                                   descriptiveImage {
                                       filepath
                                   }
                               }
                           }";

                       HttpResponseMessage response = await PerformGraphQlQuery(testServer, QueryProjects, null);
                       string content = await response.Content.ReadAsStringAsync();
                       Assert.IsTrue(response.IsSuccessStatusCode, content);
                       JsonDocument result = JsonDocument.Parse(content);
                       Assert.ThrowsException<KeyNotFoundException>(() => result.RootElement.GetProperty("errors"), content);
                       JsonElement projects = result.RootElement.GetProperty("data").GetProperty("projects");
                       Assert.IsTrue(projects.GetArrayLength() > 0, content);

                       int projectId = projects.EnumerateArray().First().GetProperty("id").GetInt32();
                       const string QueryProject = @"
                           query($projectId : ID!) {
                               project(id: $projectId) {
                                   id
                                   name
                                   descriptiveImage {
                                       filepath
                                   }
                               }
                           }";
                       dynamic variables = new { projectId };
                       response = await PerformGraphQlQuery(testServer, QueryProject, variables);
                       content = await response.Content.ReadAsStringAsync();
                       Assert.IsTrue(response.IsSuccessStatusCode, content);
                       result = JsonDocument.Parse(content);
                       Assert.ThrowsException<KeyNotFoundException>(() => result.RootElement.GetProperty("errors"), content);
                       JsonElement project = result.RootElement.GetProperty("data").GetProperty("project");
                       Assert.AreEqual(projectId, project.GetProperty("id").GetInt32());
                   });

        [TestMethod]
        public Task TestAuthorization()
            => WithTestServer(
                   async (scope, testServer) =>
                   {
                       const string QueryProjects = @"
                           query {
                               projects {
                                   id
                                   name
                                   owner {
                                       id
                                   }
                               }
                           }";

                       HttpResponseMessage response = await PerformGraphQlQuery(testServer, QueryProjects, null);
                       string content = await response.Content.ReadAsStringAsync();
                       Assert.IsTrue(response.IsSuccessStatusCode, content);
                       JsonDocument result = JsonDocument.Parse(content);
                       Assert.IsNotNull(result.RootElement.GetProperty("errors"), content);

                       SeedOptions seedOptions = scope.ServiceProvider.GetRequiredService<IOptions<SeedOptions>>().Value;

                       using (var httpClient = testServer.CreateClient())
                       {
                           // Login as admin
                           Dictionary<string, string> loginContent = new Dictionary<string, string>()
                           {
                               { "Email", seedOptions.AdminEmail },
                               { "Password", seedOptions.AdminPassword }
                           };
                           using (var formContent = new FormUrlEncodedContent(loginContent))
                           {
                               HttpResponseMessage authResult = await httpClient.PostAsync(new Uri("/account/login", UriKind.Relative), formContent);
                               string authResultContent = await authResult.Content.ReadAsStringAsync();
                               string cookie = authResult.Headers.Single(h => h.Key == "Set-Cookie").Value.Single().Split(";").First();
                               httpClient.DefaultRequestHeaders.Add("Cookie", cookie);
                               Assert.IsTrue(authResult.IsSuccessStatusCode, authResultContent);
                           }

                           // Retry call as admin
                           response = await PerformGraphQlQuery(httpClient, QueryProjects, null);
                           content = await response.Content.ReadAsStringAsync();
                           Assert.IsTrue(response.IsSuccessStatusCode, content);
                           result = JsonDocument.Parse(content);
                           Assert.ThrowsException<KeyNotFoundException>(() => result.RootElement.GetProperty("errors"), content);
                       }
                   });

        private static async Task<HttpResponseMessage> PerformGraphQlQuery(TestServer testServer, string query, dynamic variables)
        {
            using (var httpClient = testServer.CreateClient())
            {
                return await PerformGraphQlQuery(httpClient, query, variables);
            }
        }

        private static async Task<HttpResponseMessage> PerformGraphQlQuery(HttpClient httpClient, string query, dynamic variables)
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
