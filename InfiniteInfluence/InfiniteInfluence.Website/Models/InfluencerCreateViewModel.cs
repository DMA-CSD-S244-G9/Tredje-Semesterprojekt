using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;


namespace InfiniteInfluence.Website.Models;


public class InfluencerCreateViewModel
{
    // BaseUser fields (til Users-tabellen)
    [Required]
    [EmailAddress]
    [Display(Name = "Login E-mail")]
    public string LoginEmail { get; set; } = string.Empty;


    //TODO: Maybe implement salting and hashing later
    [Required]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;


    // Influencer fields
    [Required]
    [Display(Name = "User Display name")]
    public string DisplayName { get; set; } = string.Empty;


    [Required]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;


    [Required]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;


    [Display(Name = "Profile image URL")]
    public string ProfileImageUrl { get; set; } = string.Empty;


    [Required]
    [Range(18, 200, ErrorMessage = "Age must be between 18 and 200.")]
    public int Age { get; set; }


    [Required]
    public string Gender { get; set; } = string.Empty;


    // Dropdown options til Gender
    public List<SelectListItem> GenderOptions { get; set; } = new List<SelectListItem>();


    [Required]
    public string Country { get; set; } = string.Empty;


    [Display(Name = "State/Region")]
    public string InfluencerState { get; set; } = string.Empty;


    [Required]
    public string City { get; set; } = string.Empty;


    [Display(Name = "Language")]
    public string InfluencerLanguage { get; set; } = string.Empty;


    [Display(Name = "Biography")]
    [MaxLength(1000, ErrorMessage = "Biography cannot exceed 1000 characters.")]
    public string Biography { get; set; } = string.Empty;


    // Personal contact info
    [Required]
    [Display(Name = "Contact phone number")]
    public string ContactPhoneNumber { get; set; } = string.Empty;


    [Required]
    [EmailAddress]
    [Display(Name = "Contact email address")]
    public string ContactEmailAddress { get; set; } = string.Empty;


    // Social media platforms & Followers
    [Display(Name = "Instagram profile URL")]
    public string? InstagramProfileUrl { get; set; } = string.Empty;
    public int InstagramFollowers { get; set; }


    [Display(Name = "YouTube profile URL")]
    public string? YouTubeProfileUrl { get; set; } = string.Empty;
    public int YouTubeFollowers { get; set; }


    [Display(Name = "TikTok profile URL")]
    public string? TikTokProfileUrl { get; set; } = string.Empty;
    public int TikTokFollower { get; set; }


    [Display(Name = "Snapchat profile URL")]
    public string? SnapchatProfileUrl { get; set; } = string.Empty;
    public int SnapchatFollowers { get; set; }


    [Display(Name = "X (Twitter) profile URL")]
    public string? XProfileUrl { get; set; } = string.Empty;
    public int XFollowers { get; set; }


    // Influencer Domains
    [Display(Name = "Domain #1")]
    [StringLength(20, ErrorMessage = "Domain #1 cannot be longer than 20 characters.")]
    public string? Domain1 { get; set; }


    [Display(Name = "Domain #2")]
    [StringLength(20, ErrorMessage = "Domain #2 cannot be longer than 20 characters.")]
    public string? Domain2 { get; set; }


    [Display(Name = "Domain #3")]
    [StringLength(20, ErrorMessage = "Domain #3 cannot be longer than 20 characters.")]
    public string? Domain3 { get; set; }
}
