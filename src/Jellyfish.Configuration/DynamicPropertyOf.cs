// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Jellyfish.Configuration
{
    [System.Diagnostics.DebuggerDisplay("{value}")]
    internal class DynamicProperty<T> : IDynamicProperty<T>
    {
        private T value;
        public string Name { get; private set; }
        private DynamicProperties propertiesManager;

        internal DynamicProperty(DynamicProperties manager, string name)
        {
            this.Name = name;
            this.propertiesManager = manager;
        }

        public T Get()
        {
            return (T)value;
        }

        public void Set(T value)
        {
            if (!Object.Equals(this.value, value))
            {
                this.value = value;
                propertiesManager.OnPropertyChanged(this, PropertyChangedAction.Changed);
            }
        }

        object IDynamicPropertyBase.GetValue()
        {
            return Get();
        }

        void IDynamicPropertyBase.Set(object value)
        {
            this.Set((T)value);
        }
    }
}
