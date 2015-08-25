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
            DynamicProperties.Instance.Reset();

            var source = new StaticConfigurationSource();
            DynamicProperties.Instance.PollingIntervalInSeconds = 1;
            DynamicProperties.Instance.RegisterSource(source);

            var prop = DynamicProperties.Instance.GetProperty<int>("test");
            Assert.Null(prop);

            source.Set("test", 10);
            prop = DynamicProperties.Instance.GetProperty<int>("test");
            Assert.Null(prop);

            Thread.Sleep(DynamicProperties.Instance.PollingIntervalInSeconds*1000);
            prop = DynamicProperties.Instance.GetProperty<int>("test");
            Assert.NotNull(prop);
            Assert.Equal(10, prop.Get());
        }

        [Fact]
        public void ChainedSourceTest()
        {
            DynamicProperties.Instance.Reset();

            var source = new StaticConfigurationSource();
            DynamicProperties.Instance.PollingIntervalInSeconds = 1;
            DynamicProperties.Instance.RegisterSource(source);

            var chained = DynamicProperties.Factory.AsChainedProperty(30, "test10", "test20");
            Assert.Equal(30, DynamicProperties.Factory.AsChainedProperty(30, "test10", "test20").Get());

            source.Set("test20", 20);
            Thread.Sleep(DynamicProperties.Instance.PollingIntervalInSeconds * 1100);
            Assert.Equal(20, chained.Get());

            source.Set("test10", 10);
            Thread.Sleep(DynamicProperties.Instance.PollingIntervalInSeconds * 1100);
            Assert.Equal(10, chained.Get());

            source.Set("test10", 11);
            Thread.Sleep(DynamicProperties.Instance.PollingIntervalInSeconds * 1100);
            Assert.Equal(11, chained.Get());
        }


        [Fact]
        public void MultiSourceTest()
        {
            DynamicProperties.Instance.Reset();

            DynamicProperties.Instance.PollingIntervalInSeconds = 1;

            var source1 = new StaticConfigurationSource();
            DynamicProperties.Instance.RegisterSource(source1);

            var source2 = new StaticConfigurationSource();
            DynamicProperties.Instance.RegisterSource(source2);

            var prop = DynamicProperties.Instance.GetOrDefaultProperty<int>("test30");
            Assert.Equal(0, prop.Get());

            source1.Set("test30", 10);
            Thread.Sleep(DynamicProperties.Instance.PollingIntervalInSeconds * 1100);
            Assert.Equal(10, prop.Get());

            source2.Set("test30", 20);
            Thread.Sleep(DynamicProperties.Instance.PollingIntervalInSeconds * 1100);
            Assert.Equal(20, prop.Get());
        }
    }
}
