using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AsyncInterceptorSample.Attributes;
using AsyncInterceptorSample.Services;
using Castle.Core;
using Castle.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AsyncInterceptorSample.Interceptors
{
    public class TransactionInterceptorAsync : IAsyncInterceptor
    {
        public ILoggerFactory loggerFactory { get; set; }

        public TransactionInterceptorAsync(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }
        public void InterceptAsynchronous(IInvocation invocation)
        {
            invocation.ReturnValue = InternalInterceptAsynchronous(invocation);
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation);
        }

        public void InterceptSynchronous(IInvocation invocation)
        {
            var logger = loggerFactory.CreateLogger(invocation.TargetType);
            try
            {
                if (CanIntercept(invocation.TargetType))
                {
                    invocation.Proceed();
                    return;
                }

                var context = ((FooService)invocation.InvocationTarget).Context;
                using (var transaction = context.Database.BeginTransaction())
                {
                    logger.LogDebug("transaction start");
                    invocation.Proceed();
                    transaction.Commit();
                    logger.LogDebug("transaction end");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            finally
            {
            }
        }
        private async Task InternalInterceptAsynchronous(IInvocation invocation)
        {
            var logger = loggerFactory.CreateLogger(invocation.TargetType);
            try
            {
                if (CanIntercept(invocation.TargetType))
                {
                    await Invoke(invocation);
                    return;
                }

                var context = ((FooService)invocation.InvocationTarget).Context;
                using (var transaction = context.Database.BeginTransaction())
                {
                    logger.LogDebug("transaction start");
                    await Invoke(invocation);
                    transaction.Commit();
                    logger.LogDebug("transaction end");
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "");
            }
        }


        private async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
        {
 
            var logger = loggerFactory.CreateLogger(invocation.InvocationTarget.GetType());
            try
            {

                if (CanIntercept(invocation.TargetType))
                {
                    return await InvokeAsync<TResult>(invocation);
                }

                var context = GetDbContext(invocation);
                using (var transaction = context.Database.BeginTransaction())
                {
                    logger.LogDebug("transaction start");
                    TResult result = await InvokeAsync<TResult>(invocation);

                    transaction.Commit();
                    logger.LogDebug("transaction end");
                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"");
                return (TResult)(object)ex.Message;
            }

        }
        private bool CanIntercept(Type t)
        {
            return t.GetCustomAttribute<TransactionalMethodAttribute>() != null;
        }
        private async Task Invoke(IInvocation invocation)
        {
            invocation.Proceed();
            var task = (Task)invocation.ReturnValue;
            await task;
        }

        private async Task<TResult> InvokeAsync<TResult>(IInvocation invocation)
        {
            invocation.Proceed();
            var task = (Task<TResult>)invocation.ReturnValue;
            TResult result = await task;
            return result;
        }

        static readonly ConcurrentDictionary<Type, Func<object, DbContext>> DbContextGetFuncs = new ConcurrentDictionary<Type, Func<object, DbContext>>();
        DbContext GetDbContext(IInvocation invocation)
        {
            var func = DbContextGetFuncs.GetOrAdd(invocation.TargetType, t => {
                var property = t.GetProperties().Where(x => x.PropertyType.IsSubclassOf(typeof(DbContext))).FirstOrDefault() ;
                var parameter = Expression.Parameter(typeof(object), "service");
                var convert = Expression.Convert(parameter, t);
                var propertyAccess = Expression.Property(convert, property);
                var lambda = Expression.Lambda<Func<object, DbContext>>(propertyAccess, parameter);

                return lambda.Compile();
            });

            return func(invocation.InvocationTarget);

        }
    }
}
