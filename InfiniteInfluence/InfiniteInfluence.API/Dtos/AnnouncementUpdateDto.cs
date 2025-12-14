namespace InfiniteInfluence.API.Dtos;


/// <summary>
/// This is a data transfer object (DTO) which is used when updating an announcement.
/// Its properties correspond to the fields that can be updated for an announcement.
/// It ignores applicantions property during JSON serialization to prevent clients
/// </summary>>
public class AnnouncementUpdateDto
{
    public int AnnouncementId { get; set; }
    public string Title { get; set; }
    public DateTime LastEditDateTime { get; set; }
    public DateTime? StartDisplayDateTime { get; set; }
    public DateTime? EndDisplayDateTime { get; set; }
    public int MaximumApplicants { get; set; }
    public int MinimumFollowersRequired { get; set; }
    public string CommunicationType { get; set; }
    public string AnnouncementLanguage { get; set; }
    public bool IsKeepProducts { get; set; }
    public bool IsPayoutNegotiable { get; set; }
    public decimal TotalPayoutAmount { get; set; }
    public List<string> ListOfSubjects { get; set; }
    public string ShortDescriptionText { get; set; }
    public string AdditionalInformationText { get; set; }
    public string StatusType { get; set; }
    public bool IsVisible { get; set; }



    /// <summary>
    /// RowVersion used for optimistic concurrency.
    /// If you want to find the rowversion for an announcement 
    /// go to https://localhost:{api port}/Announcements/{announcementid}/
    /// </summary>
    /// <example>AAAAAAAAB+U=</example>
    // RowVersion as a Base64 string which is used by Swagger and JSON
    public string RowVersion { get; set; }
}
