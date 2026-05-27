using Kaffebar.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kaffebar.Controllers;

[ApiController]
[Route("menu")]
public class MenuController : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult GetMenu() => Ok(new[]
    {
        new Coffee(Guid.NewGuid(), "Kaffe Latte", 48.50m),
        new Coffee(Guid.NewGuid(), "Cappuccino", 45.00m),
        new Coffee(Guid.NewGuid(), "Espresso", 35.00m)
    });
}
