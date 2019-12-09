using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using CollAction.Data;

namespace CollAction.Tests.Integration
{
    public abstract class IntegrationTestBase
    {
        public async Task WithServiceProvider(Func<IServiceScope, Task> executeTests)
        {
            using (IWebHost host = GetHost(ConfigureReplacementServicesProvider).Build())
            using (IServiceScope scope = host.Services.CreateScope())
            {
                await ApplicationDbContext.InitializeDatabase(scope);
                await executeTests(scope);
            }
        }

        public Task WithTestServer(Func<IServiceScope, TestServer, Task> executeTests)
            => WithServiceProvider(
                async scope =>
                {
                    using (var testServer = new TestServer(GetHost(ConfigureReplacementServicesTestServer)))
                    {
                        await executeTests(scope, testServer);
                    }
                });

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
