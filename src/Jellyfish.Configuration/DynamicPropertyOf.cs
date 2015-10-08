// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Jellyfish.Configuration
{
    [System.Diagnostics.DebuggerDisplay("{value}")]
    internal class DynamicProperty : IDynamicProperty
    {
        private object value;
        private DynamicProperties propertiesManager;
        private bool disposed = false;

        internal DynamicProperty(DynamicProperties manager, string name)
        {
            this.Name = name;
            this.propertiesManager = manager;
        }

        /// <summary>
        /// Property name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Current value
        /// </summary>
        public T ValueAs<T>()
        {
            if (disposed) throw new ObjectDisposedException("Can not use a disposed property. Do you have call DynamicProperties.Reset() ?");
            return (T)value;
        }

        /// <summary>
        /// Update local property value. This value can be overrided by a <see cref="IConfigurationSource"/>. 
        /// Doesn't update source values.
        /// </summary>
        /// <param name="value">Property value</param>
        public void Set(object value)
        {
            if (disposed) throw new ObjectDisposedException("Can not use a disposed property. Do you have call DynamicProperties.Reset() ?");
            if (!Object.Equals(this.value, value))
            {
                this.value = value;
                propertiesManager.OnPropertyChanged(this, PropertyChangedAction.Changed);
            }
        }

        public object Value
        {
            get
            {
                return this.value;
            }
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            disposed = true;
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }

        public override string ToString()
        {
            return value != null ? value.ToString() : String.Empty;
        }
        #endregion
    }
}
