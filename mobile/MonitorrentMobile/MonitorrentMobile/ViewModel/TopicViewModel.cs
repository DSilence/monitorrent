using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MonitorrentClient;
using MonitorrentClient.Models;
using MonitorrentMobile.Helpers;
using PropertyChanged;
using Xamarin.Forms;

namespace MonitorrentMobile.ViewModel
{
    [ImplementPropertyChanged]
    public class TopicViewModel
    {
        private readonly Topic _topic;
        private readonly IMonitorrentHttpClient _monitorrentHttpClient;

        public TopicViewModel(Topic topic, IMonitorrentHttpClient monitorrentHttpClient)
        {
            _topic = topic;
            _monitorrentHttpClient = monitorrentHttpClient;
            ExecuteCommand = new Command(async () => await ExecuteTorrent());
            DeleteCommand = new Command(async () => await DeleteTorrent());
        }

        public string DisplayName
        {
            get { return _topic.DisplayName; }
            set { _topic.DisplayName = value; }
        }

        public int Id
        {
            get { return _topic.Id; }
            set { _topic.Id = value; }
        }

        public Task ExecuteTorrent()
        {
            return _monitorrentHttpClient.ExecuteTopic(new [] {Id});
        }

        public Task DeleteTorrent()
        {
            return _monitorrentHttpClient.DeleteTopic(Id);
        }

        public ICommand ExecuteCommand { get; }

        public ICommand DeleteCommand { get; }

        public ImageSource TrackerImage => ImageSource.FromResource($"MonitorrentMobile.Images.{_topic.Tracker}.png");
    }
}