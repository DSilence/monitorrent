using System;
using MonitorrentClient;

namespace MonitorrentMobile.Helpers
{
    public class ClientFactory
    {
        private static MonitorrentHttpClient _monitorrentHttpClient;
        private static object _lockObject = new object();
        public static IMonitorrentHttpClient CreateClient(Uri uri, string existingToken = null)
        {
            if (_monitorrentHttpClient == null)
            {
                lock (_lockObject)
                {
                    if (_monitorrentHttpClient == null)
                    {
                        _monitorrentHttpClient = new MonitorrentHttpClient(uri);
                    }
                }
            }
            if (_monitorrentHttpClient.BaseAddress.AbsoluteUri != uri.AbsoluteUri)
            {
                _monitorrentHttpClient.BaseAddress = uri;
            }
            if (existingToken != null)
            {
                _monitorrentHttpClient.Token = existingToken;
            }
            return _monitorrentHttpClient;
        }
    }
}