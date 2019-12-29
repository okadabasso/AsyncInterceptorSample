using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using AsyncInterceptorSample.Attributes;
using AsyncInterceptorSample.Interceptors;
using AsyncInterceptorSample.Services;
using Autofac;
using Autofac.Extras.DynamicProxy;

namespace AsyncInterceptorSample
{
    public class ComponentManager
    {
        public static void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(ComponentManager).Assembly)
                .Where(t => t.GetCustomAttribute<TransactionalComponentAttribute>() != null)
                 .AsSelf()
                 .EnableClassInterceptors()
                 .InterceptedBy(typeof(TraceInterceptor), typeof(TransactionInterceptor))
                 .PropertiesAutowired();

            builder.RegisterType<TransactionInterceptor>().AsSelf();
            builder.RegisterType<TransactionInterceptorAsync>().As<TransactionInterceptorAsync>();
            builder.RegisterType<TraceInterceptor>().AsSelf();
            builder.RegisterType<TraceInterceptorAsync>().As<TraceInterceptorAsync>();


        }
    }
}
