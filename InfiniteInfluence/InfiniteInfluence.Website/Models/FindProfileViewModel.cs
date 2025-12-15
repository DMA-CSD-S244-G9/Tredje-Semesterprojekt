using System.ComponentModel.DataAnnotations;


namespace InfiniteInfluence.Website.Models;

/// <summary>
/// Represents the data required to find a user profile by user identifier.
/// This view model includes validation attributes to ensure that the user ID is provided and valid.
/// This is used in the FindProfile action of company and influencer controller.
/// </summary>
public class FindProfileViewModel
{
    // This is temporary until we have some sort of user validation or login system
    [Required(ErrorMessage = "User Id is required.")]
    [Display(Name = "User Id")]
    [Range(1, int.MaxValue, ErrorMessage = "A valid User Id is required.")]
    public int UserId { get; set; }
}
