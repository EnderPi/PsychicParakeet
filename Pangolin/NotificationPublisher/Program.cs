using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EnderPi.Framework.Threading;
using EnderPi.Framework.Messaging;
using EnderPi.Framework.Caching;
using EnderPi.Framework.Messaging.Events;
using EnderPi.Framework.Pocos;
using EnderPi.Framework.DataAccess;
using EnderPi.Framework.Logging;

namespace NotificationPublisher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureAppConfiguration((context, config) => 
                {
                    //Configure the app here
                })
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    string connectionString = configuration.GetConnectionString("DefaultConnection");
                    WorkerOptions options = new WorkerOptions() { ConnectionString = connectionString , ApplicationName = configuration["ApplicationName"] };
                    ConfigurationDataAccess dataAccess = new ConfigurationDataAccess(connectionString);
                    LogDataAccess logDataAccess = new LogDataAccess(connectionString);
                    services.AddSingleton(logDataAccess);
                    services.AddSingleton(dataAccess);
                    services.AddSingleton(options);
                    services.AddHostedService<Worker>();
                });
    }
}
