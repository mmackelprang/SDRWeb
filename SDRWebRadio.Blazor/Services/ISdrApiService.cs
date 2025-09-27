using SDRWebRadio.Shared.DTOs;
using SDRWebRadio.Shared.Models;

namespace SDRWebRadio.Blazor.Services
{
    public interface ISdrApiService
    {
        Task<DeviceStatusResponse> GetDevicesAsync();
        Task<ApiResponse<bool>> SetRadioSettingsAsync(RadioSettingsRequest settings);
        Task<StreamControlResponse> StartStreamAsync(RadioSettingsRequest settings);
        Task<ApiResponse<bool>> StopStreamAsync();
        Task<ApiResponse<RadioSettings>> GetCurrentSettingsAsync();
        Task<bool> GetStreamStatusAsync();
    }
}