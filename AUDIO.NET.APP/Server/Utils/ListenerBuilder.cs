using AUDIO.NET.APP.Server.Services;
using AUDIO.NET.APP.Server.Services.Implementation;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUDIO.NET.APP.Server.Utils.Extensions
{
    public class ListenerBuilder : IBuildStage, IClientSecretSelectionStage, IClientIdSelectionStage, IRedirectUrlSelectionStage
    {
        string _url = String.Empty;
        string _clientId = String.Empty;
        string _clientSecret = String.Empty;
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
