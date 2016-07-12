using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MonitorrentClient.Models;

namespace MonitorrentClient
{
    public interface IMonitorrentHttpClient: IDisposable
    {
        Uri BaseAddress { get; set; }
        string Token { get; set; }

        Task<IList<Topic>> GetTopics();
        Task<string> Login(string password);
        Task<ExecuteResult> GetLogs(int skip, int take);
        Task Execute();
        Task ExecuteTopic(IList<int> topicIds);
        Task DeleteTopic(int topicId);
        Task<ExecuteDetails> ExecuteCurrentDetails();
        Task<ExecuteDetails> GetLogDetails(int logId, int after);
    }
}