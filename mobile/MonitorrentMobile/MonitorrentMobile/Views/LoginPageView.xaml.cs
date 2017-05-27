using MonitorrentMobile.Converters;
using Xamarin.Forms;

namespace MonitorrentMobile.Views
{
    public partial class LoginPageView : ContentPage
    {
        public LoginPageView()
        {
            InitializeComponent();
            LogoutButton.SetBinding(IsVisibleProperty, "IsFailed", BindingMode.OneWay, new InvertedBoolenConverter());
        }
    }
}
