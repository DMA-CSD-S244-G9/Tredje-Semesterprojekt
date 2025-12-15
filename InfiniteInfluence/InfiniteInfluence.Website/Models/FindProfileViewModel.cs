using System.ComponentModel.DataAnnotations;


namespace InfiniteInfluence.Website.Models;


public class FindProfileViewModel
{
    // This is temporary until we have some sort of user validation or login system
    [Required(ErrorMessage = "User Id is required.")]
    [Display(Name = "User Id")]
    [Range(1, int.MaxValue, ErrorMessage = "A valid User Id is required.")]
    public int UserId { get; set; }
}
