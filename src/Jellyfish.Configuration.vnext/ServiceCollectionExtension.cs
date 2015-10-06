using Jellyfish.Configuration;
using Microsoft.Framework.Internal;

namespace Microsoft.Framework.DependencyInjection
{
    public static class JellyfishExtensions 
	{    
        public static DynamicPropertiesBuilder UseDynamicProperties(this IServiceCollection services, [NotNull]Microsoft.Framework.Configuration.IConfiguration config)
        { 
            DynamicProperties.Init();
            services.AddSingleton<IDynamicProperties>(s=>DynamicProperties.Instance);
            return new DynamicPropertiesBuilder(DynamicProperties.Instance);
        }
    }
    
    class DynamicPropertiesBuilder 
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
        
        public DynamicPropertiesBuilder WithConfiguration(params IConfiguration[] configurations) {
                        
            _instance.RegisterSource(new AspConfigurationSource(configurations));  
            return this;
        }
                
        public DynamicPropertiesBuilder WithSource(IConfigurationSource source) {
                        
            _instance.RegisterSource(source);  
            return this;
        }
    }
}