// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.Extensions.DependencyInjection.ServiceLookup;

namespace Microsoft.Extensions.DependencyInjection.Tests
{
    internal static class ServiceCollectionContainerBuilderTestExtensions
    {
        public static ServiceProvider BuildServiceProvider(this IServiceCollection services, ServiceProviderMode mode, ServiceProviderOptions options = null)
        {
            var opts = options ?? ServiceProviderOptions.Default;
            if (mode == ServiceProviderMode.Default)
            {
                return services.BuildServiceProvider(opts);
            }

            IServiceProviderEngine engine = mode switch
            {
                ServiceProviderMode.Dynamic => new DynamicServiceProviderEngine(services, opts.ServiceProviderFactory),
                ServiceProviderMode.Runtime => new RuntimeServiceProviderEngine(services, opts.ServiceProviderFactory),
                ServiceProviderMode.Expressions => new ExpressionsServiceProviderEngine(services, opts.ServiceProviderFactory),
                ServiceProviderMode.ILEmit => new ILEmitServiceProviderEngine(services, opts.ServiceProviderFactory),
                _ => throw new NotSupportedException()
            };

            return new ServiceProvider(services, engine, opts);
        }
    }
}
