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
        public static Jellyfish.Configuration.DynamicPropertiesBuilder UseDynamicProperties(this IServiceCollection services)
        { 
            DynamicProperties.Init();
            services.AddSingleton<IDynamicProperties>(s=>DynamicProperties.Instance);
            return new Jellyfish.Configuration.DynamicPropertiesBuilder(DynamicProperties.Instance);
        }
    }
}

namespace Jellyfish.Configuration
{
    public class DynamicPropertiesBuilder 
    {
        private IDynamicProperties _instance;
        
        internal DynamicPropertiesBuilder(IDynamicProperties properties) 
        {
            _instance = properties;    
        }

        public DynamicPropertiesBuilder UseJellyfish() {
                        
            _instance.RegisterSource(new UrlConfigurationSource("http://localhost:30100"));  
            return this;
        }
        
        public DynamicPropertiesBuilder WithConfiguration([NotNull]IEnumerable<IConfigurationSection> configurations) {
                        
            _instance.RegisterSource(new AspConfigurationSource(configurations.ToArray()));  
            return this;
        }
         
        public DynamicPropertiesBuilder WithConfiguration([NotNull]IConfigurationRoot configuration) {
                        
            _instance.RegisterSource(new AspConfigurationSource(configuration.GetChildren().ToArray()));  
            return this;
        }
                        
        public DynamicPropertiesBuilder WithSource([NotNull]Jellyfish.Configuration.IConfigurationSource source) {
                        
            _instance.RegisterSource(source);  
            return this;
        }
        
        public DynamicPropertiesBuilder WithRestSource([NotNull]string uri) {
                        
            _instance.RegisterSource(new UrlConfigurationSource(uri));  
            return this;
        }
    
    }
}