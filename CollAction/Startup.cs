using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    public sealed class Startup
    {
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.environment = environment;
            publicAddress = this.configuration["PublicAddress"];
            connectionString = $"Host={this.configuration["DbHost"]};Username={this.configuration["DbUser"]};Password={this.configuration["DbPassword"]};Database={this.configuration["Db"]};Port={this.configuration["DbPort"]}";
        }

        private readonly string corsPolicy = "FrontendCors";
        private readonly string publicAddress;
        private readonly string connectionString;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;

        public void ConfigureServices(IServiceCollection services)
        {
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

            if (configuration["Authentication:Facebook:AppId"] != null)
            {
                authenticationBuilder = authenticationBuilder.AddFacebook(options =>
                {
                    options.AppId = configuration["Authentication:Facebook:AppId"];
                    options.AppSecret = configuration["Authentication:Facebook:AppSecret"];
                    options.Scope.Add("email");
                });
            }

            if (configuration["Authentication:Google:ClientId"] != null)
            {
                authenticationBuilder = authenticationBuilder.AddGoogle(options =>
                {
                    options.ClientId = configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
                    options.Scope.Add("email");
                });
            }

            if (configuration["Authentication:Twitter:ConsumerKey"] != null)
            {
                authenticationBuilder = authenticationBuilder.AddTwitter(options =>
                {
                    options.ConsumerKey = configuration["Authentication:Twitter:ConsumerKey"];
                    options.ConsumerSecret = configuration["Authentication:Twitter:ConsumerSecret"];
                });
            }

            services.AddApplicationInsightsTelemetry(configuration);

            services.AddLogging(loggingBuilder =>
            {
                LoggerConfiguration configuration = new LoggerConfiguration()
                       .WriteTo.Console(LogEventLevel.Information)
                       .WriteTo.ApplicationInsights(TelemetryConfiguration.Active, TelemetryConverter.Traces);

                string slackHook = this.configuration["SlackHook"];
                if (slackHook != null)
                {
                    configuration.WriteTo.Slack(slackHook, restrictedToMinimumLevel: LogEventLevel.Error);
                }

                Log.Logger = configuration.CreateLogger();
                loggingBuilder.AddSerilog(Log.Logger);
            });

            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                    .AddNewtonsoftJson();

            services.AddHangfire(
                config => config.UseSerilogLogProvider()
                                .UsePostgreSqlStorage(connectionString));

            services.AddDataProtection()
                    .Services
                    .Configure<KeyManagementOptions>(options => options.XmlRepository = new DataProtectionRepository(new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(connectionString).Options));

            services.AddCors(c =>
            {
                c.AddPolicy(
                    corsPolicy,
                    builder =>
                        builder.AllowAnyMethod()
                               .SetIsOriginAllowedToAllowWildcardSubdomains()
                               .AllowAnyHeader()
                               .AllowCredentials()
                               .WithOrigins(publicAddress.Split(";")));
            });

            services.AddUrlHelper();
            services.AddHealthChecks()
                    .AddDbContextCheck<ApplicationDbContext>();

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
            services.Configure<StripeSignatures>(configuration);
            services.Configure<SiteOptions>(configuration);
            services.Configure<AuthMessageSenderOptions>(configuration);
            services.Configure<ImageServiceOptions>(configuration);
            services.Configure<ImageProcessingOptions>(configuration);
            services.Configure<NewsletterServiceOptions>(configuration);
            services.Configure<ProjectEmailOptions>(configuration);
            services.Configure<SeedOptions>(configuration);
            services.Configure<MailChimpOptions>(options =>
            {
                options.ApiKey = configuration["MailChimpKey"];
            });
            services.Configure<RequestOptions>(options =>
            {
                options.ApiKey = configuration["StripeSecretApiKey"];
            });
            services.Configure<StripePublicOptions>(options =>
            {
                options.StripePublicKey = configuration["StripePublicApiKey"];
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

        public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime)
        {
            app.UseRouting();
            app.UseCors(corsPolicy);

            if (environment.IsProduction())
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
                app.UseExceptionHandler("/error");

                applicationLifetime.ApplicationStopping.Register(() => Log.CloseAndFlush());
            }
            else if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseGraphiQl("/graphiql", "/graphql");
            }

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseHangfireServer(new BackgroundJobServerOptions() { WorkerCount = 1 });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHangfireDashboard(new DashboardOptions()
                {
                    AppPath = publicAddress,
                    DashboardTitle = "CollAction Jobs",
                    Authorization = new[] { new HangfireAdminAuthorizationFilter() }
                });
                endpoints.MapControllerRoute("Default", "{controller}/{action}");
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
