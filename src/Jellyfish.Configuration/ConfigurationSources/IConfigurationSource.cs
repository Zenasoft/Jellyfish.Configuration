// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Jellyfish.Configuration
{
    /// <summary>
    /// This class represents a result from a poll of configuration source
    /// </summary>
    public class PollResult
    {
        /// <summary>
        /// Empty result
        /// </summary>
        public static PollResult Empty = new PollResult(new Dictionary<string,object>());

        /// <summary>
        /// Values to update
        /// </summary>
        public IDictionary<string, object> Values { get; private set; }

        public PollResult(IDictionary<string, object> values)
        {
            Values = values;
        }
    }

    /// <summary>
    /// The definition of configuration source that brings dynamic changes to the configuration via polling.
    /// </summary>
    public interface IConfigurationSource
    {
        /// <summary>
        /// Poll the configuration source to get the latest content.
        /// </summary>
        Task<PollResult> PollProperties( CancellationToken token);
    }
}
