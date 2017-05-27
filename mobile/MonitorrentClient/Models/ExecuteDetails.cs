using System.Collections.Generic;
using Newtonsoft.Json;

namespace MonitorrentClient.Models
{
    public class ExecuteDetails
    {
        [JsonProperty("logs")]
        public IList<ExecuteLog> Logs { get; set; }
        [JsonProperty("is_running")]
        public bool IsRunning { get; set; }
    }
}
