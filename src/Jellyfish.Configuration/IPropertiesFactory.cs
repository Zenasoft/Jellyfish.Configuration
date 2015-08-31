// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Jellyfish.Configuration
{
    /// <summary>
    /// Provides methods to create a property
    /// </summary>
    public interface IPropertiesFactory
    {
        /// <summary>
        /// Create a new dynamic property
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="value">Default value</param>
        /// <param name="name">Property name</param>
        /// <returns>A dynamic property instance</returns>
        IDynamicProperty<T> AsProperty<T>(T value, string name=null);
        /// <summary>
        /// Create a new chained dynamic property
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="propertyName">Main property name</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="fallbackPropertyNames">List of properties to chain. The first is the main property</param>
        /// <returns>A dynamic property instance</returns>
        IDynamicProperty<T> AsChainedProperty<T>(string propertyName, T defaultValue = default(T), params string[] fallbackPropertyNames);
    }
}