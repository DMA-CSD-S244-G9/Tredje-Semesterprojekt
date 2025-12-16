using Dapper;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using InfiniteInfluence.DataAccessLibrary.Tools;
using System.Data;
using System.Transactions;

namespace InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;


///<summary>
/// This class provides data access methods for managing Influencer entities in a SQL Server database.
/// It includes functionality to create new influencers and retrieve existing ones by their user ID.
/// 
/// It implements the ICompanyDao interface and extends the BaseConnectionDao for database connection management.
/// 
/// This class does not perform logging or presentation logic; instead, it throws exceptions that
/// are intended to be handled and logged by higher layers such as API or MVC controllers.
///</summary>

public class InfluencerDao : BaseConnectionDao, IInfluencerDao
{
    #region Constructors
    /// <summary>
    /// Empty constructor that calls the base class constructor with the provided connection string
    /// This allows the InfluencerDao to inherit the database connection functionality from BaseConnectionDao
    /// 
    /// How it works:
    /// - In API's program, InfluencerDao is instansiated with a connection string
    /// - It sends the string to BaseConnectionDao which stores it
    /// - Every time InfluencerDao needs to access the database, it calls CreateConnection()
    /// - CreateConnection() creates a SqlConnection using that stored connection string
    /// </summary>
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
    // SQL statement for retrieving a Influencer by userId
    // Uses an INNER JOIN to combine data from Users and Influencedr tables
    // To ensure that the userId entered is an influencer Id and also to get access to the loginEmail and passwordHash from Users table when login is implemented
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

    // SQL statement for retrieving influencer domains by userId
    private readonly string? queryFindInfluencerDomains = @"
                SELECT domain
                FROM InfluencerDomains
                WHERE userId = @UserId;";

    #endregion


    #region Create Influencer
    /// <summary>
    /// Creates a new Influencer in the database.
    /// </summary>
    /// 
    /// <remarks>
    /// Inserts data into both the Users and Influencers tables,
    /// storing the password securely using hashing.
    /// 
    /// Transaction management is employed to ensure atomicity of the operation.
    /// </remarks>
    /// 
    /// <returns>
    /// This method returns the newly created Influencer's UserId.
    /// </returns>
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
    /// Retrieves an Influencer associated with the specified user ID.
    /// </summary>
    /// 
    /// <remarks>
    /// This method queries the database to retrieve the Influencer details and its associated domains
    /// for the given user ID. If no company is found for the specified user ID, the method returns null.
    /// </remarks>
    /// 
    /// <returns>
    /// A Influencer object containing the company's details and associated domains if the user ID is found;
    /// otherwise,null.
    /// </returns>
    public Influencer? GetOne(int userId)
    {

        using IDbConnection connection = CreateConnection();

        try{
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

        // Catches any exceptions that occur during the database operations and wraps them in a DataException
        catch (Exception exception)
        {
            // Rethrows the exception with additional context about the failure 
            // Its caught higher up the call stack where it can be logged appropriately in the API controller
            throw new DataException("Failed to retrieve influencer from the database.", exception);
        }
    }
    #endregion
}


