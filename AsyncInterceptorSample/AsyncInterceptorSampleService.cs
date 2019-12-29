using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AsyncInterceptorSample.Services;
using AsyncInterceptorSample.Models;
using Microsoft.Extensions.Logging;

namespace AsyncInterceptorSample
{
    class AsyncInterceptorSampleService : IHostedService
    {
        private readonly IApplicationLifetime appLifetime;
        private readonly ApplicationDbContext dbContext;
        private readonly FooService fooService;
        private readonly ILogger<AsyncInterceptorSampleService> logger;
        public AsyncInterceptorSampleService(
            IApplicationLifetime appLifetime,
            ApplicationDbContext dbContext,
            ILogger<AsyncInterceptorSampleService> logger,
            FooService service)
        {
            this.appLifetime = appLifetime;
            this.dbContext = dbContext;
            this.logger = logger;
            fooService = service;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.appLifetime.ApplicationStarted.Register(OnStarted);
            logger.LogInformation("task started");
            return Task.CompletedTask;
        }

        public  Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("task stopped");
            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            var task = fooService.HelloAsync();
            task.Wait();
            logger.LogDebug($"async result = {task.Result}");

            dbContext.ClearEntryState();

            var result = fooService.Hello();
            logger.LogDebug($"sync result = {result}");

            this.appLifetime.StopApplication(); 
        }
    }
}
