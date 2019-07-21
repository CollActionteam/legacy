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
using CollAction.Services.Project;
using CollAction.Services.Newsletter;
using CollAction.Services.Festival;
using CollAction.Services.DataProtection;
using CollAction.Services.Image;
using Microsoft.AspNetCore.Http;
using Stripe;
using CollAction.Services.Donation;
using CollAction.Services;
using Serilog.Events;
using Serilog.Sinks.Slack;
using Microsoft.ApplicationInsights.Extensibility;
using CollAction.Services.ViewRender;
using AspNetCore.IServiceCollection.AddIUrlHelper;
using MailChimp.Net;
using MailChimp.Net.Interfaces;
using CollAction.Services.HashAssetService;

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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = $"Host={Configuration["DbHost"]};Username={Configuration["DbUser"]};Password={Configuration["DbPassword"]};Database={Configuration["Db"]};Port={Configuration["DbPort"]}";

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

            services.AddUrlHelper();

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

            services.AddMvc();

            services.AddHangfire(config => config.UsePostgreSqlStorage(connectionString));

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IParticipantsService, ParticipantsService>();
            services.AddScoped<IImageService, AmazonS3ImageService>();
            services.AddTransient<INewsletterSubscriptionService, NewsletterSubscriptionService>();
            services.AddTransient<IFestivalService, FestivalService>();
            services.AddTransient<IDonationService, DonationService>();
            services.AddTransient<IViewRenderService, ViewRenderService>();
            services.AddTransient<IMailChimpManager, MailChimpManager>();
            services.AddSingleton<IHashAssetService, HashAssetService>(provider => new HashAssetService(!Environment.IsDevelopment()));

            services.AddDataProtection()
                    .Services.Configure<KeyManagementOptions>(options => options.XmlRepository = new DataProtectionRepository(new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(connectionString).Options));

            services.Configure<StripeSignatures>(Configuration);
            services.Configure<SiteOptions>(Configuration);
            services.Configure<DisqusOptions>(Configuration);
            services.Configure<AuthMessageSenderOptions>(Configuration);
            services.Configure<ImageServiceOptions>(Configuration);
            services.Configure<ImageProcessingOptions>(Configuration);
            services.Configure<NewsletterSubscriptionServiceOptions>(Configuration);
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
            services.Configure<StripeJavascriptOptions>(options =>
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

            services.Configure<CookiePolicyOptions>(options => 
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
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
            }

            if (!Configuration.GetValue<bool>("CspDisable"))
            {
                Logger.LogInformation("enabling security headers");
                app.UseSecurityHeaders(new HeaderPolicyCollection() // See https://www.owasp.org/index.php/OWASP_Secure_Headers_Project
                   .AddStrictTransportSecurityMaxAgeIncludeSubDomains() // Add a HSTS header, making sure browsers connect to collaction + subdomains with https from now on
                   .AddXssProtectionEnabled() // Enable browser xss protection
                   .AddContentTypeOptionsNoSniff() // Ensure the browser doesn't guess/sniff content-types
                   .AddReferrerPolicyStrictOriginWhenCrossOrigin() // Send a full URL when performing a same-origin request, only send the origin of the document to a-priori as-much-secure destination (HTTPS->HTTPS), and send no header to a less secure destination (HTTPS->HTTP) 
                   .AddFrameOptionsDeny() // No framing allowed (put us inside a frame tag)
                   .AddContentSecurityPolicy(cspBuilder =>
                   {
                       cspBuilder.AddBlockAllMixedContent(); // Block mixed http/https content
                       cspBuilder.AddUpgradeInsecureRequests(); // Upgrade all http requests to https
                       cspBuilder.AddObjectSrc().Self().Sources.AddRange(Configuration["CspObjectSrc"]?.Split(";") ?? new string[0]); // Only allow plugins/objects from our own site, or configured sources
                       cspBuilder.AddFormAction().Self() // Only allow form actions to our own site, or mailinator, or social media logins, or configured sources
                                                 .Sources.AddRange(new[]
                                                                   {
                                                                       "https://collaction.us14.list-manage.com/",
                                                                       "https://www.facebook.com/",
                                                                       "https://m.facebook.com",
                                                                       "https://accounts.google.com/",
                                                                       "https://api.twitter.com/",
                                                                       "https://www.twitter.com/"
                                                                   }.Concat(Configuration["CspFormAction"]?.Split(";") ?? new string[0]));
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
                                                                   }.Concat(Configuration["CspConnectSrc"]?.Split(";") ?? new string[0]));
                       cspBuilder.AddImgSrc().Self() // Only allow self-hosted images, or google analytics (for tracking images), or configured sources
                                             .Data()    // Used for project creation image preview
                                             .Sources.AddRange(new[]
                                                               {
                                                                   "https://www.google-analytics.com",
                                                                   $"https://{Configuration["S3Bucket"]}.s3.{Configuration["S3Region"]}.amazonaws.com",
                                                                   "*.disquscdn.com",
                                                                   "*.disqus.com"
                                                               }.Concat(Configuration["CspImgSrc"]?.Split(";") ?? new string[0]));
                       cspBuilder.AddStyleSrc().Self() // Only allow style/css from these sources (note: css injection can actually be dangerous), or configured sources
                                               .UnsafeInline() // Unfortunately this is necessary, the backend passess some things that are directly passed into css style attributes, especially on the project page. TODO: We should try to get rid of this.
                                               .Sources.AddRange(new[]
                                                                 {
                                                                     "https://maxcdn.bootstrapcdn.com/",
                                                                     "https://fonts.googleapis.com/",
                                                                     "*.disquscdn.com"
                                                                 }.Concat(Configuration["CspStyleSrc"]?.Split(";") ?? new string[0]));
                       cspBuilder.AddFontSrc().Self() // Only allow fonts from these sources, or configured sources
                                              .Sources.AddRange(new[]
                                                               {
                                                                   "https://maxcdn.bootstrapcdn.com/",
                                                                   "https://fonts.googleapis.com/",
                                                                   "https://fonts.gstatic.com"
                                                               }.Concat(Configuration["CspFontSrc"]?.Split(";") ?? new string[0]));
                       cspBuilder.AddFrameAncestors().Sources.AddRange(Configuration["CspFrameAncestors"]?.Split(";") ?? new string[0]); // Only allow configured sources
                       cspBuilder.AddMediaSrc().Self()
                                               .Sources.AddRange(Configuration["CspMediaSrc"]?.Split(";") ?? new string[0]); // Only allow self-hosted videos, or configured sources
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
                                                   }.Concat(Configuration["CspScriptSrc"]?.Split(";") ?? new string[0])
                                                    .Concat(env.IsDevelopment() ? new[] { "'unsafe-eval'" } : // In development mode webpack uses eval to load debug information
                                                                                  Enumerable.Empty<string>()));
                   })
                );
            }

            applicationLifetime.ApplicationStopping.Register(() => Log.CloseAndFlush());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStatusCodePages();
            
            app.UseCookiePolicy();

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

            app.UseHangfireDashboard(options: new DashboardOptions()
            {
                Authorization = new IDashboardAuthorizationFilter[] {
                    new HangfireAdminAuthorizationFilter()
                }
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute("find",
                     "find",
                     new { controller = "Projects", action = "Find" }
                );

                routes.MapRoute("details",
                     "Projects/{name}/{id}/details",
                     new { controller = "Projects", action = "Details" }
                 );

                routes.MapRoute("participate",
                     "Projects/{name}/{id}/participate",
                     new { controller = "Projects", action = "Commit" }
                 );

                routes.MapRoute("thankyoucommit",
                     "Projects/{name}/{id}/thankyou",
                     new { controller = "Projects", action = "ThankYouCommit" }
                 );

                routes.MapRoute("thankyoucreate",
                     "Projects/Create/{name}/{id}/thankyou",
                     new { controller = "Projects", action = "ThankYouCreate" }
                 );

                routes.MapRoute("start",
                     "start",
                     new { controller = "Projects", action = "StartInfo" }
                );

                routes.MapRoute("about",
                     "about",
                     new { controller = "Home", action = "About" }
                 );

                routes.MapRoute("crowdactingfestival",
                     "crowdactingfestival",
                     new { controller = "Home", action = "CrowdActingFestival" }
                 );

                routes.MapRoute("robots.txt",
                    "robots.txt",
                    new { controller = "Home", action = "Robots" });

                routes.MapRoute("sitemap.xml",
                    "sitemap.xml",
                    new { controller = "Home", action = "Sitemap" });

                routes.MapRoute("GetCategories",
                     "api/categories",
                     new { controller = "Projects", action = "GetCategories" }
                 );

                routes.MapRoute("FindProject",
                     "api/projects/{projectId:int}",
                     new { controller = "Projects", action = "FindProject" }
                 );                 

                routes.MapRoute("FindProjects",
                     "api/projects/find",
                     new { controller = "Projects", action = "FindProjects" }
                 );

                routes.MapRoute("GetStatuses",
                     "api/status",
                     new { controller = "Projects", action = "GetStatuses" }
                 );

                routes.MapRoute("MyProjects",
                     "api/manage/myprojects",
                     new { controller = "Manage", action = "MyProjects" }
                 );

                routes.MapRoute("ParticipatingInProjects",
                     "api/manage/participating",
                     new { controller = "Manage", action = "ParticipatingInProjects" }
                 );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>())
            using (var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>())
            using (var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
            {
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
