// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.Contracts;

namespace Jellyfish.Configuration
{
    internal class PropertiesFactory : IPropertiesFactory
    {
        private DynamicProperties _properties;

        internal PropertiesFactory(DynamicProperties dynamicProperties)
        {
            Contract.Requires(dynamicProperties != null);
            this._properties = dynamicProperties;
        }

        private void Add<T>(string name, IDynamicProperty<T> prop)
        {
            Contract.Requires(!String.IsNullOrEmpty(name));
            Contract.Requires(prop != null);

            _properties.Properties.Add(name, prop);
        }

        public IDynamicProperty<T> AsProperty<T>(T value, string name = null)
        {
            var p = new DynamicProperty<T>(_properties, name);
            if (!String.IsNullOrEmpty(name))
                Add(name, p);
            p.Set(value);
            return p;
        }

        public IDynamicProperty<T> AsChainedProperty<T>(string name, T defaultValue = default(T), params string[] fallbackPropertyNames)
        {
            Contract.Requires(!String.IsNullOrEmpty(name));
            Contract.Requires(fallbackPropertyNames.Length > 0, "You must provide at least on efallback property name");

            var properties = new string[fallbackPropertyNames.Length + 1];
            properties[0] = name;
            Array.Copy(fallbackPropertyNames, 0, properties, 1, fallbackPropertyNames.Length);
            var p = new ChainedDynamicProperty<T>(_properties, defaultValue, properties);
            return p;
        }
    }
}