using System;
using System.Reflection;
using AspectCore.DynamicProxy;
using AspectCore.Extensions.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AspectCore.Extensions.DependencyInjection {
    public static class ServiceCollectionBuildExtensions {
        [Obsolete("Use BuildAspectInjectorProvider to return AspectCore Injector, or use BuildDynamicProxyServiceProvider to return MSDI ServiceProvider.", true)]
        public static IServiceProvider BuildAspectCoreServiceProvider(this IServiceCollection services) {
            return services.BuildDynamicProxyProvider();
        }

        public static IServiceProvider BuildDynamicProxyProvider(this IServiceCollection services) {
            return services.WeaveDynamicProxyService().BuildServiceProvider();
        }

        public static IServiceProvider BuildDynamicProxyProvider(this IServiceCollection services, bool validateScopes) {
            return services.WeaveDynamicProxyService().BuildServiceProvider(validateScopes);
        }

        public static IServiceCollection WeaveDynamicProxyService(this IServiceCollection services) {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var serviceProvider = services.TryAddDynamicProxyServices().BuildServiceProvider(false);

            var serviceValidator = new ServiceValidator(serviceProvider.GetRequiredService<IAspectValidatorBuilder>());
            var proxyTypeGenerator = serviceProvider.GetRequiredService<IProxyTypeGenerator>();

            IServiceCollection dynamicProxyServices = new ServiceCollection();
            foreach (var service in services) {
                if (serviceValidator.TryValidate(service, out Type implementationType))
                    dynamicProxyServices.Add(MakeProxyService(service, implementationType, proxyTypeGenerator));
                else
                    dynamicProxyServices.Add(service);
            }

            (serviceProvider as IDisposable)?.Dispose();

            return dynamicProxyServices;
        }

        private static ServiceDescriptor MakeProxyService(ServiceDescriptor descriptor, Type implementationType, IProxyTypeGenerator proxyTypeGenerator) {
            var serviceTypeInfo = descriptor.ServiceType.GetTypeInfo();
            if (serviceTypeInfo.IsClass) {
                return ServiceDescriptor.Describe(
                    descriptor.ServiceType,
                    proxyTypeGenerator.CreateClassProxyType(descriptor.ServiceType, implementationType),
                    descriptor.Lifetime);
            }

            if (serviceTypeInfo.IsGenericTypeDefinition) {
                return ServiceDescriptor.Describe(
                    descriptor.ServiceType,
                    proxyTypeGenerator.CreateClassProxyType(implementationType, implementationType),
                    descriptor.Lifetime);
            }

            var proxyType = proxyTypeGenerator.CreateInterfaceProxyType(descriptor.ServiceType, implementationType);
            return ServiceDescriptor.Describe(
                descriptor.ServiceType,
                CreateFactory(descriptor, proxyType),
                descriptor.Lifetime);
        }

        private static Func<IServiceProvider, object> CreateFactory(ServiceDescriptor descriptor, Type proxyType) {
            var proxyConstructor = proxyType.GetTypeInfo().GetConstructor(new Type[] {typeof(IAspectActivatorFactory), descriptor.ServiceType});
            var reflector = proxyConstructor.GetReflector();
            if (descriptor.ImplementationInstance != null) {
                var implementationInstance = descriptor.ImplementationInstance;
                return provider => reflector.Invoke(provider.GetRequiredService<IAspectActivatorFactory>(), implementationInstance);
            }

            if (descriptor.ImplementationFactory != null) {
                var implementationFactory = descriptor.ImplementationFactory;
                return provider => reflector.Invoke(provider.GetRequiredService<IAspectActivatorFactory>(), implementationFactory(provider));
            }

            var implementationType = descriptor.ImplementationType;
            return provider => {
                var aspectActivatorFactory = provider.GetRequiredService<IAspectActivatorFactory>();
                var instance = ActivatorUtilities.CreateInstance(provider, implementationType);
                return reflector.Invoke(aspectActivatorFactory, instance);
            };
        }
    }
}