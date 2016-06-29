using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MonitorrentClient.Models;
using Newtonsoft.Json;

namespace MonitorrentClient
{
    public class MonitorrentHttpClient : IMonitorrentHttpClient
    {
        private readonly CookieContainer _cookieContainer = new CookieContainer();
        private readonly HttpClientHandler _handler = new HttpClientHandler();
        private readonly HttpClient _client;

        public MonitorrentHttpClient(Uri baseUrl)
        {
            _client = new HttpClient(_handler);
            _client.BaseAddress = baseUrl;
            _handler.CookieContainer = _cookieContainer;
        }

        public Uri BaseAddress
        {
            get { return _client.BaseAddress; }
            set { _client.BaseAddress = value; }
        }

        public string Token
        {
            get
            {
                return _cookieContainer?.GetCookies(BaseAddress)["jwt"]?.Value;
            }
            set
            {
                _cookieContainer.Add(BaseAddress, new Cookie("jwt", value));
            }
        }

        public async Task<IList<Topic>> GetTopics()
        {
            var response = await _client.GetAsync("/api/topics");
            return JsonConvert.DeserializeObject<List<Topic>>(await response.Content.ReadAsStringAsync());
        }

        public async Task<string> Login(string password)
        {
            var response =
                await _client.PostAsync("/api/login", 
                new StringContent(JsonConvert.SerializeObject(new LoginRequest
                {
                    Password = password
                })));
            if (response.IsSuccessStatusCode)
            {
                if (_cookieContainer?.GetCookies(BaseAddress)["jwt"] == null)
                {
                    IEnumerable<string> cookies = new List<string>();
                    if (response.Headers.TryGetValues("Set-Cookie", out cookies))
                    {
                        foreach (var cookie in cookies)
                        {
                            var jwtWithValues = cookie.Split(';');
                            if (jwtWithValues.Length == 3)
                            {
                                var jwtWithValue = jwtWithValues[0];
                                var keyAndValue = jwtWithValue?.Split('=');
                                if (keyAndValue?.Length == 2 && keyAndValue[0] == "jwt")
                                {
                                    var value = keyAndValue[1];
                                    Token = value;
                                }
                            }
                        }
                    }
                }
            }
            return response.IsSuccessStatusCode ? 
                _cookieContainer?.GetCookies(BaseAddress)["jwt"]
                ?.Value : null;
        }
    }
}