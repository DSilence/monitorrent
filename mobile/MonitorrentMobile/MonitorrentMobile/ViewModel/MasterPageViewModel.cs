using System.Collections.ObjectModel;
using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using Xamarin.Forms;

namespace MonitorrentMobile.ViewModel
{
    public class MasterPageViewModel : PropertyChangedBase
    {
        public MasterPageViewModel(TopicsViewModel topicsViewModel, LoginPageViewModel loginPageViewModel)
        {
            TopicsViewModel = topicsViewModel;
            var mainPage = (Page)ViewLocator.LocateForModel(topicsViewModel, null, null);
            mainPage.BindingContext = topicsViewModel;
            var settingsPage = (Page) ViewLocator.LocateForModel(loginPageViewModel, null, null);
            settingsPage.BindingContext = loginPageViewModel;
            Pages = new ObservableCollection<PageViewModel>(new[]
            {
                new PageViewModel
                {
                    ImageSource = ImageSource.FromResource("MonitorrentMobile.Images.ic_list_black_24dp.png"),
                    TargetPage = mainPage,
                    Title = "Topics"
                },
                new PageViewModel
                {
                    ImageSource = ImageSource.FromResource("MonitorrentMobile.Images.ic_settings_black_24dp.png"),
                    TargetPage = settingsPage,
                    Title = "Settings"
                },
            });
            Detail = mainPage;
        }
        public ObservableCollection<PageViewModel> Pages { get; set; }
        public Page Detail { get; set; }
        public TopicsViewModel TopicsViewModel { get; set; }

        public void SelectPage(PageViewModel pageViewModel)
        {
            Detail = pageViewModel.TargetPage;
        }
    }
}
