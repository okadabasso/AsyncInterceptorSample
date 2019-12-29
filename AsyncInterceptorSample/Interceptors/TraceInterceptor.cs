using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using AsyncInterceptorSample.Models;
using AsyncInterceptorSample.Services;
using AsyncInterceptorSample.Attributes;

namespace AsyncInterceptorSample.Interceptors
{
    public class TraceInterceptor : IInterceptor
    {
        IAsyncInterceptor _asyncInterceptor;
        public TraceInterceptor(TraceInterceptorAsync asyncInterceptor)
        {
            _asyncInterceptor = asyncInterceptor;
        }
        public void Intercept(IInvocation invocation)
        {
            _asyncInterceptor.ToInterceptor().Intercept(invocation);
        }
    }
}
