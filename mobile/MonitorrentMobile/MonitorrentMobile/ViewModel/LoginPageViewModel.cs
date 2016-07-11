using System;
using System.Threading.Tasks;
using Caliburn.Micro.Xamarin.Forms;
using MonitorrentMobile.Helpers;
using MonitorrentMobile.Views;
using PropertyChanged;
using Xamarin.Forms;

namespace MonitorrentMobile.ViewModel
{
    [ImplementPropertyChanged]
    public class LoginPageViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly Settings _settings;

        public LoginPageViewModel(INavigationService navigationService, Settings settings)
        {
            _navigationService = navigationService;
            _settings = settings;
            ServerUrl = _settings.ServerUrl != null ? _settings.ServerUrl.AbsoluteUri : "";
        }

        public string ServerUrl { get; set; }

        public string Password { get; set; }

        public async Task DoLogin()
        {
            var uri = new Uri(ServerUrl);
            _settings.ServerUrl = uri;

            var token = await ClientFactory.CreateClient(uri).Login(Password);
            if (token != null)
            {
                Settings.Current.Token = token;

                await _navigationService.NavigateToViewAsync<MainPageView>();
            }
        }
    }
}