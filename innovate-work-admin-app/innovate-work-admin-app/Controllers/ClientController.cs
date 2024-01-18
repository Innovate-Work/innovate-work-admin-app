using Hanssens.Net;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using innovate_work_admin_app.Models;
using System.Net.Http;

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

        public async Task<Client?> GetAsync(int id)
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
        public async Task<IEnumerable<Client>?> GetAllAsync()
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
        public async Task<bool> CreateAsync(Client client)
        {
            var httpClient = GetHttpClient(_httpClientName);

            var parameters = new Dictionary<string, string>();
            parameters["name"] = client.Name;
            parameters["email"] = client.Email;
            parameters["phone"] = client.Phone;
            parameters["withSubscription"] = client.WithSubscription.ToString();
            parameters["isCustom"] = client.IsCustom.ToString();

            var httpResponseMessage = await httpClient.PostAsync(httpClient.BaseAddress, new FormUrlEncodedContent(parameters));
            
            if (httpResponseMessage.IsSuccessStatusCode)
                return true;

            return false;
        }
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
