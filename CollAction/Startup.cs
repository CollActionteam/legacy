using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CollAction.Data;
using CollAction.Models;
using System.Threading.Tasks;
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
using CollAction.Services.Festival;
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
using System;
using AspNetCore.IServiceCollection.AddIUrlHelper;

namespace CollAction
{
    public class Startup
    {
        private readonly string CorsPolicy = "FrontendCors";

        public Startup(IHostingEnvironment env, IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            Environment = env;
            Logger = logger;
        }

        public ILogger<Startup> Logger { get; }

        public IConfiguration Configuration { get; }

        public IHostingEnvironment Environment { get; }

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

            services.AddAuthentication()
                    .AddFacebook(options =>
                    {
                        options.AppId = Configuration["Authentication:Facebook:AppId"];
                        options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                        options.Scope.Add("email");
                    }).AddGoogle(options =>
                    {
                        options.ClientId = Configuration["Authentication:Google:ClientId"];
                        options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                        options.Scope.Add("email");
                    }).AddTwitter(options =>
                    {
                        options.ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"];
                        options.ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"];
                    });

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

            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddHangfire(config => config.UsePostgreSqlStorage(connectionString));

            // Add application services.
            services.AddScoped<IImageService, AmazonS3ImageService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<INewsletterService, NewsletterService>();
            services.AddTransient<IDonationService, DonationService>();
            services.AddTransient<IViewRenderService, ViewRenderService>();
            services.AddTransient<IMailChimpManager, MailChimpManager>();
            services.AddTransient<IFestivalService, FestivalService>();
            services.AddTransient<IHtmlInputValidator, HtmlInputValidator>();
            services.AddUrlHelper();

            services.AddDataProtection()
                    .Services
                    .Configure<KeyManagementOptions>(options => options.XmlRepository = new DataProtectionRepository(new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(connectionString).Options));

            services.AddCors(c =>
            {
                string publicAddress = Configuration["PublicAddress"];
                c.AddPolicy(
                    CorsPolicy,
                    builder =>
                        builder.AllowAnyMethod()
                               .AllowAnyHeader()
                               .AllowCredentials()
                               .SetIsOriginAllowed(o => o == publicAddress)
                               .WithOrigins(publicAddress));
            });

            services.Configure<StripeSignatures>(Configuration);
            services.Configure<SiteOptions>(Configuration);
            services.Configure<DisqusOptions>(Configuration);
            services.Configure<AuthMessageSenderOptions>(Configuration);
            services.Configure<ImageServiceOptions>(Configuration);
            services.Configure<ImageProcessingOptions>(Configuration);
            services.Configure<NewsletterServiceOptions>(Configuration);
            services.Configure<FestivalServiceOptions>(Configuration);
            services.Configure<ProjectEmailOptions>(Configuration);
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

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            app.UseCors(CorsPolicy);

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
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
                app.UseGraphiQl("/graphiql", "/graphql");
            }
            else
            {
                app.UseExceptionHandler("error");
            }

            app.UseAuthentication();

            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = ctx =>
                {
                    const int cacheDurationSeconds = 60 * 60 * 24 * 7;
                    ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] = $"public,max-age={cacheDurationSeconds}";
                }
            });

            app.UseHangfireServer(new BackgroundJobServerOptions() { WorkerCount = 1 });

            app.UseHangfireDashboard(
                options: new DashboardOptions()
                {
                    Authorization = new IDashboardAuthorizationFilter[]
                    {
                        new HangfireAdminAuthorizationFilter()
                    }
                });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}");
            });

            InitializeDatabase(app.ApplicationServices);
        }

        public void InitializeDatabase(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                Task.Run(async () =>
                {
                    Logger.LogInformation("migrating database");
                    await context.Database.MigrateAsync();
                    Logger.LogInformation("seeding database");
                    await context.Seed(Configuration, userManager, roleManager);
                    Logger.LogInformation("done starting up");
                }).Wait();
            }
        }
    }
}
