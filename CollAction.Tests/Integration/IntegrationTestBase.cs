using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using CollAction.Data;
using System.Net.Http;
using CollAction.Services;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CollAction.Tests.Integration
{
    public abstract class IntegrationTestBase
    {
        public async Task WithServiceProvider(Func<IServiceScope, Task> executeTests)
        {
            using IWebHost host = GetHost(ConfigureReplacementServicesProvider).Build();
            using IServiceScope scope = host.Services.CreateScope();
            await ApplicationDbContext.InitializeDatabase(scope);
            await executeTests(scope);
        }

        public Task WithTestServer(Func<IServiceScope, TestServer, Task> executeTests)
            => WithServiceProvider(
                async scope =>
                {
                    using var testServer = new TestServer(GetHost(ConfigureReplacementServicesTestServer));
                    await executeTests(scope, testServer);
                });

        protected static async Task<string> GetAuthCookie(HttpClient httpClient, SeedOptions seedOptions)
        {
            // Login as admin
            Dictionary<string, string> loginContent = new Dictionary<string, string>()
            {
                { "Email", seedOptions.AdminEmail },
                { "Password", seedOptions.AdminPassword }
            };
            using var formContent = new FormUrlEncodedContent(loginContent);
            HttpResponseMessage authResult = await httpClient.PostAsync(new Uri("/account/login", UriKind.Relative), formContent);
            string authResultContent = await authResult.Content.ReadAsStringAsync();
            Assert.IsTrue(authResult.IsSuccessStatusCode, authResultContent);
            string cookie = authResult.Headers.Single(h => h.Key == "Set-Cookie").Value.Single().Split(";").First();
            Assert.IsTrue(authResult.IsSuccessStatusCode, authResultContent);
            return cookie;
        }

        protected virtual void ConfigureReplacementServicesProvider(IServiceCollection collection)
        {
        }

        protected virtual void ConfigureReplacementServicesTestServer(IServiceCollection collection)
        {
        }

        private static IWebHostBuilder GetHost(Action<IServiceCollection> configureReplacements)
            => WebHost.CreateDefaultBuilder()
                      .UseEnvironment("Development")
                      .ConfigureTestServices(configureReplacements)
                      .UseStartup<Startup>();
    }
}
