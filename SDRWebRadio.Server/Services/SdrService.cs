using System.Diagnostics;
using System.Net.Sockets;
using SDRWebRadio.Shared.Models;
using SDRWebRadio.Shared.DTOs;

namespace SDRWebRadio.Server.Services
{
    public class SdrService : ISdrService
    {
        private readonly ILogger<SdrService> _logger;
        private RadioSettings _currentSettings;
        private Process? _rtlTcpProcess;
        private Process? _rtlFmProcess;
        private TcpClient? _tcpClient;
        private const int RTL_TCP_PORT = 1234;
        private const string RTL_TCP_HOST = "localhost";

        public bool IsStreaming => _currentSettings.IsStreaming;

        public SdrService(ILogger<SdrService> logger)
        {
            _logger = logger;
            _currentSettings = new RadioSettings
            {
                Frequency = 100.1e6, // 100.1 MHz FM
                Mode = RadioMode.FM,
                SampleRate = 2048000,
                Gain = 0,
                IsStreaming = false
            };
        }

        public async Task<DeviceStatusResponse> GetDevicesAsync()
        {
            var devices = new List<SdrDevice>();
            
            try
            {
                // Check if rtl_test is available and can detect devices
                var processInfo = new ProcessStartInfo
                {
                    FileName = "rtl_test",
                    Arguments = "-t",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(processInfo);
                if (process != null)
                {
                    var output = await process.StandardOutput.ReadToEndAsync();
                    var error = await process.StandardError.ReadToEndAsync();
                    await process.WaitForExitAsync();

                    if (process.ExitCode == 0 && output.Contains("Found"))
                    {
                        // Parse device info from output
                        devices.Add(new SdrDevice
                        {
                            Id = 0,
                            Name = "RTL-SDR Device",
                            Status = DeviceStatus.Connected,
                            SerialNumber = "00000001",
                            LastUpdated = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        _logger.LogWarning("No RTL-SDR devices found. Output: {Output}, Error: {Error}", output, error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error detecting RTL-SDR devices");
                return new DeviceStatusResponse
                {
                    IsSuccess = false,
                    Message = "Error detecting devices: rtl-sdr tools not found or not installed"
                };
            }

            return new DeviceStatusResponse
            {
                Devices = devices,
                IsSuccess = true,
                Message = devices.Any() ? "Devices found" : "No devices detected"
            };
        }

        public async Task<ApiResponse<bool>> SetRadioSettingsAsync(RadioSettingsRequest settings)
        {
            try
            {
                _currentSettings.Frequency = settings.Frequency;
                _currentSettings.Mode = settings.Mode;
                _currentSettings.Gain = settings.Gain;

                _logger.LogInformation("Radio settings updated: Frequency={Frequency}, Mode={Mode}, Gain={Gain}",
                    settings.Frequency, settings.Mode, settings.Gain);

                return new ApiResponse<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = "Settings updated successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting radio settings");
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    Message = "Error updating settings"
                };
            }
        }

        public async Task<StreamControlResponse> StartStreamAsync(RadioSettingsRequest settings)
        {
            try
            {
                if (_currentSettings.IsStreaming)
                {
                    await StopStreamAsync();
                }

                await SetRadioSettingsAsync(settings);

                // Start rtl_tcp server
                await StartRtlTcpAsync();

                // Start rtl_fm for audio processing
                await StartRtlFmAsync();

                _currentSettings.IsStreaming = true;

                var stream = new AudioStream
                {
                    StreamUrl = "/api/sdr/audio",
                    Format = "wav",
                    SampleRate = 44100,
                    IsActive = true
                };

                return new StreamControlResponse
                {
                    IsSuccess = true,
                    Message = "Stream started successfully",
                    Stream = stream
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting stream");
                return new StreamControlResponse
                {
                    IsSuccess = false,
                    Message = $"Error starting stream: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<bool>> StopStreamAsync()
        {
            try
            {
                _rtlFmProcess?.Kill();
                _rtlTcpProcess?.Kill();
                _tcpClient?.Close();

                _rtlFmProcess?.Dispose();
                _rtlTcpProcess?.Dispose();
                _tcpClient?.Dispose();

                _rtlFmProcess = null;
                _rtlTcpProcess = null;
                _tcpClient = null;

                _currentSettings.IsStreaming = false;

                return new ApiResponse<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = "Stream stopped successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping stream");
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    Message = "Error stopping stream"
                };
            }
        }

        public async Task<ApiResponse<RadioSettings>> GetCurrentSettingsAsync()
        {
            return new ApiResponse<RadioSettings>
            {
                IsSuccess = true,
                Data = _currentSettings,
                Message = "Current settings retrieved"
            };
        }

        private async Task StartRtlTcpAsync()
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "rtl_tcp",
                Arguments = $"-a {RTL_TCP_HOST} -p {RTL_TCP_PORT} -f {_currentSettings.Frequency} -s {_currentSettings.SampleRate} -g {_currentSettings.Gain}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            _rtlTcpProcess = Process.Start(processInfo);
            if (_rtlTcpProcess == null)
            {
                throw new InvalidOperationException("Failed to start rtl_tcp process");
            }

            // Wait a moment for rtl_tcp to start
            await Task.Delay(1000);

            _logger.LogInformation("rtl_tcp started on {Host}:{Port}", RTL_TCP_HOST, RTL_TCP_PORT);
        }

        private async Task StartRtlFmAsync()
        {
            var modeArg = _currentSettings.Mode switch
            {
                RadioMode.FM => "fm",
                RadioMode.AM => "am",
                RadioMode.SW => "am", // Use AM for shortwave
                _ => "fm"
            };

            var processInfo = new ProcessStartInfo
            {
                FileName = "rtl_fm",
                Arguments = $"-f {_currentSettings.Frequency} -M {modeArg} -s 250000 -r 44100 -g {_currentSettings.Gain}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            _rtlFmProcess = Process.Start(processInfo);
            if (_rtlFmProcess == null)
            {
                throw new InvalidOperationException("Failed to start rtl_fm process");
            }

            _logger.LogInformation("rtl_fm started for {Mode} mode at {Frequency} Hz", _currentSettings.Mode, _currentSettings.Frequency);
        }

        public void Dispose()
        {
            StopStreamAsync().Wait();
        }
    }
}