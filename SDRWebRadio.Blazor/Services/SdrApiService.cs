using System.Text;
using System.Text.Json;
using SDRWebRadio.Shared.DTOs;
using SDRWebRadio.Shared.Models;

namespace SDRWebRadio.Blazor.Services
{
    public class SdrApiService : ISdrApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public SdrApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<DeviceStatusResponse> GetDevicesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("sdr/devices");
                var content = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<DeviceStatusResponse>(content, _jsonOptions) 
                           ?? new DeviceStatusResponse { IsSuccess = false, Message = "Failed to deserialize response" };
                }
                
                return new DeviceStatusResponse { IsSuccess = false, Message = $"API Error: {response.StatusCode}" };
            }
            catch (Exception ex)
            {
                return new DeviceStatusResponse { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<bool>> SetRadioSettingsAsync(RadioSettingsRequest settings)
        {
            try
            {
                var json = JsonSerializer.Serialize(settings, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("sdr/settings", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions) 
                           ?? new ApiResponse<bool> { IsSuccess = false, Message = "Failed to deserialize response" };
                }
                
                return new ApiResponse<bool> { IsSuccess = false, Message = $"API Error: {response.StatusCode}" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<StreamControlResponse> StartStreamAsync(RadioSettingsRequest settings)
        {
            try
            {
                var json = JsonSerializer.Serialize(settings, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("sdr/stream/start", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<StreamControlResponse>(responseContent, _jsonOptions) 
                           ?? new StreamControlResponse { IsSuccess = false, Message = "Failed to deserialize response" };
                }
                
                return new StreamControlResponse { IsSuccess = false, Message = $"API Error: {response.StatusCode}" };
            }
            catch (Exception ex)
            {
                return new StreamControlResponse { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<bool>> StopStreamAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("sdr/stream/stop", null);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions) 
                           ?? new ApiResponse<bool> { IsSuccess = false, Message = "Failed to deserialize response" };
                }
                
                return new ApiResponse<bool> { IsSuccess = false, Message = $"API Error: {response.StatusCode}" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<RadioSettings>> GetCurrentSettingsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("sdr/settings");
                var content = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<RadioSettings>>(content, _jsonOptions) 
                           ?? new ApiResponse<RadioSettings> { IsSuccess = false, Message = "Failed to deserialize response" };
                }
                
                return new ApiResponse<RadioSettings> { IsSuccess = false, Message = $"API Error: {response.StatusCode}" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<RadioSettings> { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<bool> GetStreamStatusAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("sdr/stream/status");
                var content = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<dynamic>(content, _jsonOptions);
                    return result?.GetProperty("isStreaming").GetBoolean() ?? false;
                }
                
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}