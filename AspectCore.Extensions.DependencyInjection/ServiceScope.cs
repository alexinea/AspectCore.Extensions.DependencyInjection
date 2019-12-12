using System;
using Microsoft.Extensions.DependencyInjection;
#if NET451
using AspectCore.DependencyInjection;

#else
using AspectCore.Injector;

#endif

namespace AspectCore.Extensions.DependencyInjection {
    internal class ServiceScope : IServiceScope {
        private readonly IServiceResolver _serviceResolver;
        public IServiceProvider ServiceProvider => _serviceResolver;

        public ServiceScope(IServiceResolver serviceResolver) {
            _serviceResolver = serviceResolver;
        }

        public void Dispose() {
            _serviceResolver.Dispose();
        }
    }
}