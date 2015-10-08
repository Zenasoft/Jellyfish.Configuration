// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Framework.Internal;
using System;


namespace Jellyfish.Configuration
{
    internal class PropertiesFactory : IPropertiesFactory
    {
        private DynamicProperties _properties;

        internal PropertiesFactory([NotNull]DynamicProperties dynamicProperties)
        {
            this._properties = dynamicProperties;
        }

        private void Add([NotNull]string name, [NotNull]IDynamicProperty prop)
        {
            _properties.AddProperty(name, prop);
        }

        public IDynamicProperty AsProperty<T>(T value, string name = null)
        {
            if(name != null && _properties.GetProperty(name) != null) { 
                throw new ArgumentException("Duplicate property name");
            }
            
            var p = new DynamicProperty(_properties, name);
            if (!String.IsNullOrEmpty(name))
                Add(name, p);
            p.Set(value);
            return p;
        }

        public IDynamicProperty AsChainedProperty<T>([NotNull]string name, T defaultValue = default(T), params string[] fallbackPropertyNames)
        {
            if (fallbackPropertyNames.Length == 0) throw new ArgumentException("You must provide at least on efallback property name");

            var properties = new string[fallbackPropertyNames.Length + 1];
            properties[0] = name;
            Array.Copy(fallbackPropertyNames, 0, properties, 1, fallbackPropertyNames.Length);
            var p = new ChainedDynamicProperty(_properties, defaultValue, properties);
            return p;
        }
    }
}