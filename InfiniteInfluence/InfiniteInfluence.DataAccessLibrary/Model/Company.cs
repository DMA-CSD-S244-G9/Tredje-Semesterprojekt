namespace InfiniteInfluence.DataAccessLibrary.Model;

/// <summary>
/// This class represents a Company user in the Infinite Influence system.
/// Inherits from BaseUser and includes properties specific to companies.
/// 
/// @author Line Bertelsen
/// @Date 2025-15-11
/// </summary>
public class Company : BaseUser
{
    #region Properties
    public bool IsCompanyVerified { get; set; }
    public DateTime VerificationDate { get; set; }
    public string CompanyName { get; set; }
    public object CompanyLogoUrl { get; set; }
    public List<string> ListOfCompanyDomains { get; set; }
    public string CeoName { get; set; }
    public DateTime DateOfEstablishment { get; set; }
    public string OrganisationNumber { get; set; }
    public string StandardIndustryClassification { get; set; }
    public object WebsiteUrl { get; set; }
    public string CompanyEmail { get; set; }
    public string CompanyPhoneNumber { get; set; }
    public string Country { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public string Languages { get; set; }
    public string Biography { get; set; }
    public string ContactPerson { get; set; }
    public string ContactEmailAddress { get; set; }
    public string ContactPhoneNumber { get; set; }
    #endregion

    #region Constructor

    public Company()
    {
    }

    /// <summary>
    /// Construtor to create a new Company before they have an UserId
    /// It inherits LoginEmail and PasswordHash from BaseUser
    /// </summary>
    public Company(string loginEmail, string passwordHash, bool isCompanyVerified, DateTime verificationDate, string companyName, object companyLogoUrl, List<string> listOfCompanyDomains, string ceoName, DateTime dateOfEstablishment, string organisationNumber, string standardIndustryClassification, object websiteUrl, string companyEmail, string companyPhoneNumber, string country, string state, string city, string address, string languages, string biography, string contactPerson, string contactEmailAddress, string contactPhoneNumber) 
    {
        LoginEmail = loginEmail;
        PasswordHash = passwordHash;
        IsCompanyVerified = isCompanyVerified;
        VerificationDate = verificationDate;
        CompanyName = companyName;
        CompanyLogoUrl = companyLogoUrl;
        ListOfCompanyDomains = listOfCompanyDomains;
        CeoName = ceoName;
        DateOfEstablishment = dateOfEstablishment;
        OrganisationNumber = organisationNumber;
        StandardIndustryClassification = standardIndustryClassification;
        WebsiteUrl = websiteUrl;
        CompanyEmail = companyEmail;
        CompanyPhoneNumber = companyPhoneNumber;
        Country = country;
        State = state;
        City = city;
        Address = address;
        Languages = languages;
        Biography = biography;
        ContactPerson = contactPerson;
        ContactEmailAddress = contactEmailAddress;
        ContactPhoneNumber = contactPhoneNumber;
    }

    /// <summary>
    /// To create a Company with an userId when retrieving from the database)
    /// It inherits UserId, LoginEmail and PasswordHash from BaseUser
    /// </summary>
    public Company(int userId, string email, string passwordHash, bool isCompanyVerified, DateTime verificationDate, string companyName, object companyLogoUrl, List<string> listOfCompanyDomains, string ceoName, DateTime dateOfEstablishment, string organisationNumber, string standardIndustryClassification, object websiteUrl, string companyEmail, string companyPhoneNumber, string country, string state, string city, string address, string languages, string biography, string contactPerson, string contactEmailAddress, string contactPhoneNumber) 
    {
        UserId = userId;
        LoginEmail = email;
        PasswordHash = passwordHash;
        IsCompanyVerified = isCompanyVerified;
        VerificationDate = verificationDate;
        CompanyName = companyName;
        CompanyLogoUrl = companyLogoUrl;
        ListOfCompanyDomains = listOfCompanyDomains;
        CeoName = ceoName;
        DateOfEstablishment = dateOfEstablishment;
        OrganisationNumber = organisationNumber;
        StandardIndustryClassification = standardIndustryClassification;
        WebsiteUrl = websiteUrl;
        CompanyEmail = companyEmail;
        CompanyPhoneNumber = companyPhoneNumber;
        Country = country;
        State = state;
        City = city;
        Address = address;
        Languages = languages;
        Biography = biography;
        ContactPerson = contactPerson;
        ContactEmailAddress = contactEmailAddress;
        ContactPhoneNumber = contactPhoneNumber;
    } 
    #endregion
}