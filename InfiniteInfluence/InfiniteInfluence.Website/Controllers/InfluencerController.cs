using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
//using InfiniteInfluence.ApiClient;
using Microsoft.AspNetCore.Mvc;


namespace InfiniteInfluence.Website.Controllers
{
    public class InfluencerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
