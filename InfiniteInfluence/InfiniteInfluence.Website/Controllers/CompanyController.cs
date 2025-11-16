using InfiniteInfluence.ApiClient;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using Microsoft.AspNetCore.Mvc;

namespace InfiniteInfluence.Website.Controllers;

public class CompanyController : Controller
{
    ICompanyDao _companyApiClient = new CompanyApiClient("https://localhost:7777");


    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Company company)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        try
        {
            // Din kode her
            _companyApiClient.Create(company);

            // Redirects the user to the specified page corrosponding to endpoint of the supplied action and controller 
            return RedirectToAction("Index", "Home");
        }

        catch (Exception exxception)
        {
            //_logger.LogError(exxception, "Error while creating company from MVC.");

            // Midlertidigt: vis den rigtige fejltekst fra API'et
            ModelState.AddModelError(string.Empty, $"The following api error occured: {exxception.Message}");

            // We pass model so that the view gets all of the InfluencerCreateViewModel's properties and gender selection works and we dont lose all inputted values
            return View();
        }

    }
}
