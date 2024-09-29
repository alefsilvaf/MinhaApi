using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ItauController : ControllerBase
{
    private readonly ItauService _itauService;

    public ItauController(ItauService itauService)
    {
        _itauService = itauService;
    }

    [HttpPost]
    public async Task<IActionResult> Post(RequestItauController request)
    {
        try
        {
            var response = await _itauService.CallItauApi(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro: {ex.Message}");
        }
    }
}
