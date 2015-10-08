// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Framework.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using System.Threading;
using System.Threading.Tasks;

namespace Jellyfish.Configuration
{
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
    public sealed class DynamicProperties : IDynamicProperties, IDynamicPropertiesUpdater, IDisposable
    {
        /// <summary>
        /// Raises when a property has changed
        /// </summary>
        public event EventHandler<DynamicPropertyChangedEventArgs> PropertyChanged;

        private static object _sync = new object();
        private static DynamicProperties _instance;
        private PropertiesFactory _factory;
        internal static readonly IEqualityComparer<string> Comparer = StringComparer.OrdinalIgnoreCase;
        // Manage sources and polling
        private ConfigurationManager _configurationManager;
        private ConcurrentDictionary<string, IDynamicProperty> _properties = new ConcurrentDictionary<string, IDynamicProperty>(Comparer);

        /// <summary>
        /// Get the dynamic properties factory
        /// </summary>
        IPropertiesFactory IDynamicProperties.Factory { get { return _factory; } }

        /// <summary>
        /// Properties factory used to create new property from a value
        /// </summary>
        public static IPropertiesFactory Factory { get { return Instance._factory; } }

        /// <summary>
        /// Get a singleton instance
        /// </summary>
        public static DynamicProperties Instance {
            get
            {
                if (_instance == null) { Init(); }
                return _instance;
            }
            internal set // for test only
            {
                _instance = null;
            }
        }

        /// <summary>
        /// Initialise dynamic properties configuration. Can be call only once and before any call to DynamicProperties.Instance.
        /// </summary>
        /// <param name="pollingIntervalInSeconds">Polling interval in seconds (default 60)</param>
        /// <param name="sourceTimeoutInMs">Max time allowed to a source to retrieve new values (Cancel the request but doesn't raise an error)</param>
        /// <returns>Current DynamicProperties instance</returns>
        public static IDynamicProperties Init(int pollingIntervalInSeconds = 60, int sourceTimeoutInMs = 1000)
        {
            if (_instance == null)
            {
                lock (_sync)
                {
                    if (_instance == null)
                    {
                        _instance = new DynamicProperties(pollingIntervalInSeconds, sourceTimeoutInMs);
                    }
                }
                return _instance;
            }

            throw new Exception("Can not initialize an active instance. Use Reset().");
        }

        internal void AddProperty(string name, IDynamicProperty prop)
        {
            _properties.TryAdd(name, prop);
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        /// <param name="pollingIntervalInSeconds"></param>
        /// <param name="sourceTimeoutInMs"></param>
        private DynamicProperties(int pollingIntervalInSeconds = 60, int sourceTimeoutInMs = 1000)
        {
            _factory = new PropertiesFactory(this);
            Reset(pollingIntervalInSeconds, sourceTimeoutInMs);
        }

        /// <summary>
        /// Reset configuration and properties. 
        /// All current properties will be invalid and all current sources will be lost.
        /// </summary>
        /// <param name="pollingIntervalInSeconds"></param>
        /// <param name="sourceTimeoutInMs"></param>
        public void Reset(int pollingIntervalInSeconds = 60, int sourceTimeoutInMs = 1000)
        {
            var tmp = _properties;
            var tmp2 = _configurationManager;

            Interlocked.Exchange(ref _properties, new ConcurrentDictionary<string, IDynamicProperty>(Comparer));
            Interlocked.Exchange(ref _configurationManager, new ConfigurationManager(this, pollingIntervalInSeconds, sourceTimeoutInMs));

            if (tmp != null)
            {
                foreach (var prop in tmp.Values)
                {
                    prop.Dispose();
                }
                tmp.Clear();
            }

            if( tmp2 != null)
                tmp2.Dispose();
        }

        /// <summary>
        /// Get the polling time before updates
        /// </summary>
        public int PollingIntervalInSeconds
        {
            get { return _configurationManager != null ? _configurationManager.PollingIntervalInSeconds : 0; }
        }


        internal void OnPropertyChanged(IDynamicProperty property, PropertyChangedAction action)
        {
            var tmp = PropertyChanged;
            if( tmp != null)
            {
                tmp(this, new DynamicPropertyChangedEventArgs(property, action));
            }
        }

        /// <summary>
        /// Add a new configuration source for polling
        /// </summary>
        /// <param name="source">A new configuration source</param>
        /// <returns></returns>
        public ConfigurationSourceBuilder WithSources()
        {
            return new ConfigurationSourceBuilder(Instance._configurationManager);
        }


        // for tests only
        internal void RegisterSource(IConfigurationSource source)
        {
            Instance._configurationManager.RegisterSources(new IConfigurationSource[] { source }).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get a property or null if not exists
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="name">Property name</param>
        /// <returns>A dynamic property instance or null if not exists.</returns>
        public IDynamicProperty<T> GetProperty<T>([NotNull]string name)
        {
            IDynamicProperty p;
            _properties.TryGetValue(name, out p);

            // TODO error?? if p is not of type T
            return p as IDynamicProperty<T>;
        }

        /// <summary>
        /// Create or update a property with a value
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="name">Property name</param>
        /// <param name="value">Default value</param>
        /// <returns>A dynamic property instance</returns>
        public IDynamicProperty<T> CreateOrUpdateProperty<T>([NotNull]string name, T value)
        {
            IDynamicProperty p;
            if (!_properties.TryGetValue(name, out p))
            {
                p = _factory.AsProperty(value, name);
            }
            else
            {
                p.Set(value);
            }
            return p as IDynamicProperty<T>;
        }

        /// <summary>
        /// Get a property or create a new one with a default value if not exists
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="name">Property name</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>A dynamic property instance</returns>
        public IDynamicProperty<T> GetOrCreateProperty<T>([NotNull]string name, T defaultValue=default(T))
        {
            IDynamicProperty p;
            if (_properties.TryGetValue(name, out p))
                return p as IDynamicProperty<T>;
            return _factory.AsProperty(defaultValue, name);
        }

        IDynamicProperty IDynamicPropertiesUpdater.GetOrCreate([NotNull]string key, Func<IDynamicProperty> factory)
        {
            IDynamicProperty prop = _properties.GetOrAdd(key, k => factory());
            return prop;
        }

        void IDynamicPropertiesUpdater.RemoveProperty([NotNull]string name)
        {
            IDynamicProperty p;
            if (_properties.TryRemove(name, out p))
            {
                OnPropertyChanged(p, PropertyChangedAction.Removed);                
            }
        }

        public void Dispose()
        {
            foreach (var prop in _properties.Values)
            {
                prop.Dispose();
            }
            _configurationManager.Dispose();
            _properties.Clear();
            _factory = null;
        }
    }
}