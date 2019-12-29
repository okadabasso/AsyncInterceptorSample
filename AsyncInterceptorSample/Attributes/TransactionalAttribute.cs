using System;
using System.Collections.Generic;
using System.Text;

namespace AsyncInterceptorSample.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TransactionalComponentAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Method,AllowMultiple = false)]
    public class TransactionalMethodAttribute : Attribute
    {
    }
}
