// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Jellyfish.Configuration
{
    public abstract class AbstractConfigurationSource : IConfigurationSource
    {
        protected bool FirstTime { get; set; } = true;
        protected static Task<PollResult> EmptyResult = Task.FromResult(PollResult.Empty);

        public abstract Task<PollResult> LoadProperties(CancellationToken token);

        protected object ConvertValue(string value)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject(value);
            }
            catch
            {
                return value;
            }
        }
    }
}
