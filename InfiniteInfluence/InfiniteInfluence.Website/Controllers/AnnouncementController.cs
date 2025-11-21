using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using InfiniteInfluence.Website.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;


namespace InfiniteInfluence.Website.Controllers;


public class AnnouncementController : Controller
{
    private readonly IAnnouncementDao _announcementApiClient;
    private readonly ILogger<AnnouncementController> _logger;


    // Utilises Dependency injection from the Program.cs 
    public AnnouncementController(IAnnouncementDao announcementApiClient, ILogger<AnnouncementController> logger)
    {
        _announcementApiClient = announcementApiClient;
        _logger = logger;
    }


    // GET: /Announcement/Create
    [HttpGet]
    public IActionResult Create()
    {
        AnnouncementCreateViewModel anouncementCreateViewModel = new AnnouncementCreateViewModel();

        // Adds a default value to the start and end display dates down to dayte/hours/minutes so they are already set when the view is returned
        anouncementCreateViewModel.StartDisplayDateTime = DateTime.Now.AddSeconds(-DateTime.Now.Second).AddMilliseconds(-DateTime.Now.Millisecond);
        anouncementCreateViewModel.EndDisplayDateTime = DateTime.Now.AddSeconds(-DateTime.Now.Second).AddMilliseconds(-DateTime.Now.Millisecond) + TimeSpan.FromDays(28);

        // Sets the default type of communication to be e-mail
        anouncementCreateViewModel.CommunicationType = "E-mail Communication";

        // We pass model so that the view gets all of the AnnouncementCreateViewModel's properties
        return View(anouncementCreateViewModel);
    }


    // POST: Announcement/Create
    [HttpPost]
    public IActionResult Create(AnnouncementCreateViewModel anouncementCreateViewModel)
    {
        // If the MVC validation is invalid then execute this section
        if (!ModelState.IsValid)
        {
            // We pass model so that the view gets all of the AnnouncementCreateViewModel's properties and we dont lose all inputted values
            return View(anouncementCreateViewModel);
        }

        try
        {
            // Converts the input received from the MVC View Model to an Announcement that the API can work with
            Announcement announcement = ConvertFromViewModelToApiAnnouncement(anouncementCreateViewModel);

            int newId = _announcementApiClient.Create(announcement);

            // Defines a success message that is stored in the users session or cookies and then shown
            TempData["SuccessMessage"] = "The announcement was created successfully.";

            // Redirects the user to the specified page corrosponding to endpoint of the supplied action and controller 
            return RedirectToAction("Index", "Home");
        }

        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while creating announcement from MVC.");

            // Shows the correct failure text from the REST Api
            ModelState.AddModelError(string.Empty, $"The following api error occured: {exception.Message}");

            // We pass model so that the view gets all of the AnnouncementCreateViewModel's properties and we dont lose all inputted values
            return View(anouncementCreateViewModel);
        }
    }


    //public IActionResult Index()
    //{
    //    return View();
    //}


    // GET: /Announcement
    public IActionResult Index()
    {
        try
        {
            List<Announcement> listOfAnnouncements = _announcementApiClient.GetAll().ToList();

            // LINQ query to sort announcements by StartDisplayDateTime in descending order
            List<Announcement> announcementSortedByDisplayDate = listOfAnnouncements.OrderByDescending
                (announcement => announcement.StartDisplayDateTime).ToList();
            
            return View(announcementSortedByDisplayDate);
        }

        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while retrieving announcements from MVC.");

            // Shows the correct failure text from the REST Api
            ModelState.AddModelError(string.Empty, $"The following api error occured: {exception.Message}");

            //HACK: ??? note I do not know if this is bad practice but it does the trick for now
            // Returns an empty list of announcements to prevent the view from crashing
            return View(new List<Announcement>());
        }
    }






    private Announcement ConvertFromViewModelToApiAnnouncement(AnnouncementCreateViewModel anouncementCreateViewModel)
    {
        Announcement announcement = new Announcement
        {
            UserId = anouncementCreateViewModel.UserId,

            Title = anouncementCreateViewModel.Title,

            // The creation and last edited date and time should be the time of creation
            CreationDateTime = DateTime.Now,
            LastEditDateTime = DateTime.Now,

            // Upon creation we always want our announcement to have 0 current applicants
            CurrentApplicants = 0,

            // Upon creation the announcement should be shown as open initially
            StatusType = "Open",

            // Upon creation the default visibility state is set to true
            IsVisible = true,

            StartDisplayDateTime = anouncementCreateViewModel.StartDisplayDateTime,
            EndDisplayDateTime = anouncementCreateViewModel.EndDisplayDateTime,

            MaximumApplicants = anouncementCreateViewModel.MaximumApplicants,
            MinimumFollowersRequired = anouncementCreateViewModel.MinimumFollowersRequired,

            CommunicationType = anouncementCreateViewModel.CommunicationType,
            AnnouncementLanguage = anouncementCreateViewModel.AnnouncementLanguage,

            IsKeepProducts = anouncementCreateViewModel.IsKeepProducts,
            IsPayoutNegotiable = anouncementCreateViewModel.IsPayoutNegotiable,

            TotalPayoutAmount = anouncementCreateViewModel.TotalPayoutAmount,

            ShortDescriptionText = anouncementCreateViewModel.ShortDescriptionText,
            AdditionalInformationText = anouncementCreateViewModel.AdditionalInformationText,

            // Associated influencers list stays empty by design (until influencers apply)
            ListOfAssociatedInfluencers = new List<int>()
        };


        // Creates a list of string objects that can hold up to three subject fields
        List<string> subjects = new List<string>();

        AddSubjectIfValid(subjects, anouncementCreateViewModel.Subject1);
        AddSubjectIfValid(subjects, anouncementCreateViewModel.Subject2);
        AddSubjectIfValid(subjects, anouncementCreateViewModel.Subject3);

        // Only assign the subjects if they actually have any values
        if (subjects.Any())
        {
            announcement.ListOfSubjects = subjects;
        }

        return announcement;
    }




    
    //TODO: Since Announcement, Influencer and Company will all use this method
    // or one similar, maybe refactor it to be a universal method that can be used by all 3 classes.

    
    /// <summary>
    /// Takes a raw subject string from the ViewModel, trims it, ensures
    /// it starts with '#', and adds it to the target list if not empty
    /// and not already present (case-insensitive).
    /// </summary>
    private void AddSubjectIfValid(List<string> target, string? rawDomain)
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
