using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
#if NET451
using AspectCore.DependencyInjection;
using IServiceContext = AspectCore.DependencyInjection.IServiceContext;
using ServiceContext = AspectCore.DependencyInjection.ServiceContext;

#else
using AspectCore.Injector;
using IServiceContext = AspectCore.Injector.IServiceContainer;
using ServiceContext = AspectCore.Injector.ServiceContainer;

#endif

namespace AspectCore.Extensions.DependencyInjection {
    public static class ServiceCollectionToContainerExtensions {
        public static IServiceContext ToServiceContainer(this IServiceCollection services) {
            if (services == null) {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddScoped<ISupportRequiredService, SupportRequiredService>();
            services.AddScoped<IServiceScopeFactory, ServiceScopeFactory>();
            return new ServiceContext(services.AddAspectCoreContainer().Select(Replace));
        }

        public static IServiceCollection AddAspectCoreContainer(this IServiceCollection services) {
            if (services == null) {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddSingleton<IServiceProviderFactory<IServiceContext>, AspectCoreServiceProviderFactory>();
            return services;
        }

        private static ServiceDefinition Replace(ServiceDescriptor descriptor) {
            if (descriptor.ImplementationType != null) {
                return new TypeServiceDefinition(descriptor.ServiceType, descriptor.ImplementationType, GetLifetime(descriptor.Lifetime));
            }
            else if (descriptor.ImplementationInstance != null) {
                return new InstanceServiceDefinition(descriptor.ServiceType, descriptor.ImplementationInstance);
            }
            else {
                return new DelegateServiceDefinition(descriptor.ServiceType, resolver => descriptor.ImplementationFactory(resolver), GetLifetime(descriptor.Lifetime));
            }
        }

        private static Lifetime GetLifetime(ServiceLifetime serviceLifetime) {
            switch (serviceLifetime) {
                case ServiceLifetime.Scoped:
                    return Lifetime.Scoped;
                case ServiceLifetime.Singleton:
                    return Lifetime.Singleton;
                default:
                    return Lifetime.Transient;
            }
        }
    }
}