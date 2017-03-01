using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CollAction.Data;
using CollAction.Models;
using CollAction.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Slack;
using System;

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
                builder.AddUserSecrets();
            }

            Configuration = builder.Build();
            Environment = env;
        }

        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql($"Host={Configuration["DbHost"]};Username={Configuration["DbUser"]};Password={Configuration["DbPassword"]};Database={Configuration["Db"]};Port={Configuration["DbPort"]}"));

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

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc()
                    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                    .AddDataAnnotationsLocalization();

            services.Configure<MvcOptions>(options =>
            {
                if (Environment.IsProduction())
                    options.Filters.Add(new RequireHttpsAttribute());
            });

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.Configure<AuthMessageSenderOptions>(options =>
            {
                options.FromName = Configuration["FromName"];
                options.FromAddress = Configuration["FromAddress"];
                options.SendGridKey = Configuration["SendGridKey"];
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("nl-NL")
            };
            
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
            });

            // Configure logging
            LoggerConfiguration configuration = new LoggerConfiguration()
                .WriteTo.RollingFile("log-{Date}.txt", LogEventLevel.Information);
            
            if (Configuration["SlackHook"] != null)
                configuration.WriteTo.Slack(Configuration["SlackHook"], null, null, null, null, null, LogEventLevel.Error);

            if (env.IsDevelopment())
            {
                configuration.WriteTo.LiterateConsole()
                             .WriteTo.Trace();
            }

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

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

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

                routes.MapRoute("faq",
                     "faq",
                     new { controller = "Home", action = "FAQ" }
                 );

                routes.MapRoute("contact",
                     "contact",
                     new { controller = "Home", action = "Contact" }
                 );

                routes.MapRoute("getCategories",
                     "categories",
                     new { controller = "Projects", action = "GetCategories" }
                 );
                
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            using (var userManager = app.ApplicationServices.GetService<UserManager<ApplicationUser>>())
            using (var roleManager = app.ApplicationServices.GetService<RoleManager<IdentityRole>>())
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
            {
                context.Database.Migrate();
                Task.Run(() => context.Seed(Configuration, userManager, roleManager)).Wait();
            }
        }
    }
}
