// Copyright(c) Zenasoft.All rights reserved.
//Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Framework.Configuration;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Jellyfish.Configuration
{
    internal class AspConfigurationSource : AbstractConfigurationSource, IConfigurationSource
    {
        private IConfiguration _root;

        public AspConfigurationSource(IConfiguration root)
        {
            this._root = root;
        }

        public Task<PollResult> PollProperties(CancellationToken token)
        {
            if (!FirstTime) return EmptyResult;
            FirstTime = false;

            var result =
                 new PollResult(new Dictionary<string, object>(_root.GetChildren().ToDictionary(v => v.Key.Replace(':', '.'), v => ConvertJsonValue(v.Value))));

            var t = Task.FromResult(result);
            return t;
        }
    }
}
