using Microsoft.AspNetCore.Mvc;


namespace InfiniteInfluence.Website.Controllers;

public class ErrorController : Controller
{
    [Route("Error/404")]
    public IActionResult NotFoundPage()
    {
        // Returns the view in views/Error/NotFound.cshtml
        return View("NotFound");
    }


    public IActionResult Index()
    {
        return View();
    }
}
