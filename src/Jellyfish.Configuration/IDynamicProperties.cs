// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Jellyfish.Configuration
{
    public interface IDynamicPropertyBase
    {
        void Set(object value);
        object GetValue();
        string Name { get; }
    }

    public interface IDynamicProperty<T> : IDynamicPropertyBase
    {
        T Get();
        void Set(T value);
    }

    public interface IDynamicProperties
    {
        IPropertiesFactory Factory { get; }
        event EventHandler<DynamicPropertyChangedEventArgs> PropertyChanged;
        IDynamicProperty<T> SetProperty<T>(string name, T value);
        void RemoveProperty(string name);
        IDynamicProperty<T> GetProperty<T>(string name);
        IDynamicProperty<T> GetOrDefaultProperty<T>(string name, T defaultValue);
        IDynamicProperties RegisterSource(IConfigurationSource source);
    }
}