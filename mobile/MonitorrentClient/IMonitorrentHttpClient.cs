using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MonitorrentClient.Models;

namespace MonitorrentClient
{
    public interface IMonitorrentHttpClient
    {
        Uri BaseAddress { get; set; }
        string Token { get; set; }

        Task<IList<Topic>> GetTopics();
        Task<string> Login(string password);
    }
}