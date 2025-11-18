using InfiniteInfluence.ApiClient;
using InfiniteInfluence.Website.Models;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;

using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InfiniteInfluence.Website.Controllers;


public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    // TODO: Use this to obtain the list of announcement items that should be shown on the page later
//    IInfluencerDao _InfluencerApiClient = new InfluencerApiClient("https://localhost:32773");


    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult ChooseProfileType()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
