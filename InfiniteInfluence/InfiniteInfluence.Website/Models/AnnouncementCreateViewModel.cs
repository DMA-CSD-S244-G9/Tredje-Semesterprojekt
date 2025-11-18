using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InfiniteInfluence.Website.Models
{
    public class AnnouncementCreateViewModel
    {
        // This is temporary until we have some sort of user validation or login system
        [Required]
        [Display(Name = "Company UserId")]
        public int UserId { get; set; }


        [Required]
        [StringLength(32)]
        public string Title { get; set; } = string.Empty;


        // When the announcement should become visible
        [Display(Name = "Start display time")]
        public DateTime? StartDisplayDateTime { get; set; }


        // When the announcement should stop being visible
        [Display(Name = "End display time")]
        public DateTime? EndDisplayDateTime { get; set; }


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
        [StringLength(100)]
        public string CommunicationType { get; set; } = "E-mail Communication";


        [Required]
        [Display(Name = "Language of communication")]
        [StringLength(100)]
        public string AnnouncementLanguage { get; set; } = string.Empty;


        // Payment Section
        [Display(Name = "May influencer keep the products")]
        public bool IsKeepProducts { get; set; } = false;


        [Display(Name = "Is the payment negotiable")]
        public bool IsPayoutNegotiable { get; set; } = false;


        [Required]
        [Display(Name = "Total payment in USD")]
        [Range(0.00, 10000000.00, ErrorMessage = "Total payout amount must be between 0.00 and 10000000.00$")]
        public decimal TotalPayoutAmount { get; set; }


        // Description information
        [Required]
        [Display(Name = "Short descriptive text")]
        [StringLength(100)]
        public string ShortDescriptionText { get; set; } = string.Empty;


        [Required]
        [Display(Name = "Additional information")]
        [StringLength(1500)]
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
    }
}
