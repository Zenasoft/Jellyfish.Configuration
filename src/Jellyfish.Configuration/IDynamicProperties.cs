// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Framework.Internal;
using System;

namespace Jellyfish.Configuration
{
    /// <summary>
    /// A dynamic property 
    /// </summary>
    public interface IDynamicProperty : IDisposable
    {
        /// <summary>
        /// Update local property value. This value can be overrided by a <see cref="IConfigurationSource"/>. 
        /// Doesn't update source values.
        /// </summary>
        /// <param name="value">Property value</param>
        void Set(object value);
        /// <summary>
        /// Get typed value
        /// </summary>
        T ValueAs<T>();
        /// <summary>
        /// Get value
        /// </summary>
        object Value { get; }
        /// <summary>
        /// Property name
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Provides dynamic properties updating when the config is changed.
    /// Accessing a dynamic property is very fast and thread safe. The last value is cached and updated on the fly from a <see cref="IConfigurationSource"/> at fixed interval.
    /// Updates are made using polling requests on a list of sources.
    /// <para>
    /// Dynamic properties are read only. You can set a value but it will be valid only as a default value.
    /// </para>
    /// <para>
    /// DynamicProperty objects are not subject to normal garbage collection.
    /// They should be used only as a static value that lives for the
    /// lifetime of the program.
    /// </para>
    /// <code>
    /// var i = DynamicProperties.Instance.GetProperty<int>("prop1");
    /// var i2 = DynamicProperties.Instance.GetOrDefaultProperty<int>("prop1", 1);
    /// </code>
    /// </summary>
    public interface IDynamicProperties
    {
        /// <summary>
        /// Get the dynamic properties factory
        /// </summary>
        IPropertiesFactory Factory { get; }

        /// <summary>
        /// Get the polling time before updates
        /// </summary>
        int PollingIntervalInSeconds { get; }

        /// <summary>
        /// Raises when a property has changed
        /// </summary>
        event EventHandler<DynamicPropertyChangedEventArgs> PropertyChanged;

        /// <summary>
        /// Create or update a property with a value
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="name">Property name</param>
        /// <param name="value">Default value</param>
        /// <returns>A dynamic property instance</returns
        IDynamicProperty CreateOrUpdateProperty<T>([NotNull]string name, T value);

        /// <summary>
        /// Get a property or null if not exists
        /// </summary>
        /// <param name="name">Property name</param>
        /// <returns>A dynamic property instance or null if not exists.</returns>
        IDynamicProperty GetProperty([NotNull]string name);

        /// <summary>
        /// Get a property or create a new one with a default value if not exists
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="name">Property name</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>A dynamic property instance</returns>
        IDynamicProperty GetOrCreateProperty<T>([NotNull]string name, T defaultValue);

        /// <summary>
        /// Used to add source configuration
        /// </summary>
        /// <returns></returns>
        ConfigurationSourceBuilder WithSources();
    }
}