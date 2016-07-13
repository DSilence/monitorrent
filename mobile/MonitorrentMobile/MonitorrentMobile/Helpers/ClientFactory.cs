using System;
using Caliburn.Micro.Xamarin.Forms;
using MonitorrentClient;
using MonitorrentMobile.Views;

namespace MonitorrentMobile.Helpers
{
    public class ClientFactory
    {
        private static MonitorrentHttpClient _monitorrentHttpClient;
        private static readonly object _lockObject = new object();
        public static IMonitorrentHttpClient CreateClient(INavigationService navigationService, Uri uri, string existingToken = null)
        {
            if (_monitorrentHttpClient == null)
            {
                lock (_lockObject)
                {
                    if (_monitorrentHttpClient == null)
                    {
                        var newClient = new MonitorrentHttpClient(uri);
                        newClient.Unauthorized += async (sender, args) =>
                        {
                            Settings.Current.IsLoginFailed = true;
                            await navigationService.NavigateToViewAsync<LoginPageView>();
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