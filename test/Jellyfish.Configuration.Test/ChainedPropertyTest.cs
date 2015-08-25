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

            var chained = DynamicProperties.Factory.AsChainedProperty(30, "test", "test1");
            Assert.Equal(30, DynamicProperties.Factory.AsChainedProperty(30, "test", "test1").Get());

            var prop2 = DynamicProperties.Factory.AsProperty(20, "test1");
            Assert.Equal(20, chained.Get());

            chained.Set(40);
            prop2.Set(25);
            Assert.Equal(40, chained.Get());

            var prop = DynamicProperties.Factory.AsProperty(10, "test");
            Assert.Equal(10, chained.Get());


            Assert.Equal(25, DynamicProperties.Factory.AsChainedProperty(30, "??", "test1").Get());
            Assert.Equal(40, DynamicProperties.Factory.AsChainedProperty(40, "??", "???").Get());
        }
    }
}
