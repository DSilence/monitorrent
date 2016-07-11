using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MonitorrentClient.Models
{
    public class ExecuteResult
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("data")]
        public List<ExecuteEntry> LogEntries { get; set; }
    }
}
