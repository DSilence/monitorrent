using Caliburn.Micro;

namespace MonitorrentMobile.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(IoC.Get<MonitorrentMobile.App>());
        }
    }
}
