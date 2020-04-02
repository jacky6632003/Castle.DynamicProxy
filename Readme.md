# AOP 動態 Interceptor - 使用 Castle.DynamicProxy

>優點：較簡單地實現AOP功能，不用修改原本的class或增加Decorator pattern的class
>
>缺點：在產生物件時，效能會有影響


### 範例專案
[ASP.NET Core 3.1 - g6519 Training-2020-02](https://github.evertrust.com.tw/g6519/Training-2020-02)

### 需安裝的 NuGet Packages
- Castle.Core 

### 建立攔截器類別 - 範例 CoreFiler

**CoreProfilerInterceptor.cs**

```csharp
using Castle.DynamicProxy;
using CoreProfiler;

namespace NorthwindWebCore.Controllers.Infrastructure.Interceptors
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
```

### 在Startup.cs - ConfigureServices() 註冊攔截器

```csharp
            services.AddScoped<IUsersService>(provider =>
            {
                IUsersService usersService = provider.GetService(typeof(UsersService)) as UsersService;

                var generator = new ProxyGenerator();
                var interceptor = provider.GetService(typeof(CoreProfilerInterceptor)) as IInterceptor;
                var proxy = generator.CreateInterfaceProxyWithTarget(usersService, interceptor);

                return proxy;
            });
```
或使用Helper簡化註冊
```csharp
            services.AddInterfaceProxy<IUsersRepository, UsersRepository>(typeof(CoreProfilerInterceptor));
            services.AddInterfaceProxy<IUsersService, UsersService>(typeof(CoreProfilerInterceptor));
```
並加入InterceptorHelper.cs
```csharp
using System;
using System.Linq;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

namespace NorthwindWebCore.Controllers.Infrastructure.Helpers
{
    public static class InterceptorHelper
    {
        public static IServiceCollection AddInterfaceProxy<TService, TImplementation>(
            this IServiceCollection services, params Type[] interceptorTypes)
            where TService : class
            where TImplementation : TService
        {
            services.AddScoped<TService>(provider =>
            {
                var interfaceOfService = provider.GetService(typeof(TImplementation)) as TService;

                var interceptors = interceptorTypes
                    .Select(interceptorType => provider.GetService(interceptorType) as IInterceptor)
                    .ToArray();

                var generator = new ProxyGenerator();
                var proxy = generator.CreateInterfaceProxyWithTarget(interfaceOfService, interceptors);

                return proxy;
            });

            return services;
        }
    }
}
```

**注意事項**
>所有方法皆會被攔截，可以新增Attribute，掛在需要被攔截的方法上。並在攔截器上加上Attribute的判斷。

**UseCoreFilerAttribute.cs**

```csharp
using System;

namespace NorthwindWebCore.Controllers.Infrastructure.Interceptors
{
    public class UseCoreFilerAttribute : Attribute
    {        
    }
}
```


**參考資料**

[AOP In 91](https://dotblogs.com.tw/hatelove/Tags?qq=AOP)

[Castle DynamicProxy基本用法](https://www.itread01.com/content/1559459043.html)

[Autofac.Extras.DynamicProxy 中 EnableInterfaceInterceptors() 及 EnableClassInterceptors()](https://dotblogs.com.tw/supershowwei/2017/04/29/121550)
