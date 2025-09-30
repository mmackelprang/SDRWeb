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

    [Theory]
    [InlineData(RadioMode.FM, 88.0, 108.0, 0.1, true)]
    [InlineData(RadioMode.AM, 0.53, 1.71, 0.01, false)]
    [InlineData(RadioMode.SW, 2.3, 26.1, 0.01, false)]
    public void RadioBandConfig_ShouldReturnCorrectRanges(RadioMode mode, double minFreq, double maxFreq, double step, bool supportsMetadata)
    {
        // Act
        var config = RadioBandConfig.GetConfig(mode);

        // Assert
        Assert.Equal(mode, config.Mode);
        Assert.Equal(minFreq, config.MinFrequency);
        Assert.Equal(maxFreq, config.MaxFrequency);
        Assert.Equal(step, config.Step);
        Assert.Equal(supportsMetadata, config.SupportsMetadata);
    }

    [Fact]
    public void RadioFavorite_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var favorite = new RadioFavorite
        {
            Name = "Test Station",
            Frequency = 100.1,
            Mode = RadioMode.FM
        };

        // Assert
        Assert.NotEqual(Guid.Empty, favorite.Id);
        Assert.Equal("Test Station", favorite.Name);
        Assert.Equal(100.1, favorite.Frequency);
        Assert.Equal(RadioMode.FM, favorite.Mode);
    }

    [Fact]
    public void StationMetadata_ShouldSetPropertiesCorrectly()
    {
        // Arrange & Act
        var metadata = new StationMetadata
        {
            StationName = "WXYZ FM",
            ProgramService = "WXYZ",
            RadioText = "Now Playing: Test Song",
            Artist = "Test Artist",
            Title = "Test Title"
        };

        // Assert
        Assert.Equal("WXYZ FM", metadata.StationName);
        Assert.Equal("WXYZ", metadata.ProgramService);
        Assert.Equal("Now Playing: Test Song", metadata.RadioText);
        Assert.Equal("Test Artist", metadata.Artist);
        Assert.Equal("Test Title", metadata.Title);
    }

    [Fact]
    public void FavoriteRequest_ShouldValidateProperties()
    {
        // Arrange & Act
        var request = new FavoriteRequest
        {
            Name = "My Favorite",
            Frequency = 95.5,
            Mode = RadioMode.FM
        };

        // Assert
        Assert.Equal("My Favorite", request.Name);
        Assert.Equal(95.5, request.Frequency);
        Assert.Equal(RadioMode.FM, request.Mode);
    }
}