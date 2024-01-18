using Hanssens.Net;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace innovate_work_admin_app.Controllers
{
    public class UserController : BaseController
    {
        public UserController(IHttpClientFactory httpClientFactory, 
                              LocalStorage localStorage, 
                              IConfiguration configuration) : base(httpClientFactory, localStorage, configuration)
        {
        }

        public async Task<IActionResult> GetAll()
        {
            var httpClient = GetHttpClient("Users");
            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Get,
                httpClient.BaseAddress
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (!httpResponseMessage.IsSuccessStatusCode)
                return null;

            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            IEnumerable<User>? users = await JsonSerializer.DeserializeAsync<IEnumerable<User>>(contentStream, options);
            if (users == null)
                users = new List<User>();

            return View(users);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
