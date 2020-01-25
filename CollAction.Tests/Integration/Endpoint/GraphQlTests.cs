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
        public Task TestProjectList()
            => WithTestServer(
                   async (scope, testServer) =>
                   {
                       const string QueryProjects = @"
                           query {
                               projects {
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

                       HttpResponseMessage response = await PerformGraphQlQuery(testServer, QueryProjects, null).ConfigureAwait(false);
                       string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                       Assert.IsTrue(response.IsSuccessStatusCode, content);
                       JsonDocument result = JsonDocument.Parse(content);
                       Assert.ThrowsException<KeyNotFoundException>(() => result.RootElement.GetProperty("errors"), content);
                       JsonElement.ArrayEnumerator projects = result.RootElement.GetProperty("data").GetProperty("projects").EnumerateArray();
                       Assert.IsTrue(projects.Any(), content);

                       int projectId = projects.First().GetProperty("id").GetInt32();
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
                       content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                       Assert.IsTrue(response.IsSuccessStatusCode, content);
                       result = JsonDocument.Parse(content);
                       Assert.ThrowsException<KeyNotFoundException>(() => result.RootElement.GetProperty("errors"), content);
                       JsonElement project = result.RootElement.GetProperty("data").GetProperty("project");
                       Assert.AreEqual(projectId, project.GetProperty("id").GetInt32());
                   });

        [TestMethod]
        public Task TestProjectFilter()
            => WithTestServer(
                   async (scope, testServer) =>
                   {
                       const string QueryProjects = @"
                           query {
                               projects(status: COMING_SOON, category: ENVIRONMENT) {
                                   id
                                   categories {
                                     category
                                   }
                                   start
                                   end
                                   status
                               }
                           }";

                       HttpResponseMessage response = await PerformGraphQlQuery(testServer, QueryProjects, null).ConfigureAwait(false);
                       string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                       Assert.IsTrue(response.IsSuccessStatusCode, content);
                       JsonDocument result = JsonDocument.Parse(content);
                       Assert.ThrowsException<KeyNotFoundException>(() => result.RootElement.GetProperty("errors"), content);
                       JsonElement.ArrayEnumerator projects = result.RootElement.GetProperty("data").GetProperty("projects").EnumerateArray();
                       Assert.IsTrue(projects.Any(), content);
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
                                   isSuccessfull
                                   isFailed
                                   canSendProjectEmail
                               }
                           }";

                       HttpResponseMessage response = await PerformGraphQlQuery(testServer, QueryProjects, null).ConfigureAwait(false);
                       string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                       Assert.IsTrue(response.IsSuccessStatusCode, content);
                       JsonDocument result = JsonDocument.Parse(content);
                       Assert.IsNotNull(result.RootElement.GetProperty("errors"), content);

                       SeedOptions seedOptions = scope.ServiceProvider.GetRequiredService<IOptions<SeedOptions>>().Value;
                       using var httpClient = testServer.CreateClient();
                       // Retry call as admin
                       httpClient.DefaultRequestHeaders.Add("Cookie", await GetAuthCookie(httpClient, seedOptions).ConfigureAwait(false));
                       response = await PerformGraphQlQuery(httpClient, QueryProjects, null).ConfigureAwait(false);
                       content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                       Assert.IsTrue(response.IsSuccessStatusCode, content);
                       result = JsonDocument.Parse(content);
                       Assert.ThrowsException<KeyNotFoundException>(() => result.RootElement.GetProperty("errors"), content);
                   });

        [TestMethod]
        public Task TestCreateProject()
            => WithTestServer(
                   async (scope, testServer) =>
                   {
                       string createProject = $@"
                           mutation {{
                               project {{
                                   createProject(project:
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
                                           descriptionVideoLink: ""https://www.youtube.com/watch?v=a1"",
                                           tags:[""b"", ""a""]
                                       }}) {{
                                           succeeded
                                           project {{
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
                       HttpResponseMessage response = await PerformGraphQlQuery(httpClient, createProject, null).ConfigureAwait(false);
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
