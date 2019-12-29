using AsyncInterceptorSample.Attributes;
using AsyncInterceptorSample.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AsyncInterceptorSample.Services
{
    [TransactionalComponent]
    public class FooService
    {
        public ILogger<FooService> Logger { get; set; }
        public ApplicationDbContext Context { get; set; }

        public FooService(ILoggerFactory loggerFactory)
        {

        }
        [Trace]
        [TransactionalMethod]
        public virtual async Task<string>HelloAsync()
        {

            Logger.LogDebug("start hello async");

             await Context.Foos.AddAsync(new Foo()
            {
                 Id = 2,
                Name = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            });
            await Context.SaveChangesAsync();

            Logger.LogDebug("end hello async");
            return "done";
        }
        [Trace]
        [TransactionalMethod]
        public virtual string Hello()
        {
            Logger.LogDebug("start hello");

            Context.Foos.Add(new Foo()
            {

                Name = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            });
            Context.SaveChanges();

            Logger.LogDebug("end hello");
            return "done";
        }

    }
}
