using Newtonsoft.Json;

namespace MonitorrentClient.Models
{
    public class LoginRequest
    {
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
