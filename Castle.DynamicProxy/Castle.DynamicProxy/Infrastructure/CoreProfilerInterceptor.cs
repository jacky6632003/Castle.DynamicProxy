using CoreProfiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Castle.DynamicProxy.Infrastructure
{
    public class CoreProfilerInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            // 只攔截特定 Attribute
            //if (!(Attribute.GetCustomAttribute(invocation.Method, typeof(UseCoreFilerAttribute)) is UseCoreFilerAttribute
            //    auditLogAttribute))
            //{
            //    invocation.Proceed();
            //    return;
            //}
            var stepName = $"{invocation.TargetType.Name}-{invocation.Method.Name}";
            using (ProfilingSession.Current.Step(stepName))
            {
                invocation.Proceed();
            }
        }
    }
}