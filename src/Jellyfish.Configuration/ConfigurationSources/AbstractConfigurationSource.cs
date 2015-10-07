// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Framework.Internal;

namespace Jellyfish.Configuration
{
    public abstract class AbstractConfigurationSource 
    {
        protected bool FirstTime { get; set; } = true;
        public Action OnInitialized { get; set; }

        protected static Task<PollResult> EmptyResult = Task.FromResult(PollResult.Empty);

        protected object ConvertJsonValue(string value)
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
