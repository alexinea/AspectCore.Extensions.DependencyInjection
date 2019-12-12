using System;
using Microsoft.Extensions.DependencyInjection;
#if NET451
using AspectCore.DependencyInjection;

#else
using AspectCore.Injector;

#endif
namespace AspectCore.Extensions.DependencyInjection {
    internal class SupportRequiredService : ISupportRequiredService {
        private readonly IServiceResolver _serviceResolver;

        public SupportRequiredService(IServiceResolver serviceResolver) {
            _serviceResolver = serviceResolver;
        }

        public object GetRequiredService(Type serviceType) {
            if (serviceType == null) {
                throw new ArgumentNullException(nameof(serviceType));
            }

            return _serviceResolver.ResolveRequired(serviceType);
        }
    }
}