using Microsoft.AspNetCore.Mvc;
using SDRWebRadio.Server.Services;
using SDRWebRadio.Shared.DTOs;

namespace SDRWebRadio.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SdrController : ControllerBase
    {
        private readonly ISdrService _sdrService;
        private readonly ILogger<SdrController> _logger;

        public SdrController(ISdrService sdrService, ILogger<SdrController> logger)
        {
            _sdrService = sdrService;
            _logger = logger;
        }

        [HttpGet("devices")]
        public async Task<IActionResult> GetDevices()
        {
            try
            {
                var result = await _sdrService.GetDevicesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting devices");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("settings")]
        public async Task<IActionResult> SetSettings([FromBody] RadioSettingsRequest settings)
        {
            try
            {
                var result = await _sdrService.SetRadioSettingsAsync(settings);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting radio settings");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("settings")]
        public async Task<IActionResult> GetCurrentSettings()
        {
            try
            {
                var result = await _sdrService.GetCurrentSettingsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current settings");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("stream/start")]
        public async Task<IActionResult> StartStream([FromBody] RadioSettingsRequest settings)
        {
            try
            {
                var result = await _sdrService.StartStreamAsync(settings);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting stream");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("stream/stop")]
        public async Task<IActionResult> StopStream()
        {
            try
            {
                var result = await _sdrService.StopStreamAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping stream");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("stream/status")]
        public IActionResult GetStreamStatus()
        {
            try
            {
                return Ok(new { isStreaming = _sdrService.IsStreaming });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stream status");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("audio")]
        public async Task<IActionResult> GetAudioStream()
        {
            try
            {
                if (!_sdrService.IsStreaming)
                {
                    return BadRequest(new { message = "No active stream" });
                }

                // This is a placeholder for audio streaming
                // In a real implementation, you would stream audio data here
                Response.Headers.Add("Content-Type", "audio/wav");
                Response.Headers.Add("Cache-Control", "no-cache");
                
                // Return empty audio stream for now
                var silence = new byte[4096];
                return File(silence, "audio/wav");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error streaming audio");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}