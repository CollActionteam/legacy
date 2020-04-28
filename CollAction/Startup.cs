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
using Hangfire;
using Hangfire.PostgreSql;
using CollAction.Helpers;
using CollAction.Services.Email;
using CollAction.Services.Projects;
using CollAction.Services.Newsletter;
using CollAction.Services.Image;
using Stripe;
using CollAction.Services.Donation;
using CollAction.Services;
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
using Microsoft.AspNetCore.DataProtection;
using CollAction.Services.Statistics;

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
            string connectionString = $"Host={configuration["DbHost"]};Username={configuration["DbUser"]};Password={configuration["DbPassword"]};Database={configuration["Db"]};Port={configuration["DbPort"]}";
            services.AddGraphQl();
            services.AddGraphQlAuth();

            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
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
                o.Cookie.IsEssential = true;
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

            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                    .AddNewtonsoftJson();

            services.AddHangfire(
                config => config.UseSerilogLogProvider()
                                .UsePostgreSqlStorage(connectionString));

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
                                   .WithOrigins(configuration.Get<SiteOptions>().PublicAddress));
                }
                else
                {
                    c.AddDefaultPolicy(
                        builder =>
                            builder.AllowAnyMethod()
                                   .AllowAnyHeader()
                                   .AllowCredentials()
                                   .SetIsOriginAllowed(_ => true));
                }
            });

            services.AddUrlHelper();
            services.AddHealthChecks()
                    .AddDbContextCheck<ApplicationDbContext>();

            // Add application services.
            services.AddScoped<IImageService, AmazonS3ImageService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IStatisticsService, StatisticsService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<INewsletterService, NewsletterService>();
            services.AddTransient<IDonationService, DonationService>();
            services.AddTransient<IViewRenderService, ViewRenderService>();
            services.AddTransient<IMailChimpManager, MailChimpManager>();
            services.AddTransient<IHtmlInputValidator, HtmlInputValidator>();

            // Configure application options
            services.AddOptions<DisqusOptions>().Bind(configuration).ValidateDataAnnotations();
            services.AddOptions<StripeSignatures>().Bind(configuration).ValidateDataAnnotations();
            services.AddOptions<SiteOptions>().Bind(configuration).ValidateDataAnnotations();
            services.AddOptions<AuthMessageSenderOptions>().Bind(configuration).ValidateDataAnnotations();
            services.AddOptions<ImageServiceOptions>().Bind(configuration).ValidateDataAnnotations();
            services.AddOptions<NewsletterServiceOptions>().Bind(configuration).ValidateDataAnnotations();
            services.AddOptions<SeedOptions>().Bind(configuration).ValidateDataAnnotations();
            services.AddOptions<AnalyticsOptions>().Bind(configuration).ValidateDataAnnotations();
            services.AddOptions<MailChimpOptions>().Configure(options =>
            {
                options.ApiKey = configuration["MailChimpKey"];
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
            app.UseRouting();
            app.UseCors();
            app.UseSerilogRequestLogging();

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
