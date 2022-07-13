using MediatR;

namespace AUDIO.NET.APP.Server.Notifications
{
    public class AlbumChangeNotification : INotification
    {
        public string NewColor { get;  }

        public AlbumChangeNotification(string newColor)
        {
            NewColor = newColor;
        }
    }
}
