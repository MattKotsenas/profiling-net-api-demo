namespace Profiling.Api.Services.Json;

public class JsonService : IJsonService
{
    private readonly ILogger<JsonService> _logger;

    public JsonService(ILogger<JsonService> logger)
    {
        _logger = logger;
    }

    public object Run()
    {
        return new { message = "Hello, World!" };
    }
}