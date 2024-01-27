using Hanssens.Net;
using innovate_work_admin_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace innovate_work_admin_app.Controllers
{
    public class ClientController : BaseController
    {
        private readonly string _httpClientName = "clients";
        public ClientController(IHttpClientFactory httpClientFactory,
                                LocalStorage localStorage,
                                IConfiguration configuration) : base(httpClientFactory, localStorage, configuration)
        {
        }
        private async Task<Client?> GetAsync(int id)
        {
            var httpClient = GetHttpClient(_httpClientName);
            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Get,
                httpClient.BaseAddress + $"/{id}"
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (!httpResponseMessage.IsSuccessStatusCode)
                return null;

            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Client? client = await JsonSerializer.DeserializeAsync<Client>(contentStream, options);

            return client;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await GetAllAsync());
        }
        private async Task<IEnumerable<Client>?> GetAllAsync()
        {
            var httpClient = GetHttpClient(_httpClientName);

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
            IEnumerable<Client>? clients = await JsonSerializer.DeserializeAsync<IEnumerable<Client>>(contentStream, options);

            return clients;
        }

        [Authorize]
        [HttpDelete("/CLient/Delete/{clientId}")]
        public async Task<bool> DeleteAsync(Guid clientId)
        {
            var httpClient = GetHttpClient(_httpClientName);
            var httpRequestMessage = new HttpRequestMessage
            (
                HttpMethod.Delete,
                httpClient.BaseAddress + $"/{clientId}"
            );

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            if (httpResponseMessage.IsSuccessStatusCode)
                return true;

            return false;
        }
    }
}
