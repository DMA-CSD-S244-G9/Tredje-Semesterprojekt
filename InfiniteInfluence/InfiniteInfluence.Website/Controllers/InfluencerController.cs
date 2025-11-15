using InfiniteInfluence.ApiClient;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using InfiniteInfluence.Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Reflection;
using System.Security.Claims;


namespace InfiniteInfluence.Website.Controllers;


public class InfluencerController : Controller
{
    private readonly IInfluencerDao _InfluencerApiClient;
    private readonly ILogger<InfluencerController> _logger;

    
    // Utilises Dependency injection from the Program.cs 
    public InfluencerController(IInfluencerDao influencerClient, ILogger<InfluencerController> logger)
    {
        _InfluencerApiClient = influencerClient;
        _logger = logger;
    }


    // GET: /Influencer/Create
    [HttpGet]
    public IActionResult Create()
    {
        var model = new InfluencerCreateViewModel();
        PopulateGenderOptions(model);

        // We pass model so that the view gets all of the InfluencerCreateViewModel's properties and gender selection works
        return View(model);
    }


    // POST: InfluencerController/Create
    [HttpPost]
    public IActionResult Create(InfluencerCreateViewModel influencerModel)
    {
        // If the MVC validation is invalid then execute this section
        if (!ModelState.IsValid)
        {
            // When errors occur we recreate the dropdown data the Gender field else we might lose it
            PopulateGenderOptions(influencerModel);

            // We pass model so that the view gets all of the InfluencerCreateViewModel's properties and gender selection works and we dont lose all inputted values
            return View(influencerModel);
        }

        try
        {
            // Converts the input received from the MVC View Model to an Influencer that the API can work with
            Influencer influencer = ConvertFromViewModelToApiInfluencer(influencerModel);

            int newUserId = _InfluencerApiClient.Create(influencer);

            // Defines a success message that can be that is stored in the users session or cookies and then shown 
            TempData["SuccessMessage"] = "Influencer was created successfully.";

            // Redirects the user to the specified page corrosponding to endpoint of the supplied action and controller 
            return RedirectToAction("Index", "Home");
        }

        catch (Exception exxception)
        {
            _logger.LogError(exxception, "Error while creating influencer from MVC.");

            // Midlertidigt: vis den rigtige fejltekst fra API'et
            ModelState.AddModelError(string.Empty, $"The following api error occured: {exxception.Message}");

            // When errors occur we recreate the dropdown data the Gender field else we might lose it
            PopulateGenderOptions(influencerModel);

            // We pass model so that the view gets all of the InfluencerCreateViewModel's properties and gender selection works and we dont lose all inputted values
            return View(influencerModel);
        }
    }


    public IActionResult Index()
    {
        return View();
    }


    /// <summary>
    /// Converts the incoming MVC form ViewModel into the Influencer model 
    /// expected by the API layer. 
    ///
    /// This method takes the raw user input from the Create form (ViewModel),
    /// copies the corresponding inputted values into a new Influencer object that matches 
    /// the API's required structure, and applies default values to fields that 
    /// should not be controlled by the UI such as verification date and verification status.
    ///
    /// This mapping is necessary because the ViewModel and API model intentionally 
    /// differ in structure, validation rules, and allowed fields.
    /// </summary>
    /// 
    /// <param name="model">The data submitted from the MVC Create form.</param>
    /// <returns>An Influencer object ready to be sent to the API for creation.</returns>
    private Influencer ConvertFromViewModelToApiInfluencer(InfluencerCreateViewModel model)
    {
        var influencer = new Influencer
        {
            // BaseUser related fields
            LoginEmail = model.LoginEmail,
            // TODO: This should be hashed later on
            PasswordHash = model.Password,


            // Influencer related fields
            IsInfluencerVerified = false,
            VerificationDate = null,
            DisplayName = model.DisplayName,
            FirstName = model.FirstName,
            LastName = model.LastName,
            ProfileImageUrl = model.ProfileImageUrl,
            Age = model.Age,
            Gender = model.Gender,
            Country = model.Country,
            InfluencerState = model.InfluencerState,
            City = model.City,
            InfluencerLanguage = model.InfluencerLanguage,
            Biography = model.Biography,
            ContactPhoneNumber = model.ContactPhoneNumber,
            ContactEmailAddress = model.ContactEmailAddress,
            InstagramProfileUrl = model.InstagramProfileUrl,
            InstagramFollowers = model.InstagramFollowers,
            YouTubeProfileUrl = model.YouTubeProfileUrl,
            YouTubeFollowers = model.YouTubeFollowers,
            TikTokProfileUrl = model.TikTokProfileUrl,
            TikTokFollower = model.TikTokFollower,
            SnapchatProfileUrl = model.SnapchatProfileUrl,
            SnapchatFollowers = model.SnapchatFollowers,
            XProfileUrl = model.XProfileUrl,
            XFollowers = model.XFollowers,
        };


        // Creates a list of string objects that can hold up to three domain fields
        List<string> domains = new List<string>();

        AddDomainIfValid(domains, model.Domain1);
        AddDomainIfValid(domains, model.Domain2);
        AddDomainIfValid(domains, model.Domain3);

        // Only assign the domains if they actually have any values
        if (domains.Any())
        {
            influencer.InfluencerDomains = domains;
        }

        return influencer;
    }


    // Helper method to set the dropdown values for the gender dropdown 
    // TODO: (Maybe we should add 'other' as an option later?)
    private void PopulateGenderOptions(InfluencerCreateViewModel model)
    {
        model.GenderOptions = new List<SelectListItem>
        {
            new SelectListItem { Text = "Male", Value = "Male" },
            new SelectListItem { Text = "Female", Value = "Female" }
        };
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
}
