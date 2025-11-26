using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfiniteInfluence.DataAccessLibrary.Model;


public class Announcement
{
    // This is the primary key in the Announcements table
    public int AnnouncementId { get; set; }

    // This is used as the foreign key to the company table
    public int UserId { get; set; }

    // NOTE: The CompanyName is not saved in the database it is simply a derived field that we fetch through joining of tables in the AnnouncementDao class 
    public string? CompanyName { get; set; }


    public string Title { get; set; } = string.Empty;

    // These are obtained from company and not here
    // public string CompanyName { get; set; }
    // public string CompanyLogoUrl { get; set; }


    public DateTime? CreationDateTime { get; set; }
    public DateTime? LastEditDateTime { get; set; }
    public DateTime? StartDisplayDateTime { get; set; }
    public DateTime? EndDisplayDateTime { get; set; }


    public int CurrentApplicants { get; set; }
    public int MaximumApplicants { get; set; }
    public int MinimumFollowersRequired { get; set; }
    public string CommunicationType { get; set; } = string.Empty;
    public string AnnouncementLanguage { get; set; } = string.Empty;


    public bool IsKeepProducts { get; set; }
    public bool IsPayoutNegotiable { get; set; }
    public decimal TotalPayoutAmount { get; set; }


    public List<string> ListOfSubjects { get; set; } = new();
    public string ShortDescriptionText { get; set; } = string.Empty;
    public string AdditionalInformationText { get; set; } = string.Empty;

    //public string CompanyContactPerson { get; set; } = string.Empty;
    //public string CompanyContactEmailAddress { get; set; } = string.Empty;
    //public string CompanyContactPhoneNumber { get; set; } = string.Empty;


    public string StatusType { get; set; } = string.Empty;
    public bool IsVisible { get; set; }


    // The list of associated influencers are instantiated as an empty list as we
    // do only want an empty list upon creation of an announcement.
    // And population of the list will occur when influencers can begin to apply to announcements.
    public List<int> ListOfAssociatedInfluencers { get; set; } = new List<int>();


    // Llist of applicants for this announcement
    public List<AnnouncementApplicant> Applicants { get; set; } = new List<AnnouncementApplicant>();

}
