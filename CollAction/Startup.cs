using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CollAction.Data;
using CollAction.Models;
using CollAction.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Slack;
using Amazon;
using System.Linq;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Rewrite;
using NetEscapades.AspNetCore.SecurityHeaders;
using NetEscapades.AspNetCore.SecurityHeaders.Infrastructure;
using System;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace CollAction
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
            Environment = env;
        }

        public IConfigurationRoot Configuration { get; }
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

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
            });

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

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc()
                    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                    .AddDataAnnotationsLocalization();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.Configure<AuthMessageSenderOptions>(options =>
            {
                options.FromAddress = Configuration["FromAddress"];
                options.Region = RegionEndpoint.EnumerableAllRegions.First(r => r.SystemName == Configuration["SesRegion"]);
                options.SesAwsAccessKey = Configuration["SesAwsAccessKey"];
                options.SesAwsAccessKeyID = Configuration["SesAwsAccessKeyID"];
            });
            services.AddScoped<IProjectService, ProjectService>();
            services.AddTransient<INewsletterSubscriptionService, NewsletterSubscriptionService>();
            services.Configure<NewsletterSubscriptionServiceOptions>(options =>
            {
                options.MailChimpKey = Configuration["MailChimpKey"];
                options.MailChimpNewsletterListId = Configuration["MailChimpNewsletterListId"];
            });
            services.AddTransient<IFestivalService, FestivalService>();
            services.Configure<FestivalServiceOptions>(options =>
            {
                // Note: use yyyy-MM-dd HH:mm:ss notation. Don't specify to use the default value (2018-05-27 23:59:59)
                options.FestivalEndDate = Configuration.GetValue<DateTime?>("FestivalEndDate");
            });

            services.AddDataProtection()
                    .Services.Configure<KeyManagementOptions>(options => options.XmlRepository = new DataProtectionRepository(new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(connectionString).Options));

            if (Environment.IsProduction())
                services.AddApplicationInsightsTelemetry(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("nl-NL")
            };

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

                if (!Configuration.GetValue<bool>("CspDisable"))
                {
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
                                                                           "https://accounts.google.com/",
                                                                           "https://api.twitter.com/",
                                                                           "https://www.twitter.com/"
                                                                       }.Concat(Configuration["CspFormAction"]?.Split(";") ?? new string[0]));
                           cspBuilder.AddConnectSrc().Self() // Only allow API calls to self, and the websites we use for the share buttons, or configured sources
                                                     .Sources.AddRange(new[]
                                                                       {
                                                                           "https://www.linkedin.com/",
                                                                           "https://linkedin.com/",
                                                                           "https://www.twitter.com/",
                                                                           "https://twitter.com/",
                                                                           "https://www.facebook.com/",
                                                                           "https://facebook.com/",
                                                                           "https://graph.facebook.com/"
                                                                       }.Concat(Configuration["CspConnectSrc"]?.Split(";") ?? new string[0]));
                           cspBuilder.AddImgSrc().Self() // Only allow self-hosted images, or google analytics (for tracking images), or configured sources
                                                 .Sources.AddRange(new[]
                                                                   {
                                                                       "https://www.google-analytics.com"
                                                                   }.Concat(Configuration["CspImgSrc"]?.Split(";") ?? new string[0]));
                           cspBuilder.AddStyleSrc().Self() // Only allow style/css from these sources (note: css injection can actually be dangerous), or configured sources
                                                   .UnsafeInline() // Unfortunately this is necessary, the backend passess some things that are directly passed into css style attributes, especially on the project page. TODO: We should try to get rid of this.
                                                   .Sources.AddRange(new[]
                                                                     {
                                                                         "https://maxcdn.bootstrapcdn.com/",
                                                                         "https://fonts.googleapis.com/"
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
                           cspBuilder.AddScriptSrc() // Only allow scripts from our own site, the aspnetcdn site and google analytics
                                     .Self()
                                     .Sources.AddRange(new[]
                                                       {
                                                           "https://ajax.aspnetcdn.com",
                                                           "https://www.googletagmanager.com",
                                                           "https://www.google-analytics.com",
                                                           "'sha256-EHA5HNhe/+uz3ph6Fw34N85vHxX87fsJ5cH4KbZKIgU='"
                                                       }.Concat(Configuration["CspScriptSrc"]?.Split(";") ?? new string[0]));
                        })
                    );
                }

                app.UseRewriter(new RewriteOptions().AddRedirectToHttpsPermanent());
            }

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
            });

            // Configure logging
            LoggerConfiguration configuration = new LoggerConfiguration()
                .WriteTo.RollingFile("log-{Date}.txt", LogEventLevel.Information)
                .WriteTo.Console(LogEventLevel.Information);

            if (!string.IsNullOrEmpty(Configuration["SlackHook"]))
                configuration.WriteTo.Slack(Configuration["SlackHook"], restrictedToMinimumLevel: LogEventLevel.Error);

            if (env.IsDevelopment())
                configuration.WriteTo.Trace();

            Log.Logger = configuration.CreateLogger();
            loggerFactory.AddSerilog();
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

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute("find",
                     "find",
                     new { controller = "Projects", action = "Find" }
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

                routes.MapRoute("faq",
                     "faq",
                     new { controller = "Home", action = "FAQ" }
                 );

                routes.MapRoute("contact",
                     "contact",
                     new { controller = "Home", action = "Contact" }
                 );

                routes.MapRoute("robots.txt",
                    "robots.txt",
                    new { controller = "Home", action = "Robots" });

                routes.MapRoute("sitemap.xml",
                    "sitemap.xml",
                    new { controller = "Home", action = "Sitemap" });

                routes.MapRoute("getCategories",
                     "api/categories",
                     new { controller = "Projects", action = "GetCategories" }
                 );

                routes.MapRoute("GetTileProjects",
                     "api/projects/find",
                     new { controller = "Projects", action = "GetTileProjects" }
                 );

                routes.MapRoute("GetStatuses",
                     "api/status",
                     new { controller = "Projects", action = "GetStatuses" }
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
                context.Database.Migrate();
                Task.Run(() => context.Seed(Configuration, userManager, roleManager)).Wait();
            }
        }
    }
}
