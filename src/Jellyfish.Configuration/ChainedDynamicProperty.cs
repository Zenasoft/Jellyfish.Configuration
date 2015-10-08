// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;

namespace Jellyfish.Configuration
{
    /// <summary>
    /// Create a chained property composed with a dynamic property and fallback properties used if the main property is not defined.
    /// A chained property works with fallback values. If the first is not defined, the value is founded in the first property values defined in the fallback list
    /// and then the default value.
    /// </summary>
    /// <typeparam name="T">Property type</typeparam>
    [System.Diagnostics.DebuggerDisplay("{Value}")]
    public class ChainedDynamicProperty : IDynamicProperty
    {
        private string[] _fallbackProperties;
        private IDynamicProperty _activeProperty;
        private DynamicProperties _propertiesManager;
        private object _defaultValue;
        private bool disposed = false;

        string IDynamicProperty.Name
        {
            get
            {
                return _fallbackProperties[0];
            }
        }

        internal ChainedDynamicProperty(DynamicProperties manager, object defaultValue, params string[] properties)
        {
            if(properties.Length < 2) throw new ArgumentException("You must provided at least 2 properties.");
            _propertiesManager = manager;
            _fallbackProperties = properties.ToArray();
            _defaultValue = defaultValue;
            _propertiesManager.PropertyChanged += OnPropertyChanged;

            Reset();
        }

        protected void Reset()
        {
            IDynamicProperty tmp = null;
            foreach(var propertyName in _fallbackProperties)
            {
                tmp = _propertiesManager.GetProperty(propertyName);
                if(tmp != null)
                {
                    break;
                }
            }
            Interlocked.Exchange(ref _activeProperty, tmp);
        }

        private void OnPropertyChanged(object sender, DynamicPropertyChangedEventArgs e)
        {
            if (_fallbackProperties.Contains(e.Property.Name, DynamicProperties.Comparer))
            {
                Reset();
            }
        }

        /// <summary>
        /// Current value
        /// </summary>
        public T ValueAs<T>()
        {
            if (disposed) throw new ObjectDisposedException("Can not use a disposed property. Do you have call DynamicProperties.Reset() ?");
            return (T)Value;
        }

        public object Value
        {
            get
            {
                return (_activeProperty != null ? _activeProperty.Value : _defaultValue);
            }
        }

        /// <summary>
        /// Update default property value. This value can be overrided by a <see cref="IConfigurationSource"/>. 
        /// Doesn't update source values.
        /// Assigning a value as precedence on all overriding properties
        /// Only the main property has precedence so others are ignored
        /// </summary>
        /// <param name="value">Property value</param>
        public void Set(object value)
        {
            if (disposed) throw new ObjectDisposedException("Can not use a disposed property. Do you have call DynamicProperties.Reset() ?");

            _defaultValue = value;
            // Assigning a value as precedence on all overriding properties
            // Only the main property (the first) has precedence so ignore other one
            var tmp = _fallbackProperties.Take(1).ToArray();
            Interlocked.Exchange(ref _fallbackProperties, tmp);
            Reset();
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
             disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public override string ToString()
        {
            return Value != null ? Value.ToString() : String.Empty;
        }
        #endregion
    }
}
