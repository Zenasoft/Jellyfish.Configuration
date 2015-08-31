// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Jellyfish.Configuration.Sources
{
    /// <summary>
    /// Static configuration source
    /// </summary>
    public class StaticConfigurationSource : IConfigurationSource
    {
        private ConcurrentDictionary<string, object> _values = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// Set a update a new property
        /// </summary>
        /// <param name="name">Property name</param>
        /// <param name="value">Property value</param>
        public void Set(string name, object value)
        {
            Contract.Requires(!String.IsNullOrEmpty(name));
            _values.AddOrUpdate(name, value, (_,__) => value);
        }

        Task<PollResult> IConfigurationSource.PollProperties(CancellationToken token)
        {
            var t = Task.FromResult( new PollResult( new Dictionary<string,object>( _values)));
            _values.Clear();
            return t;
        }
    }
}
