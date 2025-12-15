using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace InfiniteInfluence.Website.Models;


public class CompanyCreateViewModel
{
    // BaseUser fields (til Users-tabellen)
    [Required]
    [EmailAddress]
    [Display(Name = "Login E-mail")]
    public string LoginEmail { get; set; } = string.Empty;

    // TODO: Here we should use salt and hash later – but we keep it simple for now
    [Required]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;


    // Company fields
    [Required]
    [Display(Name = "Company name")]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Ceo name")]
    public string CeoName { get; set; } = string.Empty;

    [Display(Name = "Date of establishment")]
    //Remote method in companyController to validate date on server side 
    [Remote(action: "ValidateEstablishmentDate", controller: "Company")]
    public DateTime DateOfEstablishment { get; set; }

    [Required]
    [Display(Name = "Organisation Number")]
    public string OrganisationNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Standard Industry Classification")]
    public int StandardIndustryClassification { get; set; }

    [Display(Name = "Company homepage URL")]
    public string? WebsiteUrl { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Company e-mail")]
    public string CompanyEmail { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Contact phone number")]
    public string CompanyPhoneNumber { get; set; } = string.Empty;

    [Required]
    public string Country { get; set; } = string.Empty;

    [Display(Name = "State/Region")]
    [Required]
    public string State { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    [Display(Name = "Contact person")]
    [Required]
    public string ContactPerson { get; set; } = string.Empty;

    [Display(Name = "Contact e-mail address")]
    [EmailAddress]
    [Required]
    public string ContactEmailAddress { get; set; } = string.Empty;

    [Display(Name = "Contact phone number")]
    [Required]
    public string ContactPhoneNumber { get; set; } = string.Empty;




    [Display(Name = "Biography")]
    [MaxLength(1000, ErrorMessage = "Biography cannot exceed 1000 characters.")]
    public string Biography { get; set; } = string.Empty;



    // Company Domains
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
