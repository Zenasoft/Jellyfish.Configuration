// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Jellyfish.Configuration
{
    public enum PropertyChangedAction
    {
        Changed,
        Removed
    }

    /// <summary>
    /// Dynamic property changed event
    /// </summary>
    public class DynamicPropertyChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Current typed property
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <returns></returns>
        public IDynamicProperty<T> GetPropertyAs<T>() { return (IDynamicProperty<T>)Property; }

        /// <summary>
        /// Current property
        /// </summary>
        public IDynamicProperty Property { get; private set; }

        /// <summary>
        /// Get the change action
        /// </summary>
        public PropertyChangedAction Action { get; private set; }

        internal DynamicPropertyChangedEventArgs(IDynamicProperty property, PropertyChangedAction action)
        {
            this.Action = action;
            this.Property = property;
        }
    }
}