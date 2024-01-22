using Hanssens.Net;
using innovate_work_admin_app.Models;
using innovate_work_admin_app.Models.JWT;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace innovate_work_admin_app.Controllers
{
    public class UserController : BaseController
    {
        private readonly string _httpClientName = "authentication";
        public UserController(IHttpClientFactory httpClientFactory,
                              LocalStorage localStorage,
                              IConfiguration configuration) : base(httpClientFactory, localStorage, configuration)
        {
        }
        private async Task SignIn(SecurityToken securityToken)
        {
            var claims = new List<Claim>
            {
                new Claim(nameof(SecurityToken.Token), securityToken.Token),
                new Claim(nameof(SecurityToken.ExpireAt), securityToken.ExpireAt.ToString())
                //new Claim(ClaimTypes.Name, securityToken.UserName),
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(claimsPrincipal);
        }
        private async Task<SecurityToken?> Token(string username, string password)
        {
            var httpClient = GetHttpClient(_httpClientName);
            var parameters = new
            {
                userName = username,
                password = password
            };

            var json = JsonSerializer.Serialize(parameters);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var httpResponseMessage = await httpClient.PostAsync(httpClient.BaseAddress + "/login", data);

            if (!httpResponseMessage.IsSuccessStatusCode)
                return null;

            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            SecurityToken? token = await JsonSerializer.DeserializeAsync<SecurityToken>(contentStream, options);
            return token;
        }
        public async Task<IActionResult> Authenticate(string username, string password)
        {
            var token = await Token(username, password);
            if (token == null)
                return View("Login");


            string jsonToken = JsonSerializer.Serialize(token);
            if (jsonToken == null)
                return View("Login");

            _localStorage.Store(_tokenName, jsonToken);
            await SignIn(token);
            return RedirectToAction("Index", "Client");
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

        public IActionResult Login()
        {
            return View();
        }
    }
}
