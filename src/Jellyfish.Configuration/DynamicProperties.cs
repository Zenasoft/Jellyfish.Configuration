// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;

namespace Jellyfish.Configuration
{
    public sealed class DynamicProperties : IDynamicProperties, IDynamicPropertiesUpdater, IDisposable
    {
        public event EventHandler<DynamicPropertyChangedEventArgs> PropertyChanged;

        private static object _sync = new object();
        private static DynamicProperties _instance;

        private PropertiesFactory _factory;
        IPropertiesFactory IDynamicProperties.Factory { get { return _factory; } }
        private ConfigurationManager _configurationManager;
        private Dictionary<string, IDynamicPropertyBase> _properties = new Dictionary<string, IDynamicPropertyBase>(Comparer);
        internal static readonly IEqualityComparer<string> Comparer = StringComparer.OrdinalIgnoreCase;

        public static DynamicProperties Instance { get { if (_instance == null) { Create(); } return _instance; } }

        public static IPropertiesFactory Factory { get { return Instance._factory; } }

        public static IDynamicProperties Create(Microsoft.Framework.Configuration.IConfiguration config=null, int pollingIntervalInSeconds = 60, int sourceTimeoutInMs = 1000)
        {
            if (_instance == null)
            {
                lock (_sync)
                {
                    if (_instance == null)
                    {
                        _instance = new DynamicProperties(pollingIntervalInSeconds, sourceTimeoutInMs);
                        if (config != null)
                        {
                            Instance.RegisterSource(new AspConfigurationSource(config));
                        }
                    }
                }
            }
            return _instance;
        }

        internal DynamicProperties(int pollingIntervalInSeconds = 60, int sourceTimeoutInMs = 1000)
        {
            _factory = new PropertiesFactory(this);
            Reset(pollingIntervalInSeconds, sourceTimeoutInMs);
        }

        public void Reset(int pollingIntervalInSeconds = 60, int sourceTimeoutInMs = 1000)
        {
            _properties = new Dictionary<string, IDynamicPropertyBase>(Comparer);
            _configurationManager = new ConfigurationManager(this, pollingIntervalInSeconds, sourceTimeoutInMs);
        }

        public int PollingIntervalInSeconds
        {
            get { return _configurationManager != null ? _configurationManager.PollingIntervalInSeconds : 0; }
        }

        internal IDictionary<string, IDynamicPropertyBase> Properties { get { return _properties; } }

        internal void OnPropertyChanged(IDynamicPropertyBase property, PropertyChangedAction action)
        {
            var tmp = PropertyChanged;
            if( tmp != null)
            {
                tmp(this, new DynamicPropertyChangedEventArgs(property, action));
            }
        }

        public IDynamicProperties RegisterSource(IConfigurationSource source)
        {
            Contract.Requires(source!=null);
            _configurationManager.RegisterSource(source);
            return this;
        }

        public IDynamicProperty<T> GetProperty<T>(string name)
        {
            Contract.Requires(!String.IsNullOrEmpty(name));

            IDynamicPropertyBase p;
            _properties.TryGetValue(name, out p);
            return p as IDynamicProperty<T>;
        }

        public IDynamicProperty<T> SetProperty<T>(string name, T value)
        {
            Contract.Requires(!String.IsNullOrEmpty(name));
            IDynamicPropertyBase p;
            if (!_properties.TryGetValue(name, out p))
            {
                p = Factory.AsProperty(value, name);
            }
            else
            {
                p.Set(value);
            }
            return p as IDynamicProperty<T>;
        }

        public IDynamicProperty<T> GetOrDefaultProperty<T>(string name, T defaultValue=default(T))
        {
            Contract.Requires(!String.IsNullOrEmpty(name));
            IDynamicPropertyBase p;
            if (_properties.TryGetValue(name, out p))
                return p as IDynamicProperty<T>;
            return _factory.AsProperty(defaultValue, name);
        }

        IDynamicPropertyBase IDynamicPropertiesUpdater.GetOrCreate(string key, Func<IDynamicPropertyBase> factory)
        {
            Contract.Requires(!String.IsNullOrEmpty(key));
            IDynamicPropertyBase prop;

            Dictionary<string, IDynamicPropertyBase> initial;
            Dictionary<string, IDynamicPropertyBase> tmp;
            do
            {
                if (_properties.TryGetValue(key, out prop)) break;

                initial = _properties;
                tmp = new Dictionary<string, IDynamicPropertyBase>(initial);
                prop = factory();
                tmp.Add(key, prop);
            }
            while(Interlocked.CompareExchange(ref _properties, tmp, initial) != initial);

            return prop;
        }

        public void RemoveProperty(string name)
        {
            Contract.Requires(!String.IsNullOrEmpty(name));

            IDynamicPropertyBase p;
            if (_properties.TryGetValue(name, out p))
            {
                _properties.Remove(name);
                OnPropertyChanged(p, PropertyChangedAction.Removed);                
            }
        }

        public void Dispose()
        {
            _configurationManager.Dispose();
            _properties.Clear();
            _factory = null;
        }
    }
}