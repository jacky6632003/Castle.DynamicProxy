using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Castle.DynamicProxy.Infrastructure
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