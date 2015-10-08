// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Jellyfish.Configuration
{
    /// <summary>
    /// Read configuration from a HTTP request.
    /// Request contains the last call datetime (UTC ticks)
    /// Request must send a json object with the following format
    /// {
    ///   "property name" : {type="Int64", value=nnn},
    /// or
    ///   "property name" : value
    /// }
    /// For the latter, the data type will be infered by the json serializer. 
    /// BE CAREFULL with numeric type which will be always of type Int64, this can provide erronus cast if you try to read with a 
    /// GetProperty<int> for exemple. So it's recommended to always set the type for an integer.
    /// 
    /// </summary>
    public class HttpConfigurationSource : AbstractConfigurationSource, IConfigurationSource
    {
        private Uri uri;
        private long? lastUpdate;
        
        public HttpConfigurationSource(string uri)
        {
            this.uri = new Uri(uri);
        }
        
        async Task<PollResult> IConfigurationSource.PollProperties(CancellationToken token)
        {
            var client = new HttpClient();
            var tmp = new UriBuilder(uri);
            if( lastUpdate.HasValue) {
                tmp.Query = $"last={lastUpdate}";
            }
            
            using(var request = new HttpRequestMessage(HttpMethod.Get, tmp.Uri)) 
            {
                var resp = await client.SendAsync(request, token);
                if (!resp.IsSuccessStatusCode) return PollResult.Empty;
    
                var values = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string,object>>(await resp.Content.ReadAsStringAsync());
                var keys = values.Keys.ToArray();

                foreach (var key in keys)
                {
                    var obj = values[key] as JObject;

                    if (obj != null)
                    {
                        var type = obj.Property("type");
                        var val = obj.Property("value");
                        if (type?.Value != null && val != null)
                        {
                            try
                            {
                                var str = type.Value.ToString();
                                if (!str.StartsWith("System."))
                                    str = "System." + str;
                                var t = Type.GetType(str);
                                var v = Convert.ChangeType(val.Value.ToString(), t);
                                values[key] = v;
                                continue;
                            }
                            catch
                            {
                            }
                        }

                        values.Remove(key);
                    }
                }


                lastUpdate = DateTime.UtcNow.Ticks;
    
                return new PollResult(this, values);
            }
        }
    }
}
