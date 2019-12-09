using CollAction.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace CollAction
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IWebHost host =
                WebHost.CreateDefaultBuilder(args)
                       .UseStartup<Startup>()
                       .Build();

            using (IServiceScope scope = host.Services.CreateScope())
            {
                await ApplicationDbContext.InitializeDatabase(scope);
            }

            await host.RunAsync();
        }
    }
}
