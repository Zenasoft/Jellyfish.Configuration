// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Linq;
using System.Reflection;

namespace Jellyfish.Configuration
{
    internal class ConfigurationManager : IDisposable
    {
        private List<IConfigurationSource> sources = new List<IConfigurationSource>();
        private int sourceTimeoutInMs;
        private CancellationTokenSource cancellationToken;
        private IDynamicPropertiesUpdater properties;

        public int PollingIntervalInSeconds { get; internal set; }

        internal ConfigurationManager(IDynamicPropertiesUpdater properties, int pollingIntervalInSeconds = 60, int sourceTimeoutInMs = 1000)
        {
            Contract.Assert(properties != null);
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
                var list = sources;
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
            for(int i=0;i<tasks.Length;i++)
            {
                var props = tasks[i].Result;
                LoadProperties(props);
            }
        }

        private void LoadProperties(PollResult props)
        {
            foreach (var kv in props.Values)
            {
                if (kv.Value == null)
                    continue;

                try {
                    var prop = this.properties.GetOrCreate(kv.Key, () =>
                    {
                        var propertyType = kv.Value.GetType();
                        var dp = typeof(DynamicProperty<>).MakeGenericType(propertyType);
#if DNXCORE50
                        var ctor = dp.GetTypeInfo().DeclaredConstructors.FirstOrDefault(c => {
                            if (c.IsPublic) return false;
                            return c.GetParameters().Length == 2 && c.GetParameters()[0].ParameterType == typeof(DynamicProperties) && c.GetParameters()[1].ParameterType == typeof(string);
                        });
#else
                        var ctor = dp.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
                            null,
                            new Type[] { typeof(DynamicProperties), typeof(string) },
                            null);
#endif
                        var p = (IDynamicProperty)ctor.Invoke(new object[] { properties, kv.Key });
                        return p;

                    });

                    prop.Set(kv.Value);
                }
                catch(Exception) { }
            }
        }

        public void RegisterSource(IConfigurationSource source)
        {
            Contract.Assert(source != null);
            if (sources.Contains(source))
                throw new InvalidOperationException("Duplicate source");

            List<IConfigurationSource> initial;
            List<IConfigurationSource> tmp;

            do
            {
                initial = sources;
                tmp = new List<IConfigurationSource>(initial);
                if (tmp.Contains(source))
                    break;
                tmp.Add(source);
            }
            while (Interlocked.CompareExchange(ref sources, tmp, initial) != initial);

            EnsuresPolling();
        }

        public void Dispose()
        {
            if (cancellationToken != null)
                cancellationToken.Cancel();
        }
    }
}

