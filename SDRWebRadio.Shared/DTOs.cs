using SDRWebRadio.Shared.Models;

namespace SDRWebRadio.Shared.DTOs
{
    public class DeviceStatusResponse
    {
        public List<SdrDevice> Devices { get; set; } = new();
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class RadioSettingsRequest
    {
        public double Frequency { get; set; }
        public RadioMode Mode { get; set; }
        public int Gain { get; set; } = 0;
    }

    public class StreamControlRequest
    {
        public bool Start { get; set; }
        public RadioSettingsRequest? Settings { get; set; }
    }

    public class StreamControlResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public AudioStream? Stream { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}