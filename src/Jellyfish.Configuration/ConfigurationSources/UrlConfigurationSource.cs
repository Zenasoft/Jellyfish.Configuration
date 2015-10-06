using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Jellyfish.Configuration
{
    public class UrlConfigurationSource : AbstractConfigurationSource, IConfigurationSource
    {
        private string uri;

        public UrlConfigurationSource(string uri)
        {
            this.uri = uri;
        }

        async Task<PollResult> IConfigurationSource.PollProperties(CancellationToken token)
        {
            var client = new HttpClient();

            using(var request = new HttpRequestMessage(HttpMethod.Get, uri)) 
            {
                var resp = await client.SendAsync(request, token);
                if (!resp.IsSuccessStatusCode) return PollResult.Empty;
    
                var values = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string,object>>(await resp.Content.ReadAsStringAsync());
    
                FirstTime = false;
    
                return new PollResult(values);
            }
        }
    }
}
