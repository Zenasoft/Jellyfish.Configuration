﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Jellyfish.Configuration
{
    internal class UrlConfigurationSource : AbstractConfigurationSource
    {
        private string uri;

        public override async Task<PollResult> LoadProperties(CancellationToken token)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var resp = await client.SendAsync(request, token);
            if (!resp.IsSuccessStatusCode) return PollResult.Empty;

            var values = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string,object>>(await resp.Content.ReadAsStringAsync());

            FirstTime = false;

            return new PollResult(values);
        }
    }
}