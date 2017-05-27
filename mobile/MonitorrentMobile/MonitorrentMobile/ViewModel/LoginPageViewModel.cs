using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using MonitorrentMobile.Helpers;
using MonitorrentMobile.Views;
using Action = System.Action;

namespace MonitorrentMobile.ViewModel
{
    public class LoginPageViewModel: Screen
    {
        private readonly Settings _settings;

        public LoginPageViewModel(Settings settings)
        {
            _settings = settings;
            ServerUrl = _settings.ServerUrl != null ? _settings.ServerUrl.AbsoluteUri : "";
        }

        public string ServerUrl { get; set; }

        public string Password { get; set; }

        public bool HasFailed
        {
            get => _settings.IsLoginFailed || string.IsNullOrEmpty(Settings.Current.Token); set => _settings.IsLoginFailed = value;
        }

        public MasterPageViewModel MasterPageViewModel { get; set; }

        public async Task DoLogin()
        {
            HasFailed = false;
            var uri = new Uri(ServerUrl);
            _settings.ServerUrl = uri;

            var token = await ClientFactory.CreateClient(() => MasterPageViewModel.SelectPage(MasterPageViewModel.LoginPageViewModel), uri).Login(Password);
            if (token != null)
            {
                Settings.Current.Token = token;
                MasterPageViewModel.SelectPage(MasterPageViewModel.TopicsPageViewModel);
            }
            else
            {
                HasFailed = true;
            }
        }

        public void DoLogout()
        {
            Settings.Current.Token = string.Empty;
            HasFailed = true;
        }
    }
}