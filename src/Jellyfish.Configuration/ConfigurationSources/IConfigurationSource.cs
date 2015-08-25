// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Jellyfish.Configuration
{
    public class PollResult
    {
        public static PollResult Empty = new PollResult(new Dictionary<string,object>());

        public IDictionary<string, object> Values { get; private set; }

        public PollResult(IDictionary<string, object> values)
        {
            Values = values;
        }
    }

    public interface IConfigurationSource
    {
        Task<PollResult> LoadProperties( CancellationToken token);
    }
}
