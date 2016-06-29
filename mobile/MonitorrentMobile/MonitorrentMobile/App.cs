using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using MonitorrentClient;
using MonitorrentMobile.Helpers;
using MonitorrentMobile.ViewModel;
using MonitorrentMobile.Views;
using Xamarin.Forms;

namespace MonitorrentMobile
{
    public class App: FormsApplication
    {
        private readonly SimpleContainer _container;

        public App(SimpleContainer container)
        {
            var config = new TypeMappingConfiguration()
            {
                DefaultSubNamespaceForViewModels = "MonitorrentMobile.ViewModel",
                DefaultSubNamespaceForViews = "MonitorrentMobile.Views",
            };

            ViewLocator.ConfigureTypeMappings(config);
            ViewModelLocator.ConfigureTypeMappings(config);

            _container = container;

            _container.Activated += ContainerOnActivated;

            _container.PerRequest<MainPageViewModel>();
            _container.PerRequest<LoginPageViewModel>();

            var settings = Settings.Current;

            _container.RegisterInstance(typeof(Settings), null, settings);

            _container.RegisterHandler(typeof(IMonitorrentHttpClient), null,
                simpleContainer => ClientFactory.CreateClient(settings.ServerUrl, settings.Token));

            Initialize();

            if (Settings.Current.ServerUrl == null
                || string.IsNullOrEmpty(Settings.Current.Token))
            {
                DisplayRootView<LoginPageView>();
            }
            else
            {
                DisplayRootView<MainPageView>();
            }
        }

        private async void ContainerOnActivated(object o)
        {
            var initializable = o as IInitializable;
            if (initializable != null)
            {
                await initializable.Initialize();
            }
        }

        protected override void PrepareViewFirst(NavigationPage navigationPage)
        {
            _container.Instance<INavigationService>(new NavigationPageAdapter(navigationPage));
        }
    }
}