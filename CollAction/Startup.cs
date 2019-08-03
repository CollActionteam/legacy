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
using System.Linq;
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
using CollAction.Services.Sitemap;
using GraphiQl;
using Microsoft.AspNetCore.Mvc;
using CollAction.GraphQl;
using CollAction.Services.User;

namespace CollAction
{
    public class Startup
    {
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
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<INewsletterService, NewsletterService>();
            services.AddTransient<IDonationService, DonationService>();
            services.AddTransient<IViewRenderService, ViewRenderService>();
            services.AddTransient<IMailChimpManager, MailChimpManager>();
            services.AddTransient<ISitemapService, SitemapService>();
            services.AddTransient<IFestivalService, FestivalService>();
            services.AddTransient<IHtmlInputValidator, HtmlInputValidator>();

            services.AddDataProtection()
                    .Services
                    .Configure<KeyManagementOptions>(options => options.XmlRepository = new DataProtectionRepository(new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(connectionString).Options));

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

                applicationLifetime.ApplicationStopping.Register(() => Log.CloseAndFlush());
            }

            if (!Configuration.GetValue<bool>("CspDisable"))
            {
                ConfigureCsp(app, env, Configuration);
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
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "CatchAll",
                    "{*url}",
                    new { controller = "Home", action = "Index" });
            });

            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
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

        private static void ConfigureCsp(IApplicationBuilder app, IHostingEnvironment env, IConfiguration configuration)
        {
            app.UseSecurityHeaders(
                new HeaderPolicyCollection() // See https://www.owasp.org/index.php/OWASP_Secure_Headers_Project
                   .AddStrictTransportSecurityMaxAgeIncludeSubDomains() // Add a HSTS header, making sure browsers connect to collaction + subdomains with https from now on
                   .AddXssProtectionEnabled() // Enable browser xss protection
                   .AddContentTypeOptionsNoSniff() // Ensure the browser doesn't guess/sniff content-types
                   .AddReferrerPolicyStrictOriginWhenCrossOrigin() // Send a full URL when performing a same-origin request, only send the origin of the document to a-priori as-much-secure destination (HTTPS->HTTPS), and send no header to a less secure destination (HTTPS->HTTP) 
                   .AddFrameOptionsDeny() // No framing allowed (put us inside a frame tag)
                   .AddContentSecurityPolicy(cspBuilder =>
                   {
                       cspBuilder.AddBlockAllMixedContent(); // Block mixed http/https content
                       cspBuilder.AddUpgradeInsecureRequests(); // Upgrade all http requests to https
                       cspBuilder.AddObjectSrc().Self().Sources.AddRange(configuration["CspObjectSrc"]?.Split(";") ?? new string[0]); // Only allow plugins/objects from our own site, or configured sources
                       cspBuilder.AddFormAction().Self() // Only allow form actions to our own site, or mailinator, or social media logins, or configured sources
                                                 .Sources.AddRange(new[]
                                                                   {
                                                                       "https://collaction.us14.list-manage.com/",
                                                                       "https://www.facebook.com/",
                                                                       "https://m.facebook.com",
                                                                       "https://accounts.google.com/",
                                                                       "https://api.twitter.com/",
                                                                       "https://www.twitter.com/"
                                                                   }.Concat(configuration["CspFormAction"]?.Split(";") ?? new string[0]));
                       cspBuilder.AddConnectSrc().Self() // Only allow API calls to self, and the websites we use for the share buttons, app insights or configured sources
                                                 .Sources.AddRange(new[]
                                                                   {
                                                                       "https://www.linkedin.com/",
                                                                       "https://linkedin.com/",
                                                                       "https://www.twitter.com/",
                                                                       "https://twitter.com/",
                                                                       "https://www.facebook.com/",
                                                                       "https://facebook.com/",
                                                                       "https://graph.facebook.com/",
                                                                       "https://dc.services.visualstudio.com/",
                                                                       "https://api.stripe.com",
                                                                       "*.disqus.com"
                                                                   }.Concat(configuration["CspConnectSrc"]?.Split(";") ?? new string[0]));
                       cspBuilder.AddImgSrc().Self() // Only allow self-hosted images, or google analytics (for tracking images), or configured sources
                                             .Data()    // Used for project creation image preview
                                             .Sources.AddRange(new[]
                                                               {
                                                                   "https://www.google-analytics.com",
                                                                   $"https://{configuration["S3Bucket"]}.s3.{configuration["S3Region"]}.amazonaws.com",
                                                                   "*.disquscdn.com",
                                                                   "*.disqus.com"
                                                               }.Concat(configuration["CspImgSrc"]?.Split(";") ?? new string[0]));
                       cspBuilder.AddStyleSrc().Self() // Only allow style/css from these sources (note: css injection can actually be dangerous), or configured sources
                                               .UnsafeInline() // Unfortunately this is necessary, the backend passess some things that are directly passed into css style attributes, especially on the project page. TODO: We should try to get rid of this.
                                               .Sources.AddRange(new[]
                                                                 {
                                                                     "https://maxcdn.bootstrapcdn.com/",
                                                                     "https://fonts.googleapis.com/",
                                                                     "*.disquscdn.com"
                                                                 }.Concat(configuration["CspStyleSrc"]?.Split(";") ?? new string[0]));
                       cspBuilder.AddFontSrc().Self() // Only allow fonts from these sources, or configured sources
                                              .Sources.AddRange(new[]
                                                               {
                                                                   "https://maxcdn.bootstrapcdn.com/",
                                                                   "https://fonts.googleapis.com/",
                                                                   "https://fonts.gstatic.com"
                                                               }.Concat(configuration["CspFontSrc"]?.Split(";") ?? new string[0]));
                       cspBuilder.AddFrameAncestors().Sources.AddRange(configuration["CspFrameAncestors"]?.Split(";") ?? new string[0]); // Only allow configured sources
                       cspBuilder.AddMediaSrc().Self()
                                               .Sources.AddRange(configuration["CspMediaSrc"]?.Split(";") ?? new string[0]); // Only allow self-hosted videos, or configured sources
                       cspBuilder.AddFrameAncestors().None(); // No framing allowed here (put us inside a frame tag)
                       cspBuilder.AddFrameSource().Self() // Workaround for chrome bug, apparently chrome can't uses images with svg src, so we have to use object tags. Additionally, apparently "obj" tags count as frames for chrome.. so we have to allow them through the CSP.. nice.
                                                  .Sources
                                                  .AddRange(new[]
                                                            {
                                                                "https://js.stripe.com",
                                                                "https://hooks.stripe.com",
                                                                "https://www.youtube.com/",
                                                                "disqus.com"
                                                            });
                       cspBuilder.AddScriptSrc() // Only allow scripts from our own site, the aspnetcdn site, app insights and google analytics
                                 .Self()
                                 .Sources.AddRange(new[]
                                                   {
                                                       "https://ajax.aspnetcdn.com",
                                                       "https://www.googletagmanager.com",
                                                       "https://www.google-analytics.com",
                                                       "https://js.stripe.com",
                                                       "disqus.com",
                                                       "*.disqus.com",
                                                       "*.disquscdn.com",
                                                       "*.msecnd.net",
                                                       "'sha256-EHA5HNhe/+uz3ph6Fw34N85vHxX87fsJ5cH4KbZKIgU='"
                                                   }.Concat(configuration["CspScriptSrc"]?.Split(";") ?? new string[0])
                                                    .Concat(env.IsDevelopment() ? new[] 
                                                                                  {
                                                                                          "'unsafe-eval'", // In development mode webpack uses eval to load debug information
                                                                                          "'sha256-fukrxzq0omEGjqEtLClugW+6p58X8+bd1j2EvtdR+i4='" // GraphIQL
                                                                                  } 
                                                                                : Enumerable.Empty<string>()));
                   }));
        }
    }
}
