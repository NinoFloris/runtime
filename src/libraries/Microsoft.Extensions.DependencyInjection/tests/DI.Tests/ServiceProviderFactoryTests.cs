// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection.Specification.Fakes;
using Xunit;

namespace Microsoft.Extensions.DependencyInjection.Tests
{
    public abstract class ServiceProviderContainerDecoratorTests
    {
         protected abstract IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection, ServiceProviderOptions options);

         protected IServiceProvider CreateServiceProvider(IServiceCollection collection) =>
             CreateServiceProvider(collection, new ServiceProviderOptions { ServiceProviderFactory = sp => new DelegateServiceProvider(sp) });

        [Fact]
        public void IndirectServiceProviderIsDelegated()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<ClassWithServiceProvider>();
            serviceCollection.AddTransient<IFakeService, FakeService>();

            var provider = CreateServiceProvider(serviceCollection);

            Assert.Null(provider.GetService<ClassWithServiceProvider>().ServiceProvider.GetService<IFakeService>());
        }

        [Fact]
        public void ImplementationFactoryServiceProviderIsDelegated()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient(sp => new ClassWithServiceProvider(sp));
            serviceCollection.AddTransient<IFakeService, FakeService>();

            var provider = CreateServiceProvider(serviceCollection);

            Assert.Null(provider.GetService<ClassWithServiceProvider>().ServiceProvider.GetService<IFakeService>());
        }

        [Fact]
        public void ServiceProviderIsDelegated()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IFakeService, FakeService>();

            var provider = CreateServiceProvider(serviceCollection);

            Assert.Null(provider.GetService<IServiceProvider>().GetService<IFakeService>());
        }

        [Fact]
        public void TransitiveServiceProviderIsDelegated()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IFakeService, FakeService>();

            var provider = CreateServiceProvider(serviceCollection);

            Assert.Null(provider.GetService<IServiceProvider>().GetService<IServiceProvider>().GetService<IFakeService>());
        }

        [Fact]
        public void ServiceScopeFactoryIsDelegated()
        {
            var serviceCollection = new ServiceCollection();

            var provider = CreateServiceProvider(serviceCollection);
            var scopeFactory = provider.GetService<IServiceScopeFactory>();

            using (scopeFactory.CreateScope())
            using (scopeFactory.CreateScope())
            {
                Assert.IsType<DelegateServiceScopeFactory>(scopeFactory);
                Assert.Equal(2, ((DelegateServiceScopeFactory)scopeFactory).ScopesActive);
            }

            Assert.Equal(0, ((DelegateServiceScopeFactory)scopeFactory).ScopesActive);
        }

        private class DelegateServiceScopeFactory : IServiceScopeFactory
        {
            readonly IServiceScopeFactory _scopeFactory;
            long _scopesActive = 0;

            public DelegateServiceScopeFactory(IServiceScopeFactory scopeFactory)
            {
                _scopeFactory = scopeFactory;
            }

            public long ScopesActive => _scopesActive;

            public IServiceScope CreateScope() => new DelegatedServiceScope(this, _scopeFactory.CreateScope());

            private class DelegatedServiceScope: IServiceScope
            {
                readonly DelegateServiceScopeFactory _factory;
                readonly IServiceScope _scope;

                public DelegatedServiceScope(DelegateServiceScopeFactory factory, IServiceScope scope)
                {
                    _factory = factory;
                    _scope = scope;
                    Interlocked.Increment(ref _factory._scopesActive);
                }

                public void Dispose()
                {
                    _scope.Dispose();
                    Interlocked.Decrement(ref _factory._scopesActive);
                }

                public IServiceProvider ServiceProvider => _scope.ServiceProvider;
            }
        }

        private class DelegateServiceProvider : IServiceProvider
        {
            private readonly IServiceProvider _serviceProvider;

            public DelegateServiceProvider(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
                if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider), "interesting");
            }

            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(IFakeService)) return null;
                if (serviceType == typeof(IServiceScopeFactory))
                    return new DelegateServiceScopeFactory(_serviceProvider.GetService<IServiceScopeFactory>());

                return _serviceProvider.GetService(serviceType);
            }
        }
    }

    public class DelegateServiceProviderDefaultContainerTests : DelegateServiceProviderContainerTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection collection) =>
            collection.BuildServiceProvider(ServiceProviderMode.Default, new ServiceProviderOptions
                { ServiceProviderFactory = TransitiveIdentityFactory });
    }

    public class DelegateServiceProviderDynamicContainerTests : DelegateServiceProviderContainerTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection collection) =>
            collection.BuildServiceProvider(ServiceProviderMode.Dynamic, new ServiceProviderOptions
                { ServiceProviderFactory = TransitiveIdentityFactory });
    }

    public class DelegateServiceProviderExpressionsContainerTests : DelegateServiceProviderContainerTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection collection) =>
            collection.BuildServiceProvider(ServiceProviderMode.Expressions, new ServiceProviderOptions
                { ServiceProviderFactory = TransitiveIdentityFactory });
    }

    public class DelegateServiceProviderILEmitContainerTests : DelegateServiceProviderContainerTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection collection) =>
            collection.BuildServiceProvider(ServiceProviderMode.ILEmit, new ServiceProviderOptions
                { ServiceProviderFactory = TransitiveIdentityFactory });
    }

    public class DelegateServiceProviderRuntimeContainerTests : DelegateServiceProviderContainerTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection collection) =>
            collection.BuildServiceProvider(ServiceProviderMode.Runtime, new ServiceProviderOptions
                { ServiceProviderFactory = TransitiveIdentityFactory });
    }
}
