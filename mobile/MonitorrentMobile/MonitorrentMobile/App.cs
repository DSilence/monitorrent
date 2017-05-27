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

            _container.PerRequest<TopicsViewModel>();
            _container.PerRequest<LoginPageViewModel>();
            _container.PerRequest<MasterPageViewModel>();

            var settings = Settings.Current;

            _container.RegisterInstance(typeof(Settings), null, settings);

            var masterPageViewModel = _container.GetInstance<MasterPageViewModel>();

            _container.RegisterHandler(typeof(IMonitorrentHttpClient), null,
                simpleContainer => ClientFactory.CreateClient(() => masterPageViewModel.SelectPage(masterPageViewModel.LoginPageViewModel), settings.ServerUrl, settings.Token));

            Initialize();

            DisplayRootView<MasterPageView>();
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