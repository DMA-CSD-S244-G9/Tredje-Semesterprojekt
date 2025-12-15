using System.ComponentModel.DataAnnotations;


namespace InfiniteInfluence.Website.Models;


/// <summary>
/// Represents the data model for editing an announcement, including details such as title, display dates,
/// communication preferences, payment information, and additional metadata.
/// Is used in AnnouncementController for editing announcements, to capture user input and validate the data before processing.
/// </summary>
/// <remarks>This view model is used to capture and validate user input for updating announcements. 
/// It includes various fields with validation attributes to ensure data integrity, such as required
/// fields, string length constraints, and range limits. The model also supports optional metadata like subjects and
/// flags for payment preferences.</remarks>
public class AnnouncementEditViewModel
{
    public int AnnouncementId { get; set; }

    // This is temporary until we have some sort of user validation or login system
    [Required]
    [Display(Name = "Company User Id")]
    [Range(1, int.MaxValue, ErrorMessage = "A valid company UserId is required.")]

    public int UserId { get; set; }


    [Required]
    [StringLength(32, ErrorMessage = "Title cannot be longer than 32 characters.")]
    [Display(Name = "Announcement Title")]
    public string Title { get; set; } = string.Empty;


    // When the announcement should become visible
    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Start display date and time")]
    public DateTime? StartDisplayDateTime { get; set; }


    // When the announcement should stop being visible
    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "End display date and time")]
    public DateTime? EndDisplayDateTime { get; set; }


    // The maximum amount of applicants an announcement is looking for
    [Required]
    [Display(Name = "Maximum applicants")]
    [Range(1, 25, ErrorMessage = "Maximum applicants must be between 1 and 25.")]
    public int MaximumApplicants { get; set; }


    // The minimum amount of followers required must be 0 or up to 2147483647 which is the maximum value of an integer
    [Required]
    [Display(Name = "Minimum followers to apply")]
    [Range(0, int.MaxValue, ErrorMessage = "Minimum followers required must be between 0 and 2147483647")]
    public int MinimumFollowersRequired { get; set; }


    // Communication type (e.g. Social Media Post, Video, Story)
    [Required]
    [Display(Name = "Communication type")]
    [StringLength(100, ErrorMessage = "Communication type cannot be longer than 100 characters.")]
    public string CommunicationType { get; set; } = "E-mail Communication";


    [Required]
    [Display(Name = "Language of communication")]
    [StringLength(100, ErrorMessage = "Language cannot be longer than 100 characters.")]
    public string AnnouncementLanguage { get; set; } = string.Empty;


    // Payment Section
    [Display(Name = "May the influencer keep the products")]
    public bool IsKeepProducts { get; set; } = false;


    [Display(Name = "Is the payment negotiable")]
    public bool IsPayoutNegotiable { get; set; } = false;


    [Required]
    [Display(Name = "Total payment in USD")]
    [Range(0.00, 1_000_000, ErrorMessage = "Total payout amount must be between 0.00 and 10000000.00$")]
    // We use this regex to limit the user's input to maximum 2 decimal numbers
    [RegularExpression(@"^\d+([.,]\d{1,2})?$", ErrorMessage = "Only numbers with up to two decimals are allowed.")]
    public decimal TotalPayoutAmount { get; set; }


    // Description information
    [Required]
    [Display(Name = "Short descriptive text")]
    [StringLength(100, ErrorMessage = "Short description cannot be longer than 100 characters.")]
    public string ShortDescriptionText { get; set; } = string.Empty;


    [Required]
    [Display(Name = "Additional information")]
    [StringLength(1500, ErrorMessage = "Additional information text cannot be longer than 1500 characters.")]
    public string AdditionalInformationText { get; set; } = string.Empty;


    // Announcement Subjects
    [Display(Name = "Subject #1")]
    [StringLength(20, ErrorMessage = "Subject #1 cannot be longer than 20 characters.")]
    public string? Subject1 { get; set; }


    [Display(Name = "Subject #2")]
    [StringLength(20, ErrorMessage = "Subject #2 cannot be longer than 20 characters.")]
    public string? Subject2 { get; set; }


    [Display(Name = "Subject #3")]
    [StringLength(20, ErrorMessage = "Subject #3 cannot be longer than 20 characters.")]
    public string? Subject3 { get; set; }

    public string RowVersion { get; set; }
}
