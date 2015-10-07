// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace Jellyfish.Configuration
{
    public abstract class AbstractConfigurationSource 
    {
        protected bool FirstTime { get; set; } = true;
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
