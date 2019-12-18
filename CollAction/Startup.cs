using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CollAction.Data;
using CollAction.Models;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Hangfire;
using Hangfire.PostgreSql;
using CollAction.Helpers;
using Hangfire.Dashboard;
using CollAction.Services.Email;
using CollAction.Services.Projects;
using CollAction.Services.Newsletter;
using CollAction.Services.DataProtection;
using CollAction.Services.Image;
using Stripe;
using CollAction.Services.Donation;
using CollAction.Services;
using Serilog.Events;
using Serilog.Sinks.Slack;
using Microsoft.ApplicationInsights.Extensibility;
using CollAction.Services.ViewRender;
using MailChimp.Net;
using MailChimp.Net.Interfaces;
using GraphiQl;
using Microsoft.AspNetCore.Mvc;
using CollAction.GraphQl;
using CollAction.Services.User;
using AspNetCore.IServiceCollection.AddIUrlHelper;
using CollAction.Services.HtmlValidator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace CollAction
{
    public class Startup
    {
        private readonly string corsPolicy = "FrontendCors";

        public Startup(IWebHostEnvironment env, IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            Environment = env;
            Logger = logger;
        }

        public ILogger<Startup> Logger { get; }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = $"Host={Configuration["DbHost"]};Username={Configuration["DbUser"]};Password={Configuration["DbPassword"]};Database={Configuration["Db"]};Port={Configuration["DbPort"]}";

            services.AddGraphQl();
            services.AddGraphQlAuth();

            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            // Identity/Auth cookie, allow it to be used from different sites, use CORS to secure it
            services.ConfigureApplicationCookie(o =>
            {
                o.Cookie.HttpOnly = false;
                o.Cookie.SameSite = SameSiteMode.None;
            });

            var authenticationBuilder = services.AddAuthentication();

            if (Configuration["Authentication:Facebook:AppId"] != null)
            {
                authenticationBuilder = authenticationBuilder.AddFacebook(options =>
                {
                    options.AppId = Configuration["Authentication:Facebook:AppId"];
                    options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                    options.Scope.Add("email");
                });
            }

            if (Configuration["Authentication:Google:ClientId"] != null)
            {
                authenticationBuilder = authenticationBuilder.AddGoogle(options =>
                {
                    options.ClientId = Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                    options.Scope.Add("email");
                });
            }

            if (Configuration["Authentication:Twitter:ConsumerKey"] != null)
            {
                authenticationBuilder = authenticationBuilder.AddTwitter(options =>
                {
                    options.ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"];
                    options.ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"];
                });
            }

            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddLogging(loggingBuilder =>
            {
                LoggerConfiguration configuration = new LoggerConfiguration()
                       .WriteTo.Console(LogEventLevel.Information)
                       .WriteTo.ApplicationInsights(TelemetryConfiguration.Active, TelemetryConverter.Traces);

                string slackHook = Configuration["SlackHook"];
                if (slackHook != null)
                {
                    configuration.WriteTo.Slack(slackHook, restrictedToMinimumLevel: LogEventLevel.Error);
                }

                Log.Logger = configuration.CreateLogger();
                loggingBuilder.AddSerilog(Log.Logger);
            });

            services.AddControllers()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                    .AddNewtonsoftJson();

            services.AddHangfire(config => config.UsePostgreSqlStorage(connectionString));

            services.AddDataProtection()
                    .Services
                    .Configure<KeyManagementOptions>(options => options.XmlRepository = new DataProtectionRepository(new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(connectionString).Options));

            services.AddCors(c =>
            {
                string publicAddress = Configuration["PublicAddress"];
                c.AddPolicy(
                    corsPolicy,
                    builder =>
                        builder.AllowAnyMethod()
                               .AllowAnyHeader()
                               .AllowCredentials()
                               .SetIsOriginAllowed(o => o == publicAddress)
                               .WithOrigins(publicAddress));
            });

            services.AddUrlHelper();
            services.AddHealthChecks();

            // Add application services.
            services.AddScoped<IImageService, AmazonS3ImageService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<INewsletterService, NewsletterService>();
            services.AddTransient<IDonationService, DonationService>();
            services.AddTransient<IViewRenderService, ViewRenderService>();
            services.AddTransient<IMailChimpManager, MailChimpManager>();
            services.AddTransient<IHtmlInputValidator, HtmlInputValidator>();

            // Configure application options
            services.Configure<StripeSignatures>(Configuration);
            services.Configure<SiteOptions>(Configuration);
            services.Configure<DisqusOptions>(Configuration);
            services.Configure<AuthMessageSenderOptions>(Configuration);
            services.Configure<ImageServiceOptions>(Configuration);
            services.Configure<ImageProcessingOptions>(Configuration);
            services.Configure<NewsletterServiceOptions>(Configuration);
            services.Configure<ProjectEmailOptions>(Configuration);
            services.Configure<SeedOptions>(Configuration);
            services.Configure<MailChimpOptions>(options =>
            {
                options.ApiKey = Configuration["MailChimpKey"];
            });
            services.Configure<RequestOptions>(options =>
            {
                options.ApiKey = Configuration["StripeSecretApiKey"];
            });
            services.Configure<StripePublicOptions>(options =>
            {
                options.StripePublicKey = Configuration["StripePublicApiKey"];
            });
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            app.UseRouting();
            app.UseCors(corsPolicy);

            if (env.IsProduction())
            {
                // Ensure our middleware handles proxied https, see https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Forwarded-Proto
                var forwardedHeaderOptions = new ForwardedHeadersOptions()
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedHost,
                    ForwardLimit = 3
                };
                forwardedHeaderOptions.KnownProxies.Clear();
                forwardedHeaderOptions.KnownNetworks.Clear();
                app.UseForwardedHeaders(forwardedHeaderOptions);
                app.UseRewriter(new RewriteOptions().AddRedirectToHttpsPermanent());
                app.UseHsts();

                applicationLifetime.ApplicationStopping.Register(() => Log.CloseAndFlush());
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseGraphiQl("/graphiql", "/graphql");
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseAuthentication();

            app.UseHangfireServer(new BackgroundJobServerOptions() { WorkerCount = 1 });

            app.UseHangfireDashboard(
                options: new DashboardOptions()
                {
                    Authorization = new IDashboardAuthorizationFilter[]
                    {
                        new HangfireAdminAuthorizationFilter()
                    }
                });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("Default", "{controller}/{action}");
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
