using InfiniteInfluence.ApiClient;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using InfiniteInfluence.Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfiniteInfluence.Website.Controllers;

public class CompanyController : Controller
{
    //ICompanyDao _companyApiClient = new CompanyApiClient("https://localhost:7777");
    private readonly ICompanyDao _CompanyApiClient;
    private readonly ILogger<CompanyController> _logger;

    // Utilises Dependency injection from the Program.cs 
    public CompanyController(ICompanyDao companyApiClient, ILogger<CompanyController> logger)
    {
        _CompanyApiClient = companyApiClient;
        _logger = logger;
    }

    // GET: /Company/Create
    [HttpGet]
    public IActionResult Create()
    {
        var model = new CompanyCreateViewModel
        {
            //Adds a default value of today datetime, to the DateOfEstablishment field, so it doesnt start at year 1
            DateOfEstablishment = DateTime.Today
        };

        // We pass model so that the view gets all of the InfluencerCreateViewModel's properties and gender selection works
        return View(model);
    }


    [HttpPost]
    public IActionResult Create(CompanyCreateViewModel companyModel)
    {
        // If the MVC validation is invalid then execute this section
        if (!ModelState.IsValid)
        {
            // We pass model so that the view gets all of the InfluencerCreateViewModel's properties and gender selection works and we dont lose all inputted values
            return View(companyModel);
        }

        try
        {
            // Converts the input received from the MVC View Model to an Influencer that the API can work with
            Company company = ConvertFromViewModelToApiInfluencer(companyModel);

            int newUserId = _CompanyApiClient.Create(company);

            // Defines a success message that can be that is stored in the users session or cookies and then shown 
            TempData["SuccessMessage"] = "The company profile was successfully created.";

            // Redirects the user to the specified page corrosponding to endpoint of the supplied action and controller 
            return RedirectToAction("Index", "Home");
        }

        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while creating company from MVC.");

            // Midlertidigt: vis den rigtige fejltekst fra API'et
            ModelState.AddModelError(string.Empty, $"The following api error occured: {exception.Message}");

            // We pass model so that the view gets all of the InfluencerCreateViewModel's properties and gender selection works and we dont lose all inputted values
            return View(companyModel);
        }
    }

    /// <summary>
    /// Converts the incoming MVC form ViewModel into the Company model 
    /// expected by the API layer. 
    ///
    /// This method takes the raw user input from the Create form (ViewModel),
    /// copies the corresponding inputted values into a new Company object that matches 
    /// the API's required structure, and applies default values to fields that 
    /// should not be controlled by the UI such as verification date and verification status.
    ///
    /// This mapping is necessary because the ViewModel and API model intentionally 
    /// differ in structure, validation rules, and allowed fields.
    /// </summary>
    /// 
    /// <param name="model">The data submitted from the MVC Create form.</param>
    /// <returns>A Company object ready to be sent to the API for creation.</returns>
    private Company ConvertFromViewModelToApiInfluencer(CompanyCreateViewModel model)
    {
        var company = new Company
        {
            // BaseUser related fields
            LoginEmail = model.LoginEmail,
            // TODO: This should be hashed later on
            PasswordHash = model.Password,


            // Company related fields
            IsCompanyVerified = false,
            VerificationDate = null,
            CompanyName = model.CompanyName,
            CompanyLogoUrl = null,
            CeoName = model.CeoName,

            DateOfEstablishment = model.DateOfEstablishment,
            OrganisationNumber = model.OrganisationNumber,
            StandardIndustryClassification = model.StandardIndustryClassification,
            WebsiteUrl = model.WebsiteUrl,

            CompanyEmail = model.CompanyEmail,
            CompanyPhoneNumber = model.CompanyPhoneNumber,

            Country = model.Country,
            CompanyState = model.State,
            City = model.City,
            CompanyAddress = model.Address,
            CompanyLanguage = model.Language,

            ContactPerson = model.ContactPerson,
            ContactEmailAddress = model.ContactEmailAddress,
            ContactPhoneNumber = model.ContactPhoneNumber,
            Biography = model.Biography
        };


        // Creates a list of string objects that can hold up to three domain fields
        List<string> domains = new List<string>();

        AddDomainIfValid(domains, model.Domain1);
        AddDomainIfValid(domains, model.Domain2);
        AddDomainIfValid(domains, model.Domain3);

        // Only assign the domains if they actually have any values
        if (domains.Any())
        {
            company.CompanyDomains = domains;
        }

        return company;
    }

    #region Helpter methods

    /// <summary>
    /// Takes a raw domain string from the ViewModel, trims it, ensures
    /// it starts with '#', and adds it to the target list if not empty
    /// and not already present (case-insensitive).
    /// </summary>
    private void AddDomainIfValid(List<string> target, string? rawDomain)
    {
        // Checks if the data from the input field is empty in which case we simply skip
        if (string.IsNullOrWhiteSpace(rawDomain))
        {
            return;
        }

        // Trimps the inputted values from the field so there are no spaces to make it a proper hashtag
        string value = rawDomain.Trim();

        // Makes it so that the inputted string always starts with #
        if (!value.StartsWith("#"))
        {
            value = "#" + value;
        }

        // Prevent duplicates and also makes sure it is checked for using case insensitivity
        if (!target.Contains(value, StringComparer.OrdinalIgnoreCase))
        {
            target.Add(value);
        }
    }

    // Remote method used in CompanyCreateViewModel to validate date on server side 
    public IActionResult ValidateEstablishmentDate(DateTime dateOfEstablishment)
    {
        if (dateOfEstablishment >= DateTime.Today)
        {
            return Json("Date must be before today.");
        }

        return Json(true);
    }
    #endregion

}
