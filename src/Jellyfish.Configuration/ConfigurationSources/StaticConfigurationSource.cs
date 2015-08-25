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
    public class StaticConfigurationSource : AbstractConfigurationSource
    {
        private ConcurrentDictionary<string, object> _values = new ConcurrentDictionary<string, object>();

        public void Set(string name, object value)
        {
            Contract.Requires(!String.IsNullOrEmpty(name));
            _values.AddOrUpdate(name, value, (_,__) => value);
        }

        public override Task<PollResult> LoadProperties(CancellationToken token)
        {
            var t = Task.FromResult( new PollResult( new Dictionary<string,object>( _values)));
            _values.Clear();
            return t;
        }
    }
}
