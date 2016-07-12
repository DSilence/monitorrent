using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorrentClient
{
    public static class ApiContracts
    {
        public const string Topics = "/api/topics";
        public const string Topic = "/api/topics/{0}";
        public const string Login = "/api/login";
        public const string Logs = "/api/execute/logs?skip={0}&take={1}";
        public const string Execute = "/api/execute/call";
        public const string ExecuteSpecific = Execute + "?ids={0}";
        public const string ExecuteCurrentDetails = "/api/execute/logs/current";
        public const string ExecuteSpecificDetails = "/api/execute/logs/{0}/details?after={1}";
    }
}
