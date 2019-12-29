using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AsyncInterceptorSample.Services;
using Castle.Core;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using System.Reflection;
using AsyncInterceptorSample.Attributes;

namespace AsyncInterceptorSample.Interceptors
{
    public class TraceInterceptorAsync : IAsyncInterceptor
    {
        public ILoggerFactory loggerFactory { get; set; }

        public TraceInterceptorAsync(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }
        public void InterceptAsynchronous(IInvocation invocation)
        {
            if (invocation.MethodInvocationTarget.GetCustomAttribute<TraceAttribute>() == null)
            {
                invocation.ReturnValue = InternalInterceptAsynchronous(invocation);
                return;
            }
            var logger = loggerFactory.CreateLogger(invocation.TargetType);
            logger.LogTrace(FormatLogMessage(invocation, "start"));
            try
            {
                invocation.ReturnValue = InternalInterceptAsynchronous(invocation);

            }
            finally
            {
                logger.LogTrace(FormatLogMessage(invocation, "end"));
            }
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            if (invocation.MethodInvocationTarget.GetCustomAttribute<TraceAttribute>() == null)
            {
                invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation);
                return;
            }
            var logger = loggerFactory.CreateLogger(invocation.TargetType);
            logger.LogTrace(FormatLogMessage(invocation, "start"));
            try
            {
                invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation);
            }
            finally
            {
                logger.LogTrace(FormatLogMessage(invocation, "end"));
            }
        }
        public void InterceptSynchronous(IInvocation invocation)
        {
            
            if (invocation.MethodInvocationTarget.GetCustomAttribute<TraceAttribute>() == null)
            {
                invocation.Proceed();
                return;
            }
            var logger = loggerFactory.CreateLogger(invocation.TargetType);
            logger.LogTrace(FormatLogMessage(invocation, "start"));
            try
            {
                invocation.Proceed();

            }
            finally
            {
                logger.LogTrace(FormatLogMessage(invocation, "end"));
            }
        }
        private async Task InternalInterceptAsynchronous(IInvocation invocation)
        {
            invocation.Proceed();

            var task = (Task)invocation.ReturnValue;
            await task;
        }

        private async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
        {
            invocation.Proceed();
            var task = (Task<TResult>)invocation.ReturnValue;
            TResult result = await task;
            return result;
        }
        string FormatLogMessage(IInvocation invocation, string message)
        {

            return $"{invocation.TargetType.FullName}#{invocation.MethodInvocationTarget.Name}  {message}"; ;

        }
    }
}
