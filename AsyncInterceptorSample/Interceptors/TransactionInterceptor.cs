using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using AsyncInterceptorSample.Models;
using AsyncInterceptorSample.Services;

namespace AsyncInterceptorSample.Interceptors
{
    public class TransactionInterceptor : IInterceptor
    {
        IAsyncInterceptor _asyncInterceptor;
        public TransactionInterceptor(TransactionInterceptorAsync asyncInterceptor)
        {
            _asyncInterceptor = asyncInterceptor;
        }
        public void Intercept(IInvocation invocation)
        {
  
              _asyncInterceptor.ToInterceptor().Intercept(invocation);
        }
    }
}
