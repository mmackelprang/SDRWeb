# Copilot Instructions for SDRWeb

## Project Overview

This is an SDR WebRadio application that controls and streams from an RTL-SDR dongle attached to a Raspberry Pi. The web UI allows listening to and tuning AM, FM, and shortwave radio stations.

## Technology Stack

- **Backend**: .NET 8, ASP.NET Core Web API
- **Frontend**: Blazor Server, Bootstrap 5, Font Awesome
- **Hardware**: RTL-SDR dongle (RTL2832U-based)
- **Platform**: Raspberry Pi 4+ with Raspberry Pi OS
- **SDR Tools**: rtl-sdr suite (rtl_tcp, rtl_fm, rtl_test)
- **Testing**: xUnit

## Project Structure

```
SDRWebRadio/
├── SDRWebRadio.Blazor/    # Main Blazor Server UI project
│   ├── Components/        # Blazor UI components
│   ├── Services/          # Client-side services
│   └── Program.cs         # App configuration
├── SDRWebRadio.Server/    # REST API backend (communicates with RTL-SDR)
│   ├── Controllers/       # API endpoints
│   ├── Services/          # Business logic
│   └── Program.cs         # API configuration
├── SDRWebRadio.Shared/    # Common models, DTOs, and shared logic
│   ├── Models/            # Domain models
│   └── DTOs/              # Data transfer objects
├── SDRWebRadio.Tests/     # Unit and integration tests
└── SDRWebRadio.sln        # Solution file
```

## Key Components

### Models (SDRWebRadio.Shared/Models/)
- `RadioSettings`: Configuration for radio streaming (frequency, sample rate, gain)
- `SdrDevice`: Represents an SDR device with status
- `DeviceStatus`: Enum for device states (Connected, Disconnected, Error, etc.)
- `RadioMode`: Enum for radio modes (FM, AM, SW)

### DTOs (SDRWebRadio.Shared/DTOs/)
- `ApiResponse<T>`: Generic API response wrapper
- `RadioSettingsRequest`: Request for updating radio settings
- `DeviceStatusResponse`: Response with device status information

### Services (SDRWebRadio.Server/Services/)
- `ISdrService`: Interface for SDR operations
- `SdrService`: Implementation of SDR control logic

### Controllers (SDRWebRadio.Server/Controllers/)
- `SdrController`: API endpoints for SDR control and streaming

## Building and Testing

### Build the solution
```bash
dotnet build
```

### Run tests
```bash
dotnet test
```

### Run with code coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Code Style and Conventions

### General Guidelines
- Follow standard C# naming conventions (PascalCase for classes/methods, camelCase for parameters)
- Use async/await for I/O operations
- Include XML documentation comments for public APIs
- Keep controllers thin - business logic belongs in services

### Project-Specific Guidelines
- Use dependency injection for services
- DTOs should be used for API communication
- Models should represent domain entities
- Tests should use xUnit with descriptive names following pattern: `MethodName_ShouldExpectedBehavior`

### API Configuration
- Server API runs on port 7002 (HTTPS) or 5002 (HTTP)
- CORS is configured for Blazor client (ports 7001 and 5001)
- Swagger is enabled in development mode

### Known Warnings
The following warnings are known and can be ignored for now:
- CS1998: Async methods without await operators in SdrService and SdrController
- ASP0019: IHeaderDictionary usage in SdrController

## Testing Standards

- Use xUnit for unit tests
- Test names should describe what they test: `TestMethod_ShouldExpectedBehavior`
- Use `[Theory]` and `[InlineData]` for parameterized tests
- Arrange-Act-Assert pattern for test structure
- All tests must pass before submitting changes

## Common Tasks

### Adding a new API endpoint
1. Define DTO in `SDRWebRadio.Shared/DTOs.cs`
2. Add method to `ISdrService` interface
3. Implement method in `SdrService`
4. Add controller action in `SdrController`
5. Add unit tests

### Adding a new Blazor component
1. Create component in `SDRWebRadio.Blazor/Components/`
2. Add necessary services to dependency injection in `Program.cs`
3. Update navigation in `NavMenu` if needed

### Modifying radio settings
1. Update `RadioSettings` model if needed
2. Modify `RadioSettingsRequest` DTO
3. Update service implementation
4. Add or update tests

## Important Notes

- This application is designed for Raspberry Pi with RTL-SDR hardware
- RTL-SDR tools must be installed on the system
- The application uses external processes to control the SDR hardware
- Streaming functionality depends on rtl_fm and other RTL-SDR utilities
- CORS configuration is specific to development (update for production)

## Support and Resources

- [RTL-SDR Documentation](https://osmocom.org/projects/rtl-sdr/wiki)
- [.NET 8 Documentation](https://learn.microsoft.com/dotnet/)
- [Blazor Documentation](https://learn.microsoft.com/aspnet/core/blazor/)
