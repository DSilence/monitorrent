using System;
using System.Threading.Tasks;
using System.Windows.Input;
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

        public TopicViewModel(Topic topic)
        {
            _topic = topic;
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

        public async Task ExecuteTorrent()
        {
            int t = 2;
        }

        public async Task DeleteTorrent()
        {
            int t = 2;
        }

        public ICommand ExecuteCommand { get; }

        public ICommand DeleteCommand { get; }

        public ImageSource TrackerImage => ImageSource.FromResource($"MonitorrentMobile.Images.{_topic.Tracker}.png");
    }
}