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
    public class AspectCoreServiceProviderFactory : IServiceProviderFactory<IServiceContext> {
        public IServiceContext CreateBuilder(IServiceCollection services) {
            return services.ToServiceContainer();
        }

        public IServiceProvider CreateServiceProvider(IServiceContext containerBuilder) {
            return containerBuilder.Build();
        }
    }
}