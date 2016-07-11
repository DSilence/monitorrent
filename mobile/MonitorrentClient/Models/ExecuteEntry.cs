using System;
using MonitorrentClient.Enums;
using Newtonsoft.Json;

namespace MonitorrentClient.Models
{
    public class ExecuteEntry
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("status")]
        public Status Status { get; set; }
        [JsonProperty("failed_message")]
        public string FailedMessage { get; set; }
        [JsonProperty("failed")]
        public int FailedCount { get; set; }
        [JsonProperty("finish_time")]
        public DateTime FinishTime { get; set; }
        [JsonProperty("start_time")]
        public DateTime StartTime { get; set; }
        [JsonProperty("is_running")]
        public bool IsRunning { get; set; }
        [JsonProperty("downloaded")]
        public int DownloadedCount { get; set; }
    }
}