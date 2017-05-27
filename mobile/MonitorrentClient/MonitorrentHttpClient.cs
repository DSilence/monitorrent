using System;
using System.Collections.Generic;
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
        public event EventHandler Unauthorized;
        
        private static readonly StringContent EmptyContent = new StringContent(string.Empty);

        public MonitorrentHttpClient(Uri baseUrl)
        {
            _client = new HttpClient(_handler)
            {
                BaseAddress = baseUrl
            };
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
            var response = await _client.GetAsync(ApiContracts.Topics);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                OnUnauthorized();
            }
            return JsonConvert.DeserializeObject<List<Topic>>(await response.Content.ReadAsStringAsync());
        }

        public async Task<string> Login(string password)
        {
            var response =
                await _client.PostAsync(ApiContracts.Login, 
                new StringContent(JsonConvert.SerializeObject(new LoginRequest
                {
                    Password = password
                })));
            if (response.IsSuccessStatusCode)
            {
                if (_cookieContainer?.GetCookies(BaseAddress)["jwt"] == null)
                {
                    //TODO F*cking UWP case
                    IEnumerable<string> cookies;
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

        public async Task<ExecuteResult> GetLogs(int skip, int take)
        {
            var response = await _client.GetStringAsync(string.Format(ApiContracts.Logs, skip, take));
            return JsonConvert.DeserializeObject<ExecuteResult>(response);
        }

        public Task Execute()
        {
            return _client.PostAsync(ApiContracts.Execute, EmptyContent);
        }

        public Task ExecuteTopic(IList<int> topicIds)
        {
            if (topicIds == null)
            {
                throw new ArgumentNullException(nameof(topicIds));
            }
            return _client.PostAsync(string.Format(ApiContracts.ExecuteSpecific, 
                string.Join(",", topicIds)), EmptyContent);
        }

        public Task DeleteTopic(int topicId)
        {
            return _client.DeleteAsync(string.Format(ApiContracts.Topic, topicId));
        }

        public async Task<ExecuteDetails> ExecuteCurrentDetails()
        {
            var result = await _client.GetStringAsync(ApiContracts.ExecuteCurrentDetails);
            return JsonConvert.DeserializeObject<ExecuteDetails>(result);
        }

        public async Task<ExecuteDetails> GetLogDetails(int logId, int after)
        {
            var result = await _client.GetStringAsync(string.Format(ApiContracts.ExecuteSpecificDetails, logId, after));
            return JsonConvert.DeserializeObject<ExecuteDetails>(result);
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        protected virtual void OnUnauthorized()
        {
            Unauthorized?.Invoke(this, EventArgs.Empty);
        }
    }
}