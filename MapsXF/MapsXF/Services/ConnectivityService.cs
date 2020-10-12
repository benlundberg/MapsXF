using MapsXF.Core;
using Xamarin.Essentials;

namespace MapsXF
{
    public class ConnectivityService : IConnectivityService
    {
        public bool IsConnected => Connectivity.NetworkAccess != NetworkAccess.None;
    }
}
