using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using InfiniteInfluence.Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

///  <summary>
///  This controller handles HTTP requests related to influencer operations in the MVC web application.
///  It uses dependency injection to access the influencer data access object IInfluencerDao and a logger.
///  It uses API Client to communicate with the API.
///  </summary>


namespace InfiniteInfluence.Website.Controllers;


public class InfluencerController : Controller
{
    #region Attributes and Constructor
    private readonly IInfluencerDao _InfluencerApiClient;
    private readonly ILogger<InfluencerController> _logger;


    // Utilises Dependency injection from the Program.cs 
    public InfluencerController(IInfluencerDao influencerClient, ILogger<InfluencerController> logger)
    {
        _InfluencerApiClient = influencerClient;

        // Logger used for logging errors and information about ui context
        _logger = logger;
    }
    #endregion



    #region Create Influencer Profile

    // GET: /Influencer/Create
    [HttpGet]
    public IActionResult Create()
    {
        InfluencerCreateViewModel model = new InfluencerCreateViewModel();
        PopulateGenderOptions(model);

        // We pass model so that the view gets all of the InfluencerCreateViewModel's properties and gender selection works
        return View(model);
    }


    // POST: Influencer/Create
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

            // Defines a success message that is stored in the users session or cookies and then shown
            TempData["SuccessMessage"] = "The influencer profile was successfully created.";

            // Redirects the user to the specified page corrosponding to endpoint of the supplied action and controller 
            return RedirectToAction("Index", "Announcement");
        }

        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while creating influencer from MVC.");

            // Shows the correct failure text from the REST Api
            ModelState.AddModelError(string.Empty, $"The following api error occured: {exception.Message}");

            // When errors occur we recreate the dropdown data the Gender field else we might lose it
            PopulateGenderOptions(influencerModel);

            // We pass model so that the view gets all of the InfluencerCreateViewModel's properties and gender selection works and we dont lose all inputted values
            return View(influencerModel);
        }
    }

    #endregion



    #region Find and show Influencer Profile

    /// <summary>
    /// Displays the influencer profile view.
    /// </summary>
    /// <returns>An IActionResult that renders the influencer profile view.</returns>
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
    /// <returns>
    /// An IActionResult that renders the current view with validation errors if the model state is
    /// invalid, or redirects to the "ViewProfile" action with the specified user ID if the input is valid. 
    /// </returns>
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
            // Attempts to retrieve the influencer profile using the provided userId
            Influencer? influencer = _InfluencerApiClient.GetOne(findProfile.UserId);
        }

        catch
        {
            // This error message is shown in the browser when entering a wrong id in findprofile view for influencer
            ModelState.AddModelError(nameof(findProfile.UserId), "No influencer profile was found with this User Id.");

            // Return the same findprofil view with the error message
            return View(findProfile);
        }

        // If valid, redirect to GET ViewProfile action with userId as route parameter
        return RedirectToAction("ViewProfile", new { userId = findProfile.UserId });
    }


    /// <summary>
    /// Retrieves and displays the profile of a influencer based on the specified user ID.
    /// </summary>
    /// 
    /// <remarks>
    /// This method attempts to retrieve the influencer profile using the provided userId. 
    /// If an error occurs during the retrieval process, the error is logged, and an appropriate error
    /// message is added to the model state for display in the view.
    /// </remarks>
    /// 
    /// <returns>
    /// An IActionResult that renders the influencer profile view if the profile is successfully retrieved;
    /// otherwise, renders the view with an error message if an exception occurs.
    /// </returns>
    [HttpGet]
    public IActionResult ViewProfile(int userId)
    {
        // Call your API with userId
        try
        {
            // Retrieves the influencer profile from the API using the provided userId
            Influencer? influencerProfile = _InfluencerApiClient.GetOne(userId);

            // Passes the retrieved influencer profile to the ViewProfile view for rendering
            return View(influencerProfile);
        }


        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while getting an influencer from MVC.");

            ModelState.AddModelError(string.Empty, $"The following api error occured: {exception.Message}");

            return View();
        }
    }

    #endregion



    #region Helper method

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
    private Influencer ConvertFromViewModelToApiInfluencer(InfluencerCreateViewModel influencerCreateViewModel)
    {
        Influencer influencer = new Influencer
        {
            // BaseUser related fields
            LoginEmail = influencerCreateViewModel.LoginEmail,

            // TODO: This should be hashed later on
            PasswordHash = influencerCreateViewModel.Password,


            // Influencer related fields
            IsInfluencerVerified = false,
            VerificationDate = null,
            DisplayName = influencerCreateViewModel.DisplayName,
            FirstName = influencerCreateViewModel.FirstName,
            LastName = influencerCreateViewModel.LastName,
            ProfileImageUrl = influencerCreateViewModel.ProfileImageUrl,
            Age = influencerCreateViewModel.Age,
            Gender = influencerCreateViewModel.Gender,
            Country = influencerCreateViewModel.Country,
            InfluencerState = influencerCreateViewModel.InfluencerState,
            City = influencerCreateViewModel.City,
            InfluencerLanguage = influencerCreateViewModel.InfluencerLanguage,
            Biography = influencerCreateViewModel.Biography,
            ContactPhoneNumber = influencerCreateViewModel.ContactPhoneNumber,
            ContactEmailAddress = influencerCreateViewModel.ContactEmailAddress,
            InstagramProfileUrl = influencerCreateViewModel.InstagramProfileUrl,
            InstagramFollowers = influencerCreateViewModel.InstagramFollowers,
            YouTubeProfileUrl = influencerCreateViewModel.YouTubeProfileUrl,
            YouTubeFollowers = influencerCreateViewModel.YouTubeFollowers,
            TikTokProfileUrl = influencerCreateViewModel.TikTokProfileUrl,
            TikTokFollower = influencerCreateViewModel.TikTokFollower,
            SnapchatProfileUrl = influencerCreateViewModel.SnapchatProfileUrl,
            SnapchatFollowers = influencerCreateViewModel.SnapchatFollowers,
            XProfileUrl = influencerCreateViewModel.XProfileUrl,
            XFollowers = influencerCreateViewModel.XFollowers,
        };


        // Creates a list of string objects that can hold up to three domain fields
        List<string> domains = new List<string>();

        AddDomainIfValid(domains, influencerCreateViewModel.Domain1);
        AddDomainIfValid(domains, influencerCreateViewModel.Domain2);
        AddDomainIfValid(domains, influencerCreateViewModel.Domain3);

        // Only assign the domains if they actually have any values
        if (domains.Any())
        {
            influencer.InfluencerDomains = domains;
        }

        return influencer;
    }


    // Helper method to set the dropdown values for the gender dropdown 
    // TODO: (Maybe we should add 'other' as an option later?)
    private void PopulateGenderOptions(InfluencerCreateViewModel influencerCreateViewModel)
    {
        influencerCreateViewModel.GenderOptions = new List<SelectListItem>
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

    #endregion

}
