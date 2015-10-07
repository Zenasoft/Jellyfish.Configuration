// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Framework.Internal;
using System;
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
        public static PollResult Empty = new PollResult(null, new Dictionary<string,object>());

        /// <summary>
        /// Values to update
        /// </summary>
        public IDictionary<string, object> Values { get; private set; }

        public IConfigurationSource Source { get; private set; }

        public PollResult([NotNull]IConfigurationSource source, [NotNull]IDictionary<string, object> values)
        {
            Source = source;
            Values = values;
        }
    }

    /// <summary>
    /// The definition of configuration source that brings dynamic changes to the configuration via polling.
    /// </summary>
    public interface IConfigurationSource
    {
        /// <summary>
        /// Raises when the first properties loading is finished
        /// </summary>
        Action OnInitialized { get;set;}

        /// <summary>
        /// Poll the configuration source to get the latest content.
        /// </summary>
        Task<PollResult> PollProperties([NotNull] CancellationToken token);
    }
}
