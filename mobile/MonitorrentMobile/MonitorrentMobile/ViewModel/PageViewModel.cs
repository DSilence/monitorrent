using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using Xamarin.Forms;

namespace MonitorrentMobile.ViewModel
{
    [ImplementPropertyChanged]
    public class PageViewModel
    {
        public Page TargetPage { get; set; }
        public string Title { get; set; }
        public ImageSource ImageSource { get; set; }
    }
}
