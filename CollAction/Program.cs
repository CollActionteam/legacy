﻿using CollAction.Data;
using CollAction.Services.Image;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace CollAction
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            IWebHost host = CreateHostBuilder(args).Build();

            using (IServiceScope scope = host.Services.CreateScope())
            {
                await ApplicationDbContext.InitializeDatabase(scope).ConfigureAwait(false);
                scope.ServiceProvider.GetRequiredService<IImageService>().InitializeDanglingImageJob();
            }

            await host.RunAsync().ConfigureAwait(false);
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args)
            => WebHost.CreateDefaultBuilder(args)
                      .UseStartup<Startup>();
    }
}
