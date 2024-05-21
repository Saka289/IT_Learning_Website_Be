using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers;

public class HomeController : ControllerBase
{
    // GET
    public IActionResult Index()
    {
        return Redirect(url: "~/swagger");
    }
}