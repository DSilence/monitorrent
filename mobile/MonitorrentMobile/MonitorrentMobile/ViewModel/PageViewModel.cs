using Caliburn.Micro;
using MonitorrentMobile.Helpers;
using Xamarin.Forms;

namespace MonitorrentMobile.ViewModel
{
    public class PageViewModel : PropertyChangedBase
    {
        public Page TargetPage { get; set; }
        public string Title { get; set; }
        public ImageSource ImageSource { get; set; }
    }
}
