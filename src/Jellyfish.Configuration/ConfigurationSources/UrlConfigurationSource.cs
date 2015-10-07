// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Jellyfish.Configuration
{
    public class UrlConfigurationSource : AbstractConfigurationSource, IConfigurationSource
    {
        private Uri uri;
        private long? lastUpdate;
        
        public UrlConfigurationSource(string uri)
        {
            this.uri = new Uri(uri);
        }
        
        async Task<PollResult> IConfigurationSource.PollProperties(CancellationToken token)
        {
            var client = new HttpClient();
            var tmp = new UriBuilder(uri);
            if( lastUpdate.HasValue) {
                tmp.Query = $"last {lastUpdate}";
            }
            
            using(var request = new HttpRequestMessage(HttpMethod.Get, tmp.Uri)) 
            {
                var resp = await client.SendAsync(request, token);
                if (!resp.IsSuccessStatusCode) return PollResult.Empty;
    
                var values = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string,object>>(await resp.Content.ReadAsStringAsync());
    
                lastUpdate = DateTime.UtcNow.Ticks;
    
                return new PollResult(values);
            }
        }
    }
}
