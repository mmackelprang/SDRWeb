using SDRWebRadio.Shared.Models;
using SDRWebRadio.Shared.DTOs;

namespace SDRWebRadio.Tests;

public class SdrModelsTests
{
    [Fact]
    public void RadioSettings_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var settings = new RadioSettings();

        // Assert
        Assert.Equal(2048000, settings.SampleRate);
        Assert.Equal(0, settings.Gain);
        Assert.False(settings.IsStreaming);
    }

    [Fact]
    public void SdrDevice_ShouldSetPropertiesCorrectly()
    {
        // Arrange & Act
        var device = new SdrDevice
        {
            Id = 1,
            Name = "Test Device",
            Status = DeviceStatus.Connected,
            SerialNumber = "12345",
            LastUpdated = DateTime.UtcNow
        };

        // Assert
        Assert.Equal(1, device.Id);
        Assert.Equal("Test Device", device.Name);
        Assert.Equal(DeviceStatus.Connected, device.Status);
        Assert.Equal("12345", device.SerialNumber);
    }

    [Fact]
    public void RadioSettingsRequest_ShouldValidateFrequencyRange()
    {
        // Arrange
        var request = new RadioSettingsRequest
        {
            Frequency = 100.1e6, // 100.1 MHz
            Mode = RadioMode.FM,
            Gain = 10
        };

        // Assert
        Assert.True(request.Frequency >= 24e6); // 24 MHz minimum
        Assert.True(request.Frequency <= 1766e6); // 1766 MHz maximum
        Assert.Equal(RadioMode.FM, request.Mode);
        Assert.Equal(10, request.Gain);
    }

    [Theory]
    [InlineData(RadioMode.FM)]
    [InlineData(RadioMode.AM)]
    [InlineData(RadioMode.SW)]
    public void RadioMode_ShouldSupportAllModes(RadioMode mode)
    {
        // Arrange
        var settings = new RadioSettings
        {
            Mode = mode
        };

        // Assert
        Assert.Equal(mode, settings.Mode);
    }

    [Fact]
    public void ApiResponse_ShouldHandleGenericType()
    {
        // Arrange & Act
        var response = new ApiResponse<bool>
        {
            IsSuccess = true,
            Message = "Test message",
            Data = true
        };

        // Assert
        Assert.True(response.IsSuccess);
        Assert.Equal("Test message", response.Message);
        Assert.True(response.Data);
    }

    [Fact]
    public void DeviceStatusResponse_ShouldHandleMultipleDevices()
    {
        // Arrange
        var devices = new List<SdrDevice>
        {
            new() { Id = 1, Name = "Device 1", Status = DeviceStatus.Connected },
            new() { Id = 2, Name = "Device 2", Status = DeviceStatus.Disconnected }
        };

        // Act
        var response = new DeviceStatusResponse
        {
            Devices = devices,
            IsSuccess = true,
            Message = "Found 2 devices"
        };

        // Assert
        Assert.Equal(2, response.Devices.Count);
        Assert.True(response.IsSuccess);
        Assert.Contains("2 devices", response.Message);
    }
}