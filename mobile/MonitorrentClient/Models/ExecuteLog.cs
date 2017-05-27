using System;
using Newtonsoft.Json;

namespace MonitorrentClient.Models
{
    public class ExecuteLog
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("level")]
        public string Level { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("execute_id")]
        public int ExecuteId { get; set; }
        [JsonProperty("time")]
        public DateTime Time { get; set; }
    }
}
