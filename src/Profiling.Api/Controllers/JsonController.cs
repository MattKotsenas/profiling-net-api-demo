using Microsoft.AspNetCore.Mvc;

using Profiling.Api.Services.Json;

namespace Profiling.Api.Controllers;

[ApiController]
[Route("json")]
public class JsonController : ControllerBase
{
    private readonly IJsonService _service;

    public JsonController(IJsonService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_service.Run());
    }
}