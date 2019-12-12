using System;
using Microsoft.Extensions.DependencyInjection;
#if NET451
using AspectCore.DependencyInjection;

#else
using AspectCore.Injector;

#endif

namespace AspectCore.Extensions.DependencyInjection {
    internal class MsdiScopeResolverFactory : IScopeResolverFactory {
        private readonly IServiceProvider _serviceProvider;

        public MsdiScopeResolverFactory(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        public IServiceResolver CreateScope() {
            return new MsdiServiceResolver(_serviceProvider.CreateScope().ServiceProvider);
        }
    }
}