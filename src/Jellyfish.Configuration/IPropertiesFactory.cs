// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Jellyfish.Configuration
{
    public interface IPropertiesFactory
    {
        IDynamicProperty<T> AsProperty<T>(T value, string name=null);
        IDynamicProperty<T> AsChainedProperty<T>( T defaultValue = default(T), params string[] properties);
    }
}