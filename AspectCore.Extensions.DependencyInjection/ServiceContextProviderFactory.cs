using System;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
#if NET451
using AspectCore.DependencyInjection;
using IServiceContext = AspectCore.DependencyInjection.IServiceContext;

#else
using AspectCore.Injector;
using IServiceContext = AspectCore.Injector.IServiceContainer;

#endif

namespace AspectCore.Extensions.DependencyInjection {
    [NonAspect]
    public class ServiceContextProviderFactory : IServiceProviderFactory<IServiceContext> {
        public IServiceContext CreateBuilder(IServiceCollection services) {
            return services.ToServiceContext();
        }

        public IServiceProvider CreateServiceProvider(IServiceContext containerBuilder) {
            return containerBuilder.Build();
        }
    }
}