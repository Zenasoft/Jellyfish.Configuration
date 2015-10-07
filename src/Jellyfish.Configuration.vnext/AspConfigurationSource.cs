// Copyright(c) Zenasoft.All rights reserved.
//Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Framework.Configuration;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Jellyfish.Configuration
{
    internal class AspConfigurationSource : AbstractConfigurationSource, IConfigurationSource
    {
        private IConfigurationSection[] _configurations;

        public AspConfigurationSource(IConfigurationSection[] configurations)
        {
            this._configurations = configurations;
        }

        public Task<PollResult> PollProperties(CancellationToken token)
        {
            if (!FirstTime) return EmptyResult;
            FirstTime = false;

            var result = new PollResult(this,
                new Dictionary<string, object>(_configurations.ToDictionary(
                    v => v.Key.Replace(':', '.'), 
                    v => ConvertJsonValue(v.Value)))
                    );

            var t = Task.FromResult(result);
            return t;
        }
    }
}
