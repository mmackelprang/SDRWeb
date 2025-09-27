namespace SDRWebRadio.Shared.Models
{
    public enum RadioMode
    {
        AM,
        FM,
        SW
    }

    public enum DeviceStatus
    {
        Disconnected,
        Connected,
        Streaming,
        Error
    }

    public class SdrDevice
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DeviceStatus Status { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
    }

    public class RadioSettings
    {
        public double Frequency { get; set; }
        public RadioMode Mode { get; set; }
        public int SampleRate { get; set; } = 2048000;
        public int Gain { get; set; } = 0;
        public bool IsStreaming { get; set; }
    }

    public class AudioStream
    {
        public string StreamUrl { get; set; } = string.Empty;
        public string Format { get; set; } = "wav";
        public int SampleRate { get; set; } = 44100;
        public bool IsActive { get; set; }
    }
}
