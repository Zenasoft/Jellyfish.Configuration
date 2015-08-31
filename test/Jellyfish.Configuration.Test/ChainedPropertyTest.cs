// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Xunit;

namespace Jellyfish.Configuration.Tests
{
    public class ChainedPropertyTest
    {
    
        [Fact]
        public void ChainedDeclarationTest()
        {
            DynamicProperties.Instance.Reset();

            var chained = DynamicProperties.Factory.AsChainedProperty("test", 30,  "test1");
            Assert.Equal(30, DynamicProperties.Factory.AsChainedProperty("test", 30,  "test1").Value);

            var prop2 = DynamicProperties.Factory.AsProperty(20, "test1");
            Assert.Equal(20, chained.Value);

            chained.Set(40);
            prop2.Set(25);
            Assert.Equal(40, chained.Value);

            var prop = DynamicProperties.Factory.AsProperty(10, "test");
            Assert.Equal(10, chained.Value);


            Assert.Equal(25, DynamicProperties.Factory.AsChainedProperty("??", 30,  "test1").Value);
            Assert.Equal(40, DynamicProperties.Factory.AsChainedProperty("??", 40,  "???").Value);
        }
    }
}
