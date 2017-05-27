using System.Collections.Generic;
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
