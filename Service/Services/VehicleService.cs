using System.Net.Http;
using System.Text.Json;
using Models.Models;

namespace Service.Services
{
    public class VehicleService
    {
        private readonly HttpClient _httpClient;

        public VehicleService(string apiUrl)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(apiUrl) };
        }

        public async Task<List<Vehicle>> GetVehiclesAsync(string immatriculation)
        {
            var immatEncoded = Uri.EscapeDataString(immatriculation);
            var response = await _httpClient.GetAsync($"/MyGarage/GetVehicle?immatriculation={immatEncoded}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Vehicle>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<VehicleHistory> GetVehicleHistoryAsync(string immatriculation)
        {
            var response = await _httpClient.GetAsync($"/MyGarage/GetHistVehicle/{immatriculation}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<VehicleHistory>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
