using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using MonitorrentClient;
using MonitorrentClient.Models;
using Xamarin.Forms;

namespace MonitorrentMobile.ViewModel
{
    public class TopicViewModel : PropertyChangedBase
    {
        private readonly Topic _topic;
        private readonly IMonitorrentHttpClient _monitorrentHttpClient;

        public TopicViewModel(Topic topic, IMonitorrentHttpClient monitorrentHttpClient, Func<TopicViewModel, Task> deleteTorrent)
        {
            _topic = topic;
            _monitorrentHttpClient = monitorrentHttpClient;
            ExecuteCommand = new Command(async () => await ExecuteTorrent());
            DeleteCommand = new Command(async () => await deleteTorrent(this));
        }

        public string DisplayName
        {
            get => _topic.DisplayName;
            set => _topic.DisplayName = value;
        }

        public int Id
        {
            get => _topic.Id;
            set => _topic.Id = value;
        }

        public Task ExecuteTorrent()
        {
            return _monitorrentHttpClient.ExecuteTopic(new [] {Id});
        }

        public ICommand ExecuteCommand { get; }

        public ICommand DeleteCommand { get; }

        public ImageSource TrackerImage => ImageSource.FromResource($"MonitorrentMobile.Images.{_topic.Tracker}.png");
    }
}