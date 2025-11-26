using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace InfiniteInfluence.DataAccessLibrary.Model;


public class AnnouncementApplicant
{
    public int InfluencerUserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string ContactPhoneNumber { get; set; } = string.Empty;
    public string ContactEmailAddress { get; set; } = string.Empty;
    public string ApplicationState { get; set; } = string.Empty;
}
