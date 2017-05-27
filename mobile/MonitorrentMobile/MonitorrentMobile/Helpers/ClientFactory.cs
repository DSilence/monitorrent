using System;
using Caliburn.Micro.Xamarin.Forms;
using MonitorrentClient;
using MonitorrentMobile.Views;
using Action = System.Action;

namespace MonitorrentMobile.Helpers
{
    public class ClientFactory
    {
        private static MonitorrentHttpClient _monitorrentHttpClient;
        private static readonly object LockObject = new object();
        public static IMonitorrentHttpClient CreateClient(Action navigationAction, Uri uri, string existingToken = null)
        {
            if (_monitorrentHttpClient == null)
            {
                lock (LockObject)
                {
                    if (_monitorrentHttpClient == null)
                    {
                        var newClient = new MonitorrentHttpClient(uri);
                        newClient.Unauthorized += (sender, args) =>
                        {
                            Settings.Current.IsLoginFailed = true;
                            navigationAction();
                        };
                        _monitorrentHttpClient = newClient;
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