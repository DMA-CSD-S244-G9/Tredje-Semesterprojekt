using Dapper;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;


public class InfluencerDao : BaseConnectionDao, IInfluencerDao
{
    #region Properties

    #endregion

    #region Constructors
    public InfluencerDao(string dataBaseConnectionString) : base(dataBaseConnectionString)
    {

    }
    #endregion


    public Influencer? GetByUserId(int userId)
    {
        string? queryFindInfluencer = @"
                SELECT
                    u.UserId, u.LoginEmail, u.PasswordHash,

                    i.IsInfluencerVerified, i.VerificationDate,
                    i.DisplayName, i.FirstName, i.LastName,
                    i.ProfileImageUrl, i.Age, i.Gender, i.Country,
                    i.InfluencerState, i.City, i.InfluencerLanguage, i.Biography,
                    i.InstagramProfileUrl, i.InstagramFollowers,
                    i.YouTubeProfileUrl, i.YouTubeFollowers,
                    i.TikTokProfileUrl, i.TikTokFollower,
                    i.SnapchatProfileUrl, i.SnapchatFollowers,
                    i.XProfileUrl, i.XFollowers,
                    i.ContactPhoneNumber, i.ContactEmailAddress

                FROM [Users] u
                INNER JOIN Influencers i ON i.UserId = u.UserId
                WHERE u.UserId = @UserId;";


        string? queryFindInfluencerDomains = @"
                SELECT domain
                FROM InfluencerDomains
                WHERE UserId = @UserId;";



        using IDbConnection connection = CreateConnection();
        {
            // Dapper will be mapping both the BaseUser and the Influencer classes' properties
            Influencer? foundInfluencer = connection.QuerySingleOrDefault<Influencer>(queryFindInfluencer, new { UserId = userId });

            // Returns null if the influencer is not found using the guard clause
            if (foundInfluencer == null)
            {
                return null;
            }

            // If the influencer is found then attempt to find the influencers' influencing domains from the InfluencerDomains table
            IEnumerable<string> domains = connection.Query<string>(queryFindInfluencerDomains, new { UserId = userId });

            // Adds the found domains to the list of influencer domains
            foundInfluencer.InfluencerDomains = domains.ToList();

            return foundInfluencer;
        }
    }


}


