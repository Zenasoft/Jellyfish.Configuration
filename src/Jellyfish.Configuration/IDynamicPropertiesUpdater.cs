// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Jellyfish.Configuration
{
    /// <summary>
    /// Interface used by <see cref="ConfigurationManager"/> to update properties
    /// </summary>
    internal interface IDynamicPropertiesUpdater
    {
        /// <summary>
        /// Remove an unused property
        /// </summary>
        /// <param name="name"></param>
        void RemoveProperty(string name);

        /// <summary>
        /// Get or create a property
        /// </summary>
        /// <param name="key">Property name</param>
        /// <param name="factory">Property factory</param>
        /// <returns></returns>
        IDynamicProperty GetOrCreate(string key, Func<IDynamicProperty> factory);
    }
}