// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;

namespace Jellyfish.Configuration
{
    /// <summary>
    /// Create a chained property composed with a dynamic property and fallback properties used if the main property is not defined.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Diagnostics.DebuggerDisplay("{Get()}")]
    public class ChainedDynamicProperty<T> : IDynamicProperty<T>
    {
        private string[] _properties;
        private IDynamicProperty<T> _activeProperty;
        private DynamicProperties _propertiesManager;
        private T _defaultValue;

        string IDynamicPropertyBase.Name
        {
            get
            {
                return _properties[0];
            }
        }

        object IDynamicPropertyBase.GetValue()
        {
            return Get();
        }

        internal ChainedDynamicProperty(DynamicProperties manager, T defaultValue = default(T), params string[] properties)
        {
            Contract.Requires(properties.Length > 2, "You must provided at least 2 properties.");
            _propertiesManager = manager;
            _properties = properties.ToArray();
            _defaultValue = defaultValue;
            _propertiesManager.PropertyChanged += OnPropertyChanged;

            Reset();
        }

        protected void Reset()
        {
            IDynamicProperty<T> tmp = null;
            foreach(var propertyName in _properties)
            {
                tmp = _propertiesManager.GetProperty<T>(propertyName);
                if(tmp != null)
                {
                    break;
                }
            }
            Interlocked.Exchange(ref _activeProperty, tmp);
        }

        private void OnPropertyChanged(object sender, DynamicPropertyChangedEventArgs e)
        {
            if (_properties.Contains(e.Property.Name, DynamicProperties.Comparer))
            {
                Reset();
            }
        }

        public T Get()
        {
            return _activeProperty != null ? _activeProperty.Get() : _defaultValue;
        }

        public void Set(T value)
        {
            _defaultValue = value;
            // Assigning a value as precedence on all overriding properties
            // Only the main property (the first) has precedence so ignore other one
            var tmp = _properties.Take(1).ToArray();
            Interlocked.Exchange(ref _properties, tmp);
            Reset();
        }

        public void Set(object value)
        {
            this.Set((T)value);
        }
    }
}
