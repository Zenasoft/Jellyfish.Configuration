using Microsoft.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellyfish.Configuration
{
    public class ConfigurationSourceBuilder
    {
        private ConfigurationManager _configurationManager;
        private List<IConfigurationSource> _sources = new List<IConfigurationSource>();

        internal ConfigurationSourceBuilder(ConfigurationManager _configurationManager)
        {
            this._configurationManager = _configurationManager;
        }

        public ConfigurationSourceBuilder AddSource([NotNull]IConfigurationSource source)
        {
            _sources.Add(source);
            return this;
        }

        public ConfigurationSourceBuilder AddRestSource([NotNull]string uri)
        {
            AddSource(new HttpConfigurationSource(uri));
            return this;
        }

        public Task Initialize()
        {
            return _configurationManager.RegisterSources(_sources);
        }
    }
}
