using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using MonitorrentClient;
using MonitorrentClient.Models;
using MonitorrentMobile.Annotations;
using MonitorrentMobile.Enums;
using Xamarin.Forms;

namespace MonitorrentMobile.ViewModel
{
    public class MainPageViewModel : IInitializable, INotifyPropertyChanged, IDisposable
    {
        private readonly IMonitorrentHttpClient _monitorrentHttpClient;
        private Task _updatesTask;
        private readonly CancellationTokenSource _updateCancellationTokenSource = new CancellationTokenSource();

        public MainPageViewModel(IMonitorrentHttpClient monitorrentHttpClient)
        {
            _monitorrentHttpClient = monitorrentHttpClient;
            
        }

        public bool Loading { get; set; }
        public ObservableCollection<TopicViewModel> Topics { get; set; }
        public CompletionStatus CompletionStatus { get; set; }
        public int ExecuteId { get; set; }
        public int LogId { get; set; }
        public string Status { get; set; }
        public double RunProgress { get; set; }
        public Func<double, uint, Easing, Task<bool>> UpdateProgressAction { get; set; }
        public bool IsRunning
        {
            get { return CompletionStatus == CompletionStatus.Executing; }
        }

        public async Task Execute()
        {
            try
            {
                await _monitorrentHttpClient.Execute();
            }
            catch (Exception e)
            {
            }
        }

        public void StartUpdates()
        {
            if (_updatesTask == null)
            {
                _updatesTask = ExecuteListener();
            }
        }

        public async Task SetProgress(double value)
        {
            RunProgress = value;
            await UpdateProgressAction(value, 250, Easing.Linear);
        }

        public async Task ExecuteListener()
        {
            var result = await _monitorrentHttpClient.ExecuteCurrentDetails();
            if (result.IsRunning)
            {
                CompletionStatus = CompletionStatus.Executing;
                await SetProgress(.05);
            }
            ProcessEvents(result.Logs);
            if (result.IsRunning)
            {
                await ExecuteDetailsListener();
            }
            else
            {
                await ExecuteListener();
            }
        }

        private async Task ExecuteDetailsListener(bool oneTime = false)
        {
            var result = await _monitorrentHttpClient.GetLogDetails(ExecuteId, LogId);
            ProcessEvents(result.Logs);
            if (result.IsRunning)
            {
                if (RunProgress <= 0.7)
                {
                    await SetProgress(RunProgress + 0.10);
                }
                else if (RunProgress >0.7 && RunProgress <= 0.99)
                {
                    await SetProgress(RunProgress + 0.03);
                }
                await ExecuteDetailsListener();
            }
            else
            {
                await SetProgress(1);
                CompletionStatus = CompletionStatus.Success;
                await SetProgress(0);
                if (!oneTime)
                {
                    await ExecuteListener();
                }
            }
        }

        public async Task UpdateExecuteStatus()
        {
            var result = await _monitorrentHttpClient.GetLogs(0, 1);
            if(result != null && result.LogEntries.Count > 0)
            {
                var log = result.LogEntries.Last();
                UpdateStatus(log.FinishTime);
            }
        }

        private void ProcessEvents(IList<ExecuteLog> logs)
        {
            if (logs != null && logs.Count > 0)
            {
                var logEntry = logs.Last();
                if (logEntry != null)
                {
                    ExecuteId = logEntry.ExecuteId;
                    LogId = logEntry.Id;
                    UpdateStatus(logEntry.Time);
                }
            }
        }

        private void UpdateStatus(DateTime time)
        {
            StringBuilder niceTimeBuilder = new StringBuilder("at ");
            niceTimeBuilder.Append(time.ToString("HH:mm"));
            niceTimeBuilder.Append(" (");
            niceTimeBuilder.Append(time.Humanize(false));
            niceTimeBuilder.Append(")");
            Status = niceTimeBuilder.ToString();
        }

        public void EndUpdates()
        {
            _updatesTask = null;
            _updateCancellationTokenSource.Cancel();
        }

        public async Task Initialize()
        {
            Loading = true;
            try
            {
                var topics = await _monitorrentHttpClient.GetTopics();
                Topics =
                    new ObservableCollection<TopicViewModel>(topics.Select(x => new TopicViewModel(x, _monitorrentHttpClient)));
                await UpdateExecuteStatus();
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            _updateCancellationTokenSource.Dispose();
            _monitorrentHttpClient.Dispose();
        }
    }
}
