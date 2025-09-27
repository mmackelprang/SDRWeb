using SDRWebRadio.Shared.Models;
using SDRWebRadio.Shared.DTOs;

namespace SDRWebRadio.Server.Services
{
    public interface ISdrService
    {
        Task<DeviceStatusResponse> GetDevicesAsync();
        Task<ApiResponse<bool>> SetRadioSettingsAsync(RadioSettingsRequest settings);
        Task<StreamControlResponse> StartStreamAsync(RadioSettingsRequest settings);
        Task<ApiResponse<bool>> StopStreamAsync();
        Task<ApiResponse<RadioSettings>> GetCurrentSettingsAsync();
        bool IsStreaming { get; }
    }
}