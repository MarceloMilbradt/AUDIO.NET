using AUDIO.NET.APP.Server.Services.Implementation;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUDIO.NET.APP.Server.Services
{
    public interface IRedirectUrlSelectionStage
    {
        IClientIdSelectionStage UseRedirectUrl(string url);
    }

    public interface IClientIdSelectionStage
    {
        IClientSecretSelectionStage WithClientId(string clientId);
    }

    public interface IClientSecretSelectionStage
    {
        IBuildStage AndClientSecret(string clientSecret);
    }

    public interface IBuildStage
    {
        SpotifyListener Build();
    }
}
