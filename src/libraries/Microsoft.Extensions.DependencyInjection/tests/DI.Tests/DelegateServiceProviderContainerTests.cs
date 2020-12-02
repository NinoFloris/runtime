// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Microsoft.Extensions.DependencyInjection.Tests
{
    public abstract class DelegateServiceProviderContainerTests : ServiceProviderContainerTests
    {
        protected  Func<IServiceProvider, IServiceProvider> TransitiveIdentityFactory { get; } =
            sp => sp.GetService<IServiceProvider>();

        protected Func<IServiceProvider, IServiceProvider> CustomFactory { get; } =
            sp => new DelegateServiceProvider(sp);

        private class DelegateServiceProvider : IServiceProvider
        {
            private readonly IServiceProvider _serviceProvider;
            public DelegateServiceProvider(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;
            public object GetService(Type serviceType) => _serviceProvider.GetService(serviceType);
        }
    }

    public class TransitiveIdentityServiceProviderDefaultContainerTests : DelegateServiceProviderContainerTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection collection) =>
            collection.BuildServiceProvider(ServiceProviderMode.Default, new ServiceProviderOptions
                { ServiceProviderFactory = TransitiveIdentityFactory });
    }

    public class TransitiveIdentityServiceProviderDynamicContainerTests : DelegateServiceProviderContainerTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection collection) =>
            collection.BuildServiceProvider(ServiceProviderMode.Dynamic, new ServiceProviderOptions
                { ServiceProviderFactory = TransitiveIdentityFactory });
    }

    public class TransitiveIdentityServiceProviderExpressionsContainerTests : DelegateServiceProviderContainerTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection collection) =>
            collection.BuildServiceProvider(ServiceProviderMode.Expressions, new ServiceProviderOptions
                { ServiceProviderFactory = TransitiveIdentityFactory });
    }

    public class TransitiveIdentityServiceProviderIlEmitContainerTests : DelegateServiceProviderContainerTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection collection) =>
            collection.BuildServiceProvider(ServiceProviderMode.ILEmit, new ServiceProviderOptions
                { ServiceProviderFactory = TransitiveIdentityFactory });
    }

    public class TransitiveIdentityServiceProviderRuntimeContainerTests : DelegateServiceProviderContainerTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection collection) =>
            collection.BuildServiceProvider(ServiceProviderMode.Runtime, new ServiceProviderOptions
                { ServiceProviderFactory = TransitiveIdentityFactory });
    }

    public class CustomServiceProviderDefaultContainerTests : DelegateServiceProviderContainerTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection collection) =>
            collection.BuildServiceProvider(ServiceProviderMode.Default, new ServiceProviderOptions
                { ServiceProviderFactory = CustomFactory });
    }

    public class CustomServiceProviderDynamicContainerTests : DelegateServiceProviderContainerTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection collection) =>
            collection.BuildServiceProvider(ServiceProviderMode.Dynamic, new ServiceProviderOptions
                { ServiceProviderFactory = CustomFactory });
    }

    public class CustomServiceProviderExpressionsContainerTests : DelegateServiceProviderContainerTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection collection) =>
            collection.BuildServiceProvider(ServiceProviderMode.Expressions, new ServiceProviderOptions
                { ServiceProviderFactory = CustomFactory });
    }

    public class CustomServiceProviderIlEmitContainerTests : DelegateServiceProviderContainerTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection collection) =>
            collection.BuildServiceProvider(ServiceProviderMode.ILEmit, new ServiceProviderOptions
                { ServiceProviderFactory = CustomFactory });
    }

    public class CustomServiceProviderRuntimeContainerTests : DelegateServiceProviderContainerTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection collection) =>
            collection.BuildServiceProvider(ServiceProviderMode.Runtime, new ServiceProviderOptions
                { ServiceProviderFactory = CustomFactory });
    }
}
