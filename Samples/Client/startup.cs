using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;

namespace Jellyfish.Configuration.Sample
{
    public class Startup
    {   
        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            // Setup configuration sources.

            var builder = new ConfigurationBuilder(appEnv.ApplicationBasePath)
                .AddJsonFile("config.json");

            builder.AddEnvironmentVariables();
            
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.UseDynamicProperties()
                    .WithConfiguration(Configuration.GetChildren())
                    .WithRestSource("http://localhost:5000");
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
        }
	}
}