using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using MonitorrentMobile.Helpers;
using Xamarin.Forms;

namespace MonitorrentMobile.ViewModel
{
    public class MasterPageViewModel : PropertyChangedBase
    {
        public MasterPageViewModel(TopicsViewModel topicsViewModel, LoginPageViewModel loginPageViewModel)
        {
            topicsViewModel.MasterPageViewModel = this;
            loginPageViewModel.MasterPageViewModel = this;
            var mainPage = (Page)ViewLocator.LocateForModel(topicsViewModel, null, null);
            mainPage.BindingContext = topicsViewModel;
            var settingsPage = (Page) ViewLocator.LocateForModel(loginPageViewModel, null, null);
            settingsPage.BindingContext = loginPageViewModel;
            TopicsPageViewModel = new PageViewModel
            {
                ImageSource = ImageSource.FromResource("MonitorrentMobile.Images.ic_list_black_24dp.png"),
                TargetPage = mainPage,
                Title = "Topics",
            };
            LoginPageViewModel = new PageViewModel
            {
                ImageSource = ImageSource.FromResource("MonitorrentMobile.Images.ic_settings_black_24dp.png"),
                TargetPage = settingsPage,
                Title = "Settings",
            };
            Pages = new ObservableCollection<PageViewModel>(new[]
            {
                TopicsPageViewModel,
                LoginPageViewModel
            });
            Detail = loginPageViewModel.HasFailed ? settingsPage : mainPage;
        }

        public ObservableCollection<PageViewModel> Pages { get; set; }
        public Page Detail { get; set; }
        public PageViewModel TopicsPageViewModel { get; set; }
        public PageViewModel LoginPageViewModel { get; set; }

        public void SelectPage(PageViewModel pageViewModel)
        {
            Detail = pageViewModel.TargetPage;
        }
    }
}
