using InfiniteInfluence.ApiClient;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;
using InfiniteInfluence.DataAccessLibrary.Model;
using InfiniteInfluence.Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

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

    #region Create Influencer Profile
    /// <summary>
    /// Displays the form for creating a new company.
    /// GET: /Company/Create
    /// </summary>
    /// 
    /// <remarks>
    /// Initializes the CompanyCreateViewModel with default values, including setting
    /// the <c>DateOfEstablishment</c> field to the current date. This ensures the form does not default to an invalid
    /// or unexpected date.
    /// </remarks>
    /// 
    /// <returns>
    /// An IActionResult that renders the "Create" view with the pre-populated CompanyCreateViewModel
    /// </returns>
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

    /// <summary>
    /// Handles the creation of a new company profile based on the provided view model.
    /// </summary>
    /// 
    /// <remarks>
    /// This method validates the input model, converts it to a format suitable for the API, and
    /// attempts to create a new company profile using the API client. If the operation is successful, a success message
    /// is stored in TempData and the user is redirected to the home page. If an error occurs, the
    /// exception details are logged, and an error message is added to the model state.
    /// </remarks>
    /// 
    /// <returns>
    /// An IActionResult that renders the view with validation errors if the model state is invalid,
    /// redirects to the home page upon successful creation, or re-renders the view with an error message if an
    /// exception occurs.
    /// </returns>
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
    #endregion

    #region Find and show company Profile

    [HttpGet]
    public IActionResult FindProfile()
    {
        return View();
    }

    /// <summary>
    /// Processes a request to find a user profile based on the provided input model findprofile.
    /// </summary>
    /// 
    /// <remarks>
    /// This method validates the input model before proceeding. If the model state is invalid, the
    /// method returns the current view with the validation errors displayed. Otherwise, it redirects to the
    /// "ViewProfile" action, passing the user ID as a route parameter.
    /// </remarks>
    /// 
    /// <returns>An IActionResult that renders the current view with validation errors if the model state is
    /// invalid, or redirects to the "ViewProfile" action with the specified user ID if the input is valid
    /// .</returns>

    [HttpPost]
    public IActionResult FindProfile(FindProfileViewModel findProfile)
    {
        //Runs validation checks
        if (!ModelState.IsValid)
        {
            return View(findProfile);
        }

        // Check if userId exists
        try
        {
            Company company = _CompanyApiClient.GetOne(findProfile.UserId);
        }
        catch
        {
            ModelState.AddModelError(nameof(findProfile.UserId), "No company profile was found with this User Id.");
            return View(findProfile);
        }

        // If valid, redirect to GET
        return RedirectToAction("ViewProfile", new { userId = findProfile.UserId });
    }


    /// <summary>
    /// Retrieves and displays the profile of a company based on the specified user ID.
    /// </summary>
    /// 
    /// <remarks>
    /// This method attempts to retrieve the company profile using the provided 
    /// <param name="userId"/>. If an error occurs during the retrieval process, the error is logged, and an appropriate error
    /// message is added to the model state for display in the view.
    /// </remarks>
    /// 
    /// <param name="userId">
    /// The unique identifier of the user whose company profile is to be retrieved.
    /// </param>
    /// 
    /// <returns>
    /// An <see cref="IActionResult"/> that renders the company profile view if the profile is successfully retrieved;
    /// otherwise, renders the view with an error message if an exception occurs.
    /// </returns>
    [HttpGet]
    public IActionResult ViewProfile(int userId)
    {
        // kald dit API med userId
        try
        {
            Company? companyProfile = _CompanyApiClient.GetOne(userId);
            return View(companyProfile);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while getting an company from MVC.");

            // Shows the correct failure text from the REST Api
            ModelState.AddModelError(string.Empty, $"The following api error occured: {exception.Message}");

            // We pass model so that the view gets all of the InfluencerCreateViewModel's properties and gender selection works and we dont lose all inputted values
            return View();
        }
    }
    #endregion

    #region Helper methods

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
    /// 
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

    /// <summary>
    /// Remote method used to validate the date of establishment on the server side.
    /// </summary>
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
