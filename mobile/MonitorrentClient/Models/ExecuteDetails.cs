using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
