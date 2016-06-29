using System;
using MonitorrentClient;
using MonitorrentMobile.Helpers;
using MonitorrentMobile.ViewModel;
using Xamarin.Forms;

namespace MonitorrentMobile.Views
{
    public partial class MainPageView : ContentPage
    {
        public MainPageView()
        {
            InitializeComponent();
        }

        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //Do not allow item selection
            ((ListView)sender).SelectedItem = null;
        }
    }
}