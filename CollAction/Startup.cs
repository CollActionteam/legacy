using AspNetCore.IServiceCollection.AddIUrlHelper;
using CollAction.Data;
using CollAction.GraphQl;
using CollAction.Helpers;
using CollAction.Models;
using CollAction.Services;
using CollAction.Services.Crowdactions;
using CollAction.Services.Donation;
using CollAction.Services.Email;
using CollAction.Services.HtmlValidator;
using CollAction.Services.Image;
using CollAction.Services.Initialization;
using CollAction.Services.Newsletter;
using CollAction.Services.Statistics;
using CollAction.Services.User;
using CollAction.Services.ViewRender;
using GraphiQl;
using Hangfire;
using Hangfire.PostgreSql;
using MailChimp.Net;
using MailChimp.Net.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Stripe;
using System;

namespace CollAction
{
    public sealed class Startup
    {
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.environment = environment;
        }

        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGraphQl();
            services.AddGraphQlAuth();

            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.Get<DbOptions>().ConnectionString);
                if (environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            // Identity/Auth cookie, allow it to be used from different sites, use CORS to secure it
            services.ConfigureApplicationCookie(o =>
            {
                o.Cookie.HttpOnly = false;
                o.Cookie.SameSite = environment.IsProduction() ? SameSiteMode.Lax : SameSiteMode.None;
                o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            IConfigurationSection authSection = configuration.GetSection("Authentication");
            services.AddAuthentication()
                    .AddFacebook(options =>
                    {
                        authSection.GetSection("Facebook").Bind(options);
                        options.Scope.Add("email");
                    })
                    .AddTwitter(options =>
                    {
                        authSection.GetSection("Twitter").Bind(options);
                        options.RetrieveUserDetails = true;
                    })
                    .AddGoogle(options =>
                    {
                        authSection.GetSection("Google").Bind(options);
                        options.Scope.Add("email");
                    });

            services.AddApplicationInsightsTelemetry(configuration);

            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                    .AddNewtonsoftJson();

            services.AddHangfire(
                config => config.UseSerilogLogProvider()
                                .UsePostgreSqlStorage(configuration.Get<DbOptions>().ConnectionString));

            services.AddDataProtection()
                    .PersistKeysToDbContext<ApplicationDbContext>();

            services.AddCors(c =>
            {
                if (environment.IsProduction())
                {
                    c.AddDefaultPolicy(
                        builder =>
                            builder.AllowAnyMethod()
                                   .AllowAnyHeader()
                                   .AllowCredentials()
                                   .SetPreflightMaxAge(TimeSpan.FromMinutes(10))
                                   .WithOrigins(configuration.Get<SiteOptions>().PublicAddress));
                }
                else
                {
                    c.AddDefaultPolicy(
                        builder =>
                            builder.AllowAnyMethod()
                                   .AllowAnyHeader()
                                   .AllowCredentials()
                                   .SetPreflightMaxAge(TimeSpan.FromMinutes(10))
                                   .SetIsOriginAllowed(_ => true));
                }
            });

            services.AddUrlHelper();
            services.AddHealthChecks()
                    .AddDbContextCheck<ApplicationDbContext>();

            // Add application services.
            services.AddScoped<IImageService, AmazonS3ImageService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICrowdactionService, CrowdactionService>();
            services.AddScoped<IStatisticsService, StatisticsService>();
            services.AddScoped<IInitializationService, InitializationService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<INewsletterService, NewsletterService>();
            services.AddTransient<IDonationService, DonationService>();
            services.AddTransient<IViewRenderService, ViewRenderService>();
            services.AddTransient<IMailChimpManager, MailChimpManager>();
            services.AddTransient<IHtmlInputValidator, HtmlInputValidator>();

            // Configure application options
            services.AddOptions<StripeSignatures>().Bind(configuration).ValidateDataAnnotations();
            services.AddOptions<SiteOptions>().Bind(configuration).ValidateDataAnnotations();
            services.AddOptions<AuthMessageSenderOptions>().Bind(configuration).ValidateDataAnnotations();
            services.AddOptions<ImageServiceOptions>().Bind(configuration).ValidateDataAnnotations();
            services.AddOptions<NewsletterServiceOptions>().Bind(configuration).ValidateDataAnnotations();
            services.AddOptions<SeedOptions>().Bind(configuration).ValidateDataAnnotations();
            services.AddOptions<AnalyticsOptions>().Bind(configuration).ValidateDataAnnotations();
            services.AddOptions<DbOptions>().Bind(configuration).ValidateDataAnnotations();
            services.AddOptions<DataProtectionTokenProviderOptions>()
                    .Configure(o =>
                    {
                        o.TokenLifespan = TimeSpan.FromDays(7); // 7 Days to finish setting up your account
                    });
            services.AddOptions<MailChimpOptions>().Configure(options =>
            {
                options.ApiKey = configuration.Get<NewsletterServiceOptions>().MailChimpKey;
            }).ValidateDataAnnotations();
            services.AddOptions<RequestOptions>().Configure(options =>
            {
                options.ApiKey = configuration["StripeSecretApiKey"];
            }).ValidateDataAnnotations();
            services.AddOptions<StripePublicOptions>().Configure(options =>
            {
                options.StripePublicKey = configuration["StripePublicApiKey"];
            }).ValidateDataAnnotations();
            services.AddOptions<IdentityOptions>().Configure(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
            }).ValidateDataAnnotations();
        }

        public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopping.Register(Log.CloseAndFlush);
            app.UseRouting();
            app.UseCors();
            app.UseSerilogRequestLogging();

            if (environment.IsProduction() || environment.IsStaging())
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
            }

            if (environment.IsProduction())
            {
                app.UseHsts();
                app.UseExceptionHandler("/error");
            }

            if (environment.IsDevelopment() || environment.IsStaging())
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
                    AppPath = configuration.Get<SiteOptions>().PublicAddress,
                    DashboardTitle = "CollAction Jobs",
                    Authorization = new[] { new HangfireAdminAuthorizationFilter() }
                });
                endpoints.MapControllerRoute("Default", "{controller}/{action}");
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
