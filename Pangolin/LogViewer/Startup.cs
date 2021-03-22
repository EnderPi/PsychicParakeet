using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LogViewer.Areas.Identity;
using LogViewer.Data;
using EnderPi.Framework.Logging;
using EnderPi.Framework.DataAccess;
using EnderPi.Framework.Messaging;
using EnderPi.Framework.Interfaces;
using EnderPi.Framework.Pocos;
using EnderPi.Framework.Threading;
using EnderPi.Framework.Caching;
using EnderPi.Framework.BackgroundWorker;

namespace LogViewer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            var applicationName = Configuration["ApplicationName"];
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

            var configurationDataAccess = new ConfigurationDataAccess(connectionString);
            WebAppConfigurations configs = configurationDataAccess.GetApplicationConfigurationValues<WebAppConfigurations>(applicationName);

            var logDataAccess = new LogDataAccess(connectionString);
            var myLogger = new Logger(logDataAccess, applicationName, configs.LogLevel);

            var publishingQueueName = configurationDataAccess.GetGlobalSettingString(GlobalSettings.EventPublishingQueue);
            var publishingMessageQueue = new MessageQueue(connectionString, publishingQueueName);
            var receivingMessageQueue = new MessageQueue(connectionString, configs.EventQueueName);
            var taskParameters = new ThrottledTaskProcessorParameters(1, 30, 4000, 120, false);
            var eventManager = new EventManager(connectionString, publishingMessageQueue, receivingMessageQueue, taskParameters, myLogger);
            
            eventManager.StartListening();
            configurationDataAccess.EventManager = eventManager;
            CachedConfigurationProvider cachedConfigurationProvider = new CachedConfigurationProvider(configurationDataAccess, new TimedCache());

            var simDataAccess = new SimulationDataAccess(connectionString);

            var speciesNames = new SpeciesNameDataAccess(connectionString);

            services.AddSingleton(logDataAccess);
            services.AddSingleton(myLogger);
            services.AddSingleton<IConfigurationDataAccess>(cachedConfigurationProvider);
            services.AddSingleton<ConfigurationDataAccess>(configurationDataAccess);
            services.AddSingleton<IEventManager>(eventManager);
            services.AddSingleton<IBackgroundTaskManager>(new BackgroundTaskManager(simDataAccess, myLogger, cachedConfigurationProvider));
            services.AddSingleton<ISpeciesNameDataAccess>(speciesNames);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
