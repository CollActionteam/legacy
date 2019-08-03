using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CollAction.Tests.Integration
{
    public abstract class IntegrationTestBase
    {
        public async Task WithServiceProvider(Action<ServiceCollection> configureReplacements, Func<IServiceScope, Task> executeTests)
        {
            HostingEnvironment env = new HostingEnvironment()
            {
                ContentRootPath = Directory.GetCurrentDirectory(),
                EnvironmentName = "Development"
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>()
                .Build();

            ILogger<Startup> logger = new LoggerFactory().CreateLogger<Startup>();

            Startup startup = new Startup(env, configuration, logger);

            ServiceCollection sc = new ServiceCollection();
            startup.ConfigureServices(sc);
            configureReplacements?.Invoke(sc);
            using (ServiceProvider serviceProvider = sc.BuildServiceProvider())
            {
                startup.InitializeDatabase(serviceProvider);
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    await executeTests(scope);
                }
            }
        }
    }
}
