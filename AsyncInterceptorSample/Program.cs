using AsyncInterceptorSample.Models;
using AsyncInterceptorSample.Services;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySql.Data;
using MySql;
using NLog.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace AsyncInterceptorSample
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            Console.ReadLine();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureHostConfiguration(config => {
                    config
                        .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", false, true);
                })
                .UseNLog()
                .ConfigureLogging((context, logging) => {
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                
                .ConfigureServices((hostContext, services) => {
                    services.AddHostedService<AsyncInterceptorSampleService>();
                    services.AddDbContext<ApplicationDbContext>(option =>
                    {
                        option.UseMySql(hostContext.Configuration.GetConnectionString("default"))
                        .EnableDetailedErrors(true)
                        .EnableSensitiveDataLogging(true);

                    });
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    ComponentManager.ConfigureServices(builder);
                })
            ;
                
    }


}
