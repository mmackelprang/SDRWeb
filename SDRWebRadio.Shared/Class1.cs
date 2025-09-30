namespace SDRWebRadio.Shared.Models
{
    public enum RadioMode
    {
        AM,
        FM,
        SW
    }

    public class RadioBandConfig
    {
        public RadioMode Mode { get; set; }
        public double MinFrequency { get; set; } // in MHz
        public double MaxFrequency { get; set; } // in MHz
        public double Step { get; set; } // in MHz
        public bool SupportsMetadata { get; set; }

        public static RadioBandConfig GetConfig(RadioMode mode)
        {
            return mode switch
            {
                RadioMode.FM => new RadioBandConfig
                {
                    Mode = RadioMode.FM,
                    MinFrequency = 88.0,
                    MaxFrequency = 108.0,
                    Step = 0.1,
                    SupportsMetadata = true // FM supports RDS
                },
                RadioMode.AM => new RadioBandConfig
                {
                    Mode = RadioMode.AM,
                    MinFrequency = 0.53,
                    MaxFrequency = 1.71,
                    Step = 0.01,
                    SupportsMetadata = false
                },
                RadioMode.SW => new RadioBandConfig
                {
                    Mode = RadioMode.SW,
                    MinFrequency = 2.3,
                    MaxFrequency = 26.1,
                    Step = 0.01,
                    SupportsMetadata = false
                },
                _ => throw new ArgumentException($"Unknown radio mode: {mode}")
            };
        }
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

    public class RadioFavorite
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public double Frequency { get; set; }
        public RadioMode Mode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class StationMetadata
    {
        public string StationName { get; set; } = string.Empty;
        public string ProgramService { get; set; } = string.Empty; // RDS PS
        public string RadioText { get; set; } = string.Empty; // RDS RT
        public string Artist { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
