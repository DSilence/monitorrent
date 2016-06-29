using Newtonsoft.Json;

namespace MonitorrentClient.Models
{
    public class Topic
    {
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("tracker")]
        public string Tracker { get; set; }
    }
}