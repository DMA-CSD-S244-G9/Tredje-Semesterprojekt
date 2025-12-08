using Dapper;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using InfiniteInfluence.DataAccessLibrary.Tools;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;


namespace InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;


public class InfluencerDao : BaseConnectionDao, IInfluencerDao
{
    #region Constructors
    public InfluencerDao(string dataBaseConnectionString) : base(dataBaseConnectionString)
    {

    }
    #endregion


    #region SQL Query - Create Influencer
    // SQL statement for inserting a new user and retrieving the new UserId
    private readonly string? queryInsertUser = @"
        INSERT INTO [Users] (loginEmail, passwordHash)
        VALUES (@LoginEmail, @PasswordHash);

        SELECT CAST(SCOPE_IDENTITY() AS INT); ";
    // ^-- We use Scope_Identity to retrieve the new ID that was created


    // SQL statement for inserting the influencer specific details
    private readonly string? queryInsertInfluencer = @"
        INSERT INTO Influencers (
            userId, isInfluencerVerified, verificationDate,
            displayName, firstName, lastName, profileImageUrl, 
            age, gender, country, influencerState, city, 
            influencerLanguage, biography,
            instagramProfileUrl, instagramFollowers,
            youTubeProfileUrl, youTubeFollowers,
            tikTokProfileUrl, tikTokFollower,
            snapchatProfileUrl, snapchatFollowers,
            xProfileUrl, xFollowers,
            contactPhoneNumber, contactEmailAddress)

        VALUES (
            @UserId, @IsInfluencerVerified, @VerificationDate,
            @DisplayName, @FirstName, @LastName, @ProfileImageUrl,
            @Age, @Gender, @Country, @InfluencerState, @City,
            @InfluencerLanguage, @Biography,
            @InstagramProfileUrl, @InstagramFollowers,
            @YouTubeProfileUrl, @YouTubeFollowers,
            @TikTokProfileUrl, @TikTokFollower,
            @SnapchatProfileUrl, @SnapchatFollowers,
            @XProfileUrl, @XFollowers,
            @ContactPhoneNumber, @ContactEmailAddress); ";


    // SQL for inserting domains into the InfluencerDomains table
    string? queryInsertInfluencerDomain = @"
        INSERT INTO InfluencerDomains (userId, domain)
        VALUES (@UserId, @Domain); ";
    #endregion


    #region SQL Query - Get One 
    private readonly string? queryFindInfluencer = @"
                SELECT
                    u.userId, u.loginEmail, u.passwordHash,

                    i.isInfluencerVerified, i.verificationDate,
                    i.displayName, i.firstName, i.lastName,
                    i.profileImageUrl, i.age, i.gender, i.country,
                    i.influencerState, i.city, i.influencerLanguage, i.biography,
                    i.instagramProfileUrl, i.instagramFollowers,
                    i.youTubeProfileUrl, i.youTubeFollowers,
                    i.tikTokProfileUrl, i.tikTokFollower,
                    i.snapchatProfileUrl, i.snapchatFollowers,
                    i.xProfileUrl, i.xFollowers,
                    i.contactPhoneNumber, i.contactEmailAddress

                FROM [Users] u
                INNER JOIN Influencers i ON i.userId = u.userId
                WHERE u.userId = @UserId;";


    private readonly string? queryFindInfluencerDomains = @"
                SELECT domain
                FROM InfluencerDomains
                WHERE userId = @UserId;";

    #endregion


    #region Create Influencer
    public int Create(Influencer influencer)
    {

        using IDbConnection connection = CreateConnection();
        connection.Open();


        // Begins a transaction Since we have to make changes by performing multiple queries we have to
        // use a transaction to ensure that all inserts succeed together or fail together thereby enforcing atomicity
        using IDbTransaction transaction = connection.BeginTransaction();


        try
        {
            // Hashes the password before storing it in the database
            string passwordHash = BCryptTool.HashPassword(influencer.PasswordHash);

            // Uses dapper to insert into the Users table and return the newest generated UserId using SCOPE_IDENTITY()
            int newUserId = connection.QuerySingle<int>(queryInsertUser, new { influencer.LoginEmail, passwordHash }, transaction);

            // Assigns the influencer's UserId to match the newly generated UserId
            influencer.UserId = newUserId;


            // Inserts the influencer specific data in to table using the UserId
            connection.Execute(queryInsertInfluencer, new
            {
                UserId = influencer.UserId,
                influencer.IsInfluencerVerified,
                influencer.VerificationDate,
                influencer.DisplayName,
                influencer.FirstName,
                influencer.LastName,
                influencer.ProfileImageUrl,
                influencer.Age,
                influencer.Gender,
                influencer.Country,
                influencer.InfluencerState,
                influencer.City,
                influencer.InfluencerLanguage,
                influencer.Biography,
                influencer.InstagramProfileUrl,
                influencer.InstagramFollowers,
                influencer.YouTubeProfileUrl,
                influencer.YouTubeFollowers,
                influencer.TikTokProfileUrl,
                influencer.TikTokFollower,
                influencer.SnapchatProfileUrl,
                influencer.SnapchatFollowers,
                influencer.XProfileUrl,
                influencer.XFollowers,
                influencer.ContactPhoneNumber,
                influencer.ContactEmailAddress
            }, transaction);


            // Inserts zero or more influencer domains in to the InfluencerDomain table 
            if (influencer.InfluencerDomains != null)
            {
                foreach (string domain in influencer.InfluencerDomains)
                {
                    connection.Execute(queryInsertInfluencerDomain, new { UserId = influencer.UserId, Domain = domain }, transaction);
                }
            }


            // Commits the transactions
            transaction.Commit();

            // Returns the newly generated primary key
            return newUserId;
        }

        catch (Exception exception)
        {
            // If something went wrong during the insertion then we roll back to ensure atomicity and a stable database
            transaction.Rollback();

            throw new TransactionAbortedException("Transaction failed: Something went wrong during the transaction, and a rollback to a stable version prior to the insertion has been performed. See inner exception for details.", exception);
        }
    }
    #endregion


    #region Get one Influencer
    /// <summary>
    /// Creates a new influencer by inserting data into the Users, Influencers, 
    /// and InfluencerDomains tables. The entire operation is executed inside
    /// a database transaction to ensure atomicity.
    /// </summary>
    /// 
    /// <param name="influencer">
    /// The influencer object containing all required data for creation.
    /// The object must contain valid LoginEmail and PasswordHash for the
    /// User table, and any influencer-specific fields for the Influencers
    /// and InfluencerDomains tables.
    /// </param>
    /// 
    /// <returns>
    /// The newly created user's UserId (primary key). This value is generated 
    /// by the database and assigned to both the Users and Influencers tables.
    /// </returns>
    /// 
    /// <exception cref="Exception">
    /// Thrown if any part of the creation process fails. 
    /// The database transaction will be rolled back before the exception is rethrown.
    /// </exception>
    public Influencer? GetOne(int userId)
    {
        
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
    #endregion

}


