using AUDIO.NET.APP.Shared.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUDIO.NET.APP.Shared
{
    public class ListenerBuilder : IBuildStage, IClientSecretSelectionStage, IClientIdSelectionStage, IRedirectUrlSelectionStage
    {
        string _url;
        string _clientId;
        string _clientSecret;
        ILogger _logger;
        public IBuildStage AndClientSecret(string clientSecret)
        {
            _clientSecret = clientSecret;
            return this;
        }

        public IClientIdSelectionStage UseRedirectUrl(string url)
        {
            _url = url;
            return this;
        }

        public IClientSecretSelectionStage WithClientId(string clientId)
        {
            _clientId = clientId;
            return this;
        }


        public SpotifyListener Build()
        {
            return new SpotifyListener(_url, _clientId, _clientSecret);
        }

    }
}
