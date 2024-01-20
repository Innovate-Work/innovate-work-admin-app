using Hanssens.Net;
using innovate_work_admin_app.Models.JWT;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace innovate_work_admin_app.Controllers
{
    public class BaseController : Controller
    {
        protected IHttpClientFactory _httpClientFactory;
        protected LocalStorage _localStorage;
        protected readonly string _tokenName;

        public BaseController(IHttpClientFactory httpClientFactory,
                              LocalStorage localStorage,
                              IConfiguration configuration)
        {
            _tokenName = configuration.GetSection("TokenLocalStorageName").Value;
            _httpClientFactory = httpClientFactory;
            _localStorage = localStorage;
        }

        protected HttpClient GetHttpClient(string clientName)
        {
            var httpClient = _httpClientFactory.CreateClient(clientName);
            if (_localStorage.Exists(clientName))
            {
                string jsonToken = _localStorage.Get<string>(_tokenName);
                SecurityToken token = JsonSerializer.Deserialize<SecurityToken>(jsonToken);
                if (token != null)
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            }
            return httpClient;
        }
    }
}
