using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Slack;

namespace CollAction
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .ConfigureAppConfiguration(builder =>
                   {
                       builder.AddUserSecrets<Startup>();
                   })
                   .ConfigureLogging((context, builder) =>
                   {
                       LoggerConfiguration configuration = new LoggerConfiguration()
                           .WriteTo.RollingFile("log-{Date}.txt", LogEventLevel.Information)
                           .WriteTo.Console(LogEventLevel.Information);

                       string instrumentationKey = context.Configuration["ApplicationInsights:InstrumentationKey"];
                       if (instrumentationKey != null)
                           configuration.WriteTo.ApplicationInsightsTraces(instrumentationKey);

                       string slackHook = context.Configuration["SlackHook"];
                       if (slackHook != null)
                           configuration.WriteTo.Slack(slackHook, restrictedToMinimumLevel: LogEventLevel.Error);

                       if (context.HostingEnvironment.IsDevelopment())
                           configuration.WriteTo.Trace();

                       Log.Logger = configuration.CreateLogger();
                       builder.AddSerilog();
                   })
                   .UseStartup<Startup>()
                   .Build();
    }
}
