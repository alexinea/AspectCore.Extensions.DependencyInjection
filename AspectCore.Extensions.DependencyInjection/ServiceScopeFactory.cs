using Microsoft.Extensions.DependencyInjection;
#if NET451
using AspectCore.DependencyInjection;

#else
using AspectCore.Injector;

#endif

namespace AspectCore.Extensions.DependencyInjection {
    internal class ServiceScopeFactory : IServiceScopeFactory {
        private readonly IServiceResolver _serviceResolver;

        public ServiceScopeFactory(IServiceResolver serviceResolver) {
            _serviceResolver = serviceResolver;
        }

        public IServiceScope CreateScope() {
            return new ServiceScope(_serviceResolver.CreateScope());
        }
    }
}