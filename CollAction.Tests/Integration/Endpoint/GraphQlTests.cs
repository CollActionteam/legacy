using CollAction.Data;
using CollAction.Models;
using CollAction.Services;
using CollAction.Services.Crowdactions;
using CollAction.Services.Crowdactions.Models;
using CollAction.Services.Email;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CollAction.Tests.Integration.Endpoint
{
    [Trait("Category", "Integration")]
    public sealed class GraphQlTests : IntegrationTestBase
    {
        private readonly ICrowdactionService crowdactionService;
        private readonly ApplicationDbContext context;
        private readonly SeedOptions seedOptions;
        private readonly IConfiguration configuration;

        public GraphQlTests()
        {
            crowdactionService = Scope.ServiceProvider.GetRequiredService<ICrowdactionService>();
            context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            seedOptions = Scope.ServiceProvider.GetRequiredService<IOptions<SeedOptions>>().Value;
            configuration = Scope.ServiceProvider.GetRequiredService<IConfiguration>();
        }

        [Fact]
        public async Task TestCrowdactionList()
        {
            var newCrowdaction = new NewCrowdactionInternal("test" + Guid.NewGuid(), 100, "test", "test", "test", null, DateTime.UtcNow, DateTime.UtcNow.AddDays(1), null, null, null, null, new[] { Category.Community }, Array.Empty<string>(), CrowdactionDisplayPriority.Bottom, CrowdactionStatus.Running, 0, null);
            Crowdaction createdCrowdaction = await crowdactionService.CreateCrowdactionInternal(newCrowdaction, CancellationToken.None);
            Assert.NotNull(createdCrowdaction);
            
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

            HttpResponseMessage response = await PerformGraphQlQuery(QueryCrowdactions, null);
            string content = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, content);
            JsonDocument result = JsonDocument.Parse(content);
            Assert.Throws<KeyNotFoundException>(() => result.RootElement.GetProperty("errors"));
            JsonElement.ArrayEnumerator crowdactions = result.RootElement.GetProperty("data").GetProperty("crowdactions").EnumerateArray();
            Assert.True(crowdactions.Any(), content);

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
            response = await PerformGraphQlQuery(QueryCrowdaction, variables);
            content = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, content);
            result = JsonDocument.Parse(content);
            Assert.Throws<KeyNotFoundException>(() => result.RootElement.GetProperty("errors"));
            JsonElement crowdaction = result.RootElement.GetProperty("data").GetProperty("crowdaction");
            Assert.Equal(crowdactionId.ToString(CultureInfo.InvariantCulture), crowdaction.GetProperty("id").GetString());
        }

        [Fact]
        public async Task TestAuthorization()
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

            HttpResponseMessage response = await PerformGraphQlQuery(QueryCrowdactions, null);
            string content = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, content);
            JsonDocument result = JsonDocument.Parse(content);
            Assert.Equal(1, result.RootElement.GetProperty("errors").GetArrayLength());

            using var httpClient = TestServer.CreateClient();
            // Retry call as admin
            httpClient.DefaultRequestHeaders.Add("Cookie", await GetAuthCookie(httpClient, seedOptions));
            response = await PerformGraphQlQuery(httpClient, QueryCrowdactions, null);
            content = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, content);
            result = JsonDocument.Parse(content);
            Assert.Throws<KeyNotFoundException>(() => result.RootElement.GetProperty("errors"));

            // Retry call as jwt admin
            var user = await context.Users.FirstAsync();
            var authSection = configuration.GetSection("Authentication");
            var jwtSection = authSection.GetSection("Jwt");
            using X509Certificate2 certificate = new(Convert.FromBase64String(jwtSection["Certificate"]), jwtSection["CertificatePassword"]);
            var securityToken = new JwtSecurityToken(
                "CollAction",
                "CollAction",
                new Claim[]
                {
                    new Claim("sub", user.Id),
                    new Claim("role", "admin"),
                    new Claim("name", user.FullName),
                },
                expires: DateTime.Now.AddDays(1),
                signingCredentials: new SigningCredentials(new X509SecurityKey(certificate), "RS256"));
            string httpToken = new JwtSecurityTokenHandler().WriteToken(securityToken);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpToken);
            response = await PerformGraphQlQuery(httpClient, QueryCrowdactions, null);
            content = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, content);
            result = JsonDocument.Parse(content);
            Assert.Throws<KeyNotFoundException>(() => result.RootElement.GetProperty("errors"));
        }

        [Fact]
        public async Task TestCreateCrowdaction()
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
                                descriptionVideoLink: ""https://www.youtube-nocookie.com/embed/a1"",
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

            using var httpClient = TestServer.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Cookie", await GetAuthCookie(httpClient, seedOptions));
            HttpResponseMessage response = await PerformGraphQlQuery(httpClient, createCrowdaction, null);
            string content = await response.Content.ReadAsStringAsync();
            JsonDocument result = JsonDocument.Parse(content);
            Assert.True(response.IsSuccessStatusCode, content);
            Assert.Throws<KeyNotFoundException>(() => result.RootElement.GetProperty("errors"));
            Assert.True(result.RootElement.GetProperty("data").GetProperty("crowdaction").GetProperty("createCrowdaction").GetProperty("succeeded").GetBoolean());
        }

        protected override void ConfigureReplacementServicesProvider(IServiceCollection collection)
        {
            collection.AddTransient(s => new Mock<IEmailSender>().Object);
        }
    }
}
