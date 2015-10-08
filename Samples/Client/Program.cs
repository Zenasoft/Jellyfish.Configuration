using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using System;

namespace Jellyfish.Configuration.Sample
{
    public class Program
    {
        public static void Main()
        {
            DynamicProperties.Init(pollingIntervalInSeconds: 10)
                .WithSources()
                .AddSource(new HttpConfigurationSource("http://localhost:5000"))
                .Initialize();

            var prop1 = DynamicProperties.Instance.GetOrCreateProperty<bool>("sample.prop1");
            var prop2 = DynamicProperties.Instance.GetOrCreateProperty<int>("sample.prop2");
            var prop3 = DynamicProperties.Instance.GetOrCreateProperty<string>("sample.prop3");
            var prop4 = DynamicProperties.Factory.AsProperty(50L);

            // Add "sample.prop5": 20 in the properties.json file on the server side
            var prop5 = DynamicProperties.Factory.AsChainedProperty("sample.prop5", 0L, "sample.prop2");

            do
            {
                Console.WriteLine($"prop1={prop1}");
                Console.WriteLine($"prop2={prop2}");
                Console.WriteLine($"prop3={prop3}");
                Console.WriteLine($"prop4={prop4}");
                Console.WriteLine($"prop5={prop5}");
                Console.ReadKey();
            }
            while (true);
        }
    }
}