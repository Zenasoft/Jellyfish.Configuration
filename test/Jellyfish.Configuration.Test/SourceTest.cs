// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Xunit;
using Jellyfish.Configuration.Sources;
using System.Threading;

namespace Jellyfish.Configuration.Tests
{

    public class SourceTest
    {

        [Fact]
        public void SourceDeclarationTest()
        {
            DynamicProperties.Instance = null;
            var properties = (DynamicProperties) DynamicProperties.Init(1);

            var source = new StaticConfigurationSource();
            properties.RegisterSource(source);

            var prop = properties.GetProperty("test");
            Assert.Null(prop);

            source.Set("test", 10);
            prop = properties.GetProperty("test");
            Assert.Null(prop);

            Thread.Sleep(properties.PollingIntervalInSeconds*1100);

            prop = properties.GetProperty("test");
            Assert.NotNull(prop);
            Assert.Equal(10, prop.Value);
        }

        [Fact]
        public void ChainedSourceTest()
        {
            DynamicProperties.Instance = null;
            var properties = DynamicProperties.Init(1);

            var source = new StaticConfigurationSource();
            ((DynamicProperties)properties).RegisterSource(source);

            var chained = properties.Factory.AsChainedProperty("test10", 30,  "test20");
            Assert.Equal(30, properties.Factory.AsChainedProperty("test10", 30,  "test20").Value);

            source.Set("test20", 20);
            Thread.Sleep(properties.PollingIntervalInSeconds * 1200);
            Assert.Equal(20, chained.Value);

            source.Set("test10", 10);
            Thread.Sleep(properties.PollingIntervalInSeconds * 1100);
            Assert.Equal(10, chained.Value);

            source.Set("test10", 11);
            Thread.Sleep(properties.PollingIntervalInSeconds * 1100);
            Assert.Equal(11, chained.Value);
        }


        [Fact]
        public void MultiSourceTest()
        {
            DynamicProperties.Instance = null;
            var properties = (DynamicProperties)DynamicProperties.Init(1);

            var source1 = new StaticConfigurationSource();
            properties.RegisterSource(source1);

            var source2 = new StaticConfigurationSource();
            properties.RegisterSource(source2);
             
            var prop = properties.GetOrCreateProperty<int>("test30");
            Assert.Equal(0, prop.Value);

            source1.Set("test30", 10);
            Thread.Sleep(properties.PollingIntervalInSeconds * 1100);
            Assert.Equal(10, prop.Value);

            source2.Set("test30", 20);
            Thread.Sleep(properties.PollingIntervalInSeconds * 1100);
            Assert.Equal(20, prop.Value);
        }
    }
}
