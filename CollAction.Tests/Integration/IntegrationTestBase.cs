using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using CollAction.Data;

namespace CollAction.Tests.Integration
{
    public abstract class IntegrationTestBase
    {
        public async Task WithServiceProvider(Action<IServiceCollection> configureReplacements, Func<IServiceScope, Task> executeTests)
        {
            IWebHostBuilder hostBuilder =
                WebHost.CreateDefaultBuilder()
                       .ConfigureAppConfiguration(builder =>
                       {
                           builder.AddUserSecrets<Startup>();
                       })
                       .ConfigureTestServices(configureReplacements)
                       .UseStartup<Startup>();
                       

            using (IWebHost host = hostBuilder.Build())
            using (IServiceScope scope = host.Services.CreateScope())
            {
                await ApplicationDbContext.InitializeDatabase(scope);
                await executeTests(scope);
            }
        }
    }
}
