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
        string? query = @"
                SELECT
                    u.UserId, u.LoginEmail, u.PasswordHash,
                    i.InfluencerId, i.IsInfluencerVerified, i.VerificationDate, i.DisplayName, i.FirstName, i.LastName,
                    i.ProfileImageUrl, i.ListOfInfluencerDomains, i.Age, i.Gender, i.Country, i.State, i.City, i.Languages,
                    i.Biography, i.InstagramProfileUrl, i.InstagramFollowers, i.YouTubeProfileUrl, i.YouTubeFollowers,
                    i.TikTokProfileUrl, i.TikTokFollower, i.SnapchatProfileUrl, i.SnapchatFollowers,
                    i.XProfileUrl, i.XFollowers, i.ContactPhoneNumber, i.ContactEmailAddress

                FROM[User] u
                INNER JOIN Influencer i ON i.UserId = u.UserId
                WHERE u.UserId = @UserId;";


        using IDbConnection connection = CreateConnection();
        {
            // Dapper will be mapping both the BaseUser and the Influencer classes' properties
            Influencer? foundInfluencer = connection.QuerySingleOrDefault<Influencer>(query, new { UserId = userId });

            return foundInfluencer;
        }
    }


}


