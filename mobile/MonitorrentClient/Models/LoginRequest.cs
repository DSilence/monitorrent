using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MonitorrentClient.Models
{
    public class LoginRequest
    {
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
