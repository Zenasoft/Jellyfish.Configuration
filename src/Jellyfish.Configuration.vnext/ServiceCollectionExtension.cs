// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Jellyfish.Configuration;
using Microsoft.Framework.Configuration;
using System.Linq;
using Microsoft.Framework.Internal;

namespace Microsoft.Framework.DependencyInjection
{
    public static class JellyfishExtensions 
	{    
        public static ConfigurationSourceBuilder UseDynamicProperties(this IServiceCollection services)
        { 
            DynamicProperties.Init();
            services.AddSingleton<IDynamicProperties>(s=>DynamicProperties.Instance);
            return DynamicProperties.Instance.WithSources();
        }
    }
}

namespace Jellyfish.Configuration
{
    public static class DynamicPropertiesBuilderExtensions 
    {
        public static ConfigurationSourceBuilder UseJellyfish(this ConfigurationSourceBuilder builder) {
                        
            builder.AddSource(new HttpConfigurationSource("http://localhost:30100"));  
            return builder;
        }
        
        public static ConfigurationSourceBuilder WithConfiguration(this ConfigurationSourceBuilder builder, [NotNull]IEnumerable<IConfigurationSection> configurations) {

            builder.AddSource(new AspConfigurationSource(configurations.ToArray()));  
            return builder;
        }
         
        public static ConfigurationSourceBuilder WithConfiguration(this ConfigurationSourceBuilder builder, [NotNull]IConfigurationRoot configuration) {

            builder.AddSource(new AspConfigurationSource(configuration.GetChildren().ToArray()));  
            return builder;
        }                              
    }
}