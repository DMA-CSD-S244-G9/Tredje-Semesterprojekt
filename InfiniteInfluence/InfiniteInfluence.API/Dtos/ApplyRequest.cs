namespace InfiniteInfluence.API.Dtos;


/// <summary>
/// This is a small data transfer object (DTO) which is used when an influencer applies
/// to an announcement. This is sent in the body of a POST /announcements/{id}/apply.
/// </summary>
public class ApplyRequest
{
    public int InfluencerUserId { get; set; }
}
