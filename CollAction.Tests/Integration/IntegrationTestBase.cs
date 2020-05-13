using CollAction.Services;
using CollAction.Services.Initialization;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CollAction.Tests.Integration
{
    public abstract class IntegrationTestBase : IAsyncLifetime, IDisposable
    {
        protected IntegrationTestBase(bool needsServer = true)
        {
            lock (constructorMutex)
            {
                Host = CreateTestHost().Build();
                Scope = Host.Services.CreateScope();
                if (needsServer)
                {
                    TestServer = new TestServer(CreateTestHost());
                }
            }
        }

        private static readonly object constructorMutex = new object();
        protected IWebHost Host { get; }
        protected TestServer TestServer { get; }
        protected IServiceScope Scope { get; }

        public void Dispose()
        {
            Host.Dispose();
            Scope.Dispose();
            if (TestServer != null)
            {
                TestServer.Dispose();
            }
        }

        public Task DisposeAsync()
            => Task.CompletedTask;

        public Task InitializeAsync()
            => Scope.ServiceProvider.GetRequiredService<IInitializationService>().InitializeDatabase();

        protected static async Task<string> GetAuthCookie(HttpClient httpClient, SeedOptions seedOptions)
        {
            // Login as admin
            Dictionary<string, string> loginContent = new Dictionary<string, string>()
            {
                { "Email", seedOptions.AdminEmail },
                { "Password", seedOptions.AdminPassword }
            };
            using var formContent = new FormUrlEncodedContent(loginContent);
            HttpResponseMessage authResult = await httpClient.PostAsync(new Uri("/account/login", UriKind.Relative), formContent).ConfigureAwait(false);
            string authResultContent = await authResult.Content.ReadAsStringAsync().ConfigureAwait(false);
            Assert.True(authResult.IsSuccessStatusCode, authResultContent);
            string cookie = authResult.Headers.Single(h => h.Key == "Set-Cookie").Value.Single().Split(";").First();
            Assert.True(authResult.IsSuccessStatusCode, authResultContent);
            return cookie;
        }

        protected virtual void ConfigureReplacementServicesProvider(IServiceCollection collection)
        {
        }

        protected IWebHostBuilder CreateTestHost()
            => WebHost.CreateDefaultBuilder()
                      .UseEnvironment("Development")
                      .ConfigureTestServices(ConfigureReplacementServicesProvider)
                      .UseStartup<Startup>()
                      .UseSerilog((hostingContext, loggerConfiguration) =>
                      {
                          LogEventLevel level = hostingContext.Configuration.GetValue<LogEventLevel>("LogLevel");
                          loggerConfiguration.MinimumLevel.Is(level)
                                             .MinimumLevel.Override("Microsoft", level)
                                             .MinimumLevel.Override("System", level)
                                             .MinimumLevel.Override("Microsoft.AspNetCore", level)
                                             .WriteTo.Console(level)
                                             .Enrich.FromLogContext();
                      });
    }
}
