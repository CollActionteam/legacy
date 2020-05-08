using CollAction.Services.Image;
using CollAction.Services.Initialization;
using CollAction.Services.Crowdactions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Slack;
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
                await scope.ServiceProvider.GetRequiredService<IInitializationService>().InitializeDatabase().ConfigureAwait(false);
                scope.ServiceProvider.GetRequiredService<IImageService>().InitializeDanglingImageJob();
                scope.ServiceProvider.GetRequiredService<ICrowdactionService>().InitializeRefreshParticipantCountJob();
            }

            await host.RunAsync().ConfigureAwait(false);
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args)
            => WebHost.CreateDefaultBuilder(args)
                      .UseStartup<Startup>()
                      .UseSerilog((hostingContext, loggerConfiguration) =>
                      {
                          LogEventLevel level = hostingContext.Configuration.GetValue<LogEventLevel>("LogLevel");
                          loggerConfiguration.MinimumLevel.Is(level)
                                             .MinimumLevel.Override("Microsoft", level)
                                             .MinimumLevel.Override("System", level)
                                             .MinimumLevel.Override("Microsoft.AspNetCore", level)
                                             .WriteTo.Console(level)
                                             .WriteTo.ApplicationInsights(TelemetryConfiguration.Active, TelemetryConverter.Traces)
                                             .Enrich.FromLogContext();

                          string? slackHook = hostingContext.Configuration["SlackHook"];
                          if (slackHook != null)
                          {
                              loggerConfiguration.WriteTo.Slack(slackHook, restrictedToMinimumLevel: LogEventLevel.Error);
                          }
                      });
    }
}
