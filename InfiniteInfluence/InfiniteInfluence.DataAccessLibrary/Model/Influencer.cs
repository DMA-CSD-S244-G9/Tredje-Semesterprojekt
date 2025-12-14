namespace InfiniteInfluence.DataAccessLibrary.Model;


public class Influencer : BaseUser
{
    #region Properties
    public bool IsInfluencerVerified { get; set; }
    public DateTime? VerificationDate { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ProfileImageUrl { get; set; } = string.Empty;

    public List<string> InfluencerDomains { get; set; } = new List<string>();

    public int? Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string InfluencerState { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string InfluencerLanguage { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;


    public string? InstagramProfileUrl { get; set; } = string.Empty;
    public int InstagramFollowers { get; set; }


    public string? YouTubeProfileUrl { get; set; } = string.Empty;
    public int YouTubeFollowers { get; set; }


    public string? TikTokProfileUrl { get; set; } = string.Empty;
    public int TikTokFollower { get; set; }


    public string? SnapchatProfileUrl { get; set; } = string.Empty;
    public int SnapchatFollowers { get; set; }


    public string? XProfileUrl { get; set; } = string.Empty;
    public int XFollowers { get; set; }


    public string ContactPhoneNumber { get; set; } = string.Empty;
    public string ContactEmailAddress { get; set; } = string.Empty;
    #endregion



    #region Constructors
    /// <summary>
    /// This default constructor is necessary to ensure deserialization.
    /// The lack of the default constructor was what lead issues during one of the live code project sessions.
    /// </summary>
    public Influencer()
    {

    }



    public Influencer(int userId, string loginEmail, string passwordHash, bool isInfluencerVerified, DateTime? verificationDate, string displayName, string firstName, string lastName, string profileImageUrl, IEnumerable<string> influencerDomains, int? age, string gender, string country, string influencerState, string city, string influencerLanguage, string biography, string instagramProfileUrl, int instagramFollowers, string youTubeProfileUrl, int youTubeFollowers, string tikTokProfileUrl, int tikTokFollower, string snapchatProfileUrl, int snapchatFollowers, string xProfileUrl, int xFollowers, string contactPhoneNumber, string contactEmailAddress)
    {
        // BaseUser
        UserId = userId;
        LoginEmail = loginEmail;
        PasswordHash = passwordHash;


        // The actual influencer
        IsInfluencerVerified = isInfluencerVerified;
        VerificationDate = verificationDate;

        DisplayName = displayName;
        FirstName = firstName;
        LastName = lastName;
        ProfileImageUrl = profileImageUrl;

        Age = age;
        Gender = gender;
        Country = country;
        InfluencerState = influencerState;
        City = city;
        InfluencerLanguage = influencerLanguage;

        Biography = biography;

        InstagramProfileUrl = instagramProfileUrl;
        InstagramFollowers = instagramFollowers;

        YouTubeProfileUrl = youTubeProfileUrl;
        YouTubeFollowers = youTubeFollowers;

        TikTokProfileUrl = tikTokProfileUrl;
        TikTokFollower = tikTokFollower;

        SnapchatProfileUrl = snapchatProfileUrl;
        SnapchatFollowers = snapchatFollowers;

        XProfileUrl = xProfileUrl;
        XFollowers = xFollowers;

        ContactPhoneNumber = contactPhoneNumber;
        ContactEmailAddress = contactEmailAddress;


        if (influencerDomains != null)
        {
            InfluencerDomains = influencerDomains.ToList();
        }

        else
        {
            InfluencerDomains = new List<string>();
        }
    }



    public Influencer(bool isInfluencerVerified, DateTime? verificationDate, string displayName, string firstName, string lastName, string profileImageUrl, IEnumerable<string> influencerDomains, int? age, string gender, string country, string influencerState, string city, string influencerLanguage, string biography, string instagramProfileUrl, int instagramFollowers, string youTubeProfileUrl, int youTubeFollowers, string tikTokProfileUrl, int tikTokFollower, string snapchatProfileUrl, int snapchatFollowers, string xProfileUrl, int xFollowers, string contactPhoneNumber, string contactEmailAddress)
    {
        // The actual influencer
        IsInfluencerVerified = isInfluencerVerified;
        VerificationDate = verificationDate;

        DisplayName = displayName;
        FirstName = firstName;
        LastName = lastName;
        ProfileImageUrl = profileImageUrl;

        Age = age;
        Gender = gender;
        Country = country;
        InfluencerState = influencerState;
        City = city;
        InfluencerLanguage = influencerLanguage;

        Biography = biography;

        InstagramProfileUrl = instagramProfileUrl;
        InstagramFollowers = instagramFollowers;

        YouTubeProfileUrl = youTubeProfileUrl;
        YouTubeFollowers = youTubeFollowers;

        TikTokProfileUrl = tikTokProfileUrl;
        TikTokFollower = tikTokFollower;

        SnapchatProfileUrl = snapchatProfileUrl;
        SnapchatFollowers = snapchatFollowers;

        XProfileUrl = xProfileUrl;
        XFollowers = xFollowers;

        ContactPhoneNumber = contactPhoneNumber;
        ContactEmailAddress = contactEmailAddress;


        if (influencerDomains != null)
        {
            InfluencerDomains = influencerDomains.ToList();
        }

        else
        {
            InfluencerDomains = new List<string>();
        }
    }
    #endregion
}
