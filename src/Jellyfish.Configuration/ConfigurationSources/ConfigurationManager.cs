// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Framework.Internal;


namespace Jellyfish.Configuration
{
    internal class ConfigurationManager : IDisposable
    {
        private List<IConfigurationSource> _sources = new List<IConfigurationSource>();
        private int sourceTimeoutInMs;
        private CancellationTokenSource cancellationToken;
        private IDynamicPropertiesUpdater properties;

        public int PollingIntervalInSeconds { get; internal set; }

        internal ConfigurationManager([NotNull]IDynamicPropertiesUpdater properties, int pollingIntervalInSeconds = 60, int sourceTimeoutInMs = 1000)
        {
            this.sourceTimeoutInMs = sourceTimeoutInMs;
            this.PollingIntervalInSeconds = pollingIntervalInSeconds;
            this.sourceTimeoutInMs = sourceTimeoutInMs;
            this.properties = properties;
        }

        private void EnsuresPolling()
        {
            if (Interlocked.CompareExchange(ref cancellationToken, new CancellationTokenSource(), null) == null)
            {
                Task.Factory.StartNew(Polling, 
                                cancellationToken.Token, 
                                TaskCreationOptions.DenyChildAttach|TaskCreationOptions.LongRunning, 
                                TaskScheduler.Default);
            }
        }

        private async Task Polling()
        {
            while (true)
            {
                var list = _sources;
                var tasks = new Task<PollResult>[list.Count];

                try
                {
                    try
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        for (int i = 0; i < list.Count; i++)
                        {
                            tasks[i] = list[i].PollProperties(cancellationToken.Token);
                        }

                        if (!Task.WaitAll(tasks, sourceTimeoutInMs, cancellationToken.Token))
                        {
                            // log : one task is timeout
                        }
                    }
                    catch (Exception)
                    {
                        // log
                    }

                    LoadPropertiesFromSources(tasks);

                    await Task.Delay(PollingIntervalInSeconds * 1000, cancellationToken.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private void LoadPropertiesFromSources(Task<PollResult>[] tasks)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                var result = tasks[i].Result;
                LoadProperties(result);

                if (result.Source?.OnInitialized != null)
                {
                    result.Source.OnInitialized();
                    result.Source.OnInitialized = null;
                }
            }
        }

        private void LoadProperties(PollResult props)
        {
            foreach (var kv in props.Values)
            {
                var val = kv.Value as string;
                if( kv.Value == null || val == "$delete")
                {
                    this.properties.RemoveProperty(kv.Key);
                    continue;
                }
                
                try {
                    var prop = this.properties.GetOrCreate(kv.Key, () =>
                    {
                        var p =new DynamicProperty((DynamicProperties)properties, kv.Key);
                        return p;
                    });

                    prop.Set(kv.Value);
                }
                catch(Exception) {
                    // TODO log
                }
            }
        }

        public Task RegisterSources(IEnumerable<IConfigurationSource> sources)
        {
            List<IConfigurationSource> initial;
            List<IConfigurationSource> tmp;
            var latch = new Latch();

            do
            {
                initial = _sources;
                tmp = new List<IConfigurationSource>(initial);
                foreach (var source in sources)
                {
                    if (tmp.Contains(source))
                        break;
                    source.OnInitialized = () => latch.Decrement();
                    tmp.Add(source);
                    latch.Increment();
                }
            }
            while (Interlocked.CompareExchange(ref _sources, tmp, initial) != initial);

            EnsuresPolling();

            return latch.Task;
        }

        public void Dispose()
        {
            if (cancellationToken != null)
                cancellationToken.Cancel();
        }

        // Thanks to jon skeet
        class Latch
        {
            private int count = 0;
            private readonly TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            public void Increment()
            {
                Interlocked.Increment(ref count);
            }

            public void Decrement()
            {
                if (Interlocked.Decrement(ref count) == 0)
                {
                    tcs.TrySetResult(null);
                }
            }

            public Task Task
            {
                get { return tcs.Task; }
            }
        }
    }
}

