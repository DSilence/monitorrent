using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using MonitorrentMobile.ViewModel;
using Xamarin.Forms;

namespace MonitorrentMobile.Views
{
    public partial class MasterPageView : MasterDetailPage
    {
        public MasterPageView()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty DetailPageProperty =
            BindableProperty.CreateAttached("DetailPage", typeof(Page), typeof(MasterPageView), null, BindingMode.OneWay, null, DetailPagePropertyChanged);

        private static void DetailPagePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((MasterPageView)bindable).Detail = (Page)newValue;
        }

        public Page DetailPage
        {
            get { return (Page) GetValue(DetailPageProperty); }
            set
            {
                SetValue(DetailPageProperty, value);
            }
        }

        private void PagesList_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((MasterPageViewModel)this.BindingContext).SelectPage((PageViewModel) e.SelectedItem);
        }
    }
}
