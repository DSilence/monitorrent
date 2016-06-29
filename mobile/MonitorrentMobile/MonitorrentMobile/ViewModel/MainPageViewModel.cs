using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitorrentClient;
using MonitorrentClient.Models;
using MonitorrentMobile.Helpers;
using Plugin.Settings.Abstractions;
using PropertyChanged;
using Xamarin.Forms;

namespace MonitorrentMobile.ViewModel
{
    [ImplementPropertyChanged]
    public class MainPageViewModel : IInitializable
    {
        private readonly IMonitorrentHttpClient _monitorrentHttpClient;

        public MainPageViewModel(IMonitorrentHttpClient monitorrentHttpClient)
        {
            _monitorrentHttpClient = monitorrentHttpClient;
        }

        public bool Loading { get; set; }
        public ObservableCollection<TopicViewModel> Topics { get; set; }

        public async Task Execute()
        {
            string temp = "Placeholder";
        }

        public async Task Initialize()
        {
            Loading = true;
            try
            {
                var topics = await _monitorrentHttpClient.GetTopics();
                Topics =
                    new ObservableCollection<TopicViewModel>(topics.Select(x => new TopicViewModel(x)));
            }
            catch (Exception e)
            {
                //TODO handle
            }
            finally
            {
                Loading = false;
            }
        }
    }
}
