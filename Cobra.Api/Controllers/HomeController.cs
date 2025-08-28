using Microsoft.AspNetCore.Mvc;

namespace Cobra.Api.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}