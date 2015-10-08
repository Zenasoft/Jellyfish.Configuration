// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit;
using System.Globalization;

namespace Jellyfish.Configuration.Tests
{
    public class DynamicPropertiesTest
    {
        [Fact]
        public void EmptyDynamicPropertyTest()
        {
            DynamicProperties.Instance = null;

            Assert.Null(DynamicProperties.Instance.GetProperty("test"));
            var p = DynamicProperties.Instance.GetOrCreateProperty<int>("test");
            Assert.Equal(0, p.Value);
        }

        [Fact]
        public void PropertyDeclarationTest()
        {
            DynamicProperties.Instance = null;

            var prop = DynamicProperties.Factory.AsProperty(10, "test");
            Assert.NotNull(prop);
            Assert.Equal(10, prop.Value);
            var p = DynamicProperties.Instance.GetOrCreateProperty<int>("test");
            Assert.Equal(10, p.Value);
            var prop2 = DynamicProperties.Instance.GetProperty("test");
            Assert.NotNull(prop2);
            Assert.Equal(prop.Value, prop2.Value);
        }

        [Fact]
        public void DuplicateTest()
        {
            DynamicProperties.Instance = null;

            var prop = DynamicProperties.Factory.AsProperty(10, "test");
            Assert.Throws<ArgumentException>(() =>
           {
               var prop2 = DynamicProperties.Factory.AsProperty(10, "test");
           });
        }

        [Fact]
        public void PropertyChangedTest()
        {
            DynamicProperties.Instance = null;

            var cx = 0;
            DynamicProperties.Instance.PropertyChanged += (s, e) => { if (e.Property.Name == "test") cx += e.Property.ValueAs<int>(); };

            var prop = DynamicProperties.Factory.AsProperty(10, "test");
            prop.Set(15);
            var prop2 = DynamicProperties.Factory.AsProperty(10, "test2");
            prop.Set(20);
            Assert.Equal(10 + 15 + 20, cx);
            Assert.Equal(20, prop.Value);
        }

        [Fact]
        public void MultiTypesTest()
        {
            DynamicProperties.Instance = null;

            Assert.Equal(10, DynamicProperties.Factory.AsProperty(10, "test").Value);
            Assert.Equal(2.0, DynamicProperties.Factory.AsProperty(2.0, "test2").Value);
            Assert.Equal("xxx", DynamicProperties.Factory.AsProperty("xxx", "test3").Value);
            Assert.Equal(true, DynamicProperties.Factory.AsProperty(true, "test4").Value);
            var v = new CultureInfo("fr");
            Assert.Equal(v, DynamicProperties.Factory.AsProperty(v, "test5").Value);
            var v2 = new int[] { 1, 2, 3 };
            Assert.Equal(v2, DynamicProperties.Factory.AsProperty(v2, "test6").Value);
        }
    }
}
