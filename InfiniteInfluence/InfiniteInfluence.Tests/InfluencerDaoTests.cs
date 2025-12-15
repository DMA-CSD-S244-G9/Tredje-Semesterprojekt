using Dapper;
using InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;
using InfiniteInfluence.DataAccessLibrary.Model;
using Microsoft.Data.SqlClient;
using System.Data;


namespace InfiniteInfluence.Tests;


public class InfluencerDaoTests
{
    private const string _dataBaseConnectionString = "Data Source=localhost;Initial Catalog=InfiniteInfluence;Persist Security Info=True;User ID=sa;Password=@12tf56so;Encrypt=True;Trust Server Certificate=True";

    private InfluencerDao _influencerDao;


    // Arrange the same setup for all tests
    [SetUp]
    public void SetUp()
    {
        _influencerDao = new InfluencerDao(_dataBaseConnectionString);
    }

    #region Test for CreateConnection method

    [Test]
    public void CreateConnection_WithValidConnectionString_ShouldOpenConnection()
    {
        /////////////////
        // - Arrange - //
        /////////////////

        // Arrange - This has been done in [Setup] instantiating _influencerDao
        using IDbConnection connection = _influencerDao.CreateConnection();



        /////////////
        // - Act - //
        /////////////

        connection.Open();



        ////////////////
        // - Assert - //
        ////////////////

        Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));
    }

    #endregion

    #region Test for ID: 001 - Create Influencer

    /// <summary>
    /// Test for ID: 001 - Create Influencer
    /// Acceptance Criteria:
    /// - Jeg vil kunne oprette en profil med navn, og andre generelle oplysninger. 
    /// - Profilen skal også indeholde links til diverse sociale medier.  
    /// - Profilen skal også indeholde et biografi felt til yderligere oplysninger om mig. 
    /// - Mit følgerantal skal kunne angives for hver af de sociale medier.  
    /// - Min oprettede profil skal gemmes i databasen.
    /// </summary>
    /// 
    /// <remarks>
    /// This test creates a new Influencer with valid data and verifies that a new UserId is returned.
    /// </remarks>
    [Test]
    public void Create_WithValidInfluencer_ShouldReturnNewUserId()
    {
        /////////////////
        // - Arrange - //
        /////////////////

        Influencer influencer = new Influencer
        {
            // User properties
            LoginEmail = "NUnitTestCreateValid@example.com",
            PasswordHash = "PassedWord1234",


            // Influencer properties
            IsInfluencerVerified = false,
            VerificationDate = DateTime.Now,
            DisplayName = "Test Influencer",
            FirstName = "Test",
            LastName = "Person",
            ProfileImageUrl = "www.instagram.com/profile/NUnit3Test",
            Age = 25,
            Gender = "Male",
            Country = "Denmark",
            InfluencerState = "Nordjylland",
            City = "Aalborg",
            InfluencerLanguage = "Danish",
            Biography = "Lorem ipsum dolor",
            InstagramProfileUrl = "https://instagram.com/test",
            InstagramFollowers = 8000,
            YouTubeProfileUrl = "https://youtube.com/test",
            YouTubeFollowers = 5000,
            TikTokProfileUrl = "https://tiktok.com/@test",
            TikTokFollower = 12000,
            SnapchatProfileUrl = "https://snapchat.com/add/test",
            SnapchatFollowers = 3000,
            XProfileUrl = "https://x.com/test",
            XFollowers = 7000,
            ContactPhoneNumber = "+45 12 34 56 78",
            ContactEmailAddress = "contactNUnit3@example.com",
            InfluencerDomains = new System.Collections.Generic.List<string>
            {
                "#Fashion",
                "#Clothing",
                "#Style"
            }
        };



        /////////////
        // - Act - //
        /////////////

        int newUserId = _influencerDao.Create(influencer);



        ////////////////
        // - Assert - //
        ////////////////

        Assert.That(newUserId, Is.GreaterThan(0), "The Create method should return a UserId that is above 0");
        Assert.That(influencer.UserId, Is.EqualTo(newUserId), "The Influencer.UserId should be updated and be equals to that of the newly generated UserID.");


        //////////////////
        // - Clean up - //
        //////////////////

        // Perform clean up in the database by removing the inserted data
        Cleanup(newUserId);

    }

    #endregion

    #region Test for ID: 011  - GetOne Influencer

    /// <summary>
    /// Test for ID: 011 - GetOne Influencer
    /// Acceptance Criteria:
    /// - Den specifikke influencers profils data kan hentes fra databasen. 
    /// </summary>
    /// 
    /// <remarks>
    /// Verifies that GetOne returns the correct Influencer when provided with an existing UserId,
    /// checking that all relevant fields are populated correctly.
    /// </remarks>
    [Test]
    public void GetOne_WithExistingUserId_ShouldReturnMatchingInfluencer()
    {
        /////////////////
        // - Arrange - //
        /////////////////

        // We first create a new influencer using the Create method,
        // so the test does not depend on external seed data.

        //        string uniqueSuffix = Guid.NewGuid().ToString("N");
        string uniqueSuffix = Guid.NewGuid().ToString("N").Substring(0, 10);


        Influencer influencerToCreate = new Influencer
        {
            // User properties
            LoginEmail = $"g_{uniqueSuffix}@x.com",
            // LoginEmail = $"U_{uniqueSuffix}@example.com",
            PasswordHash = "SomeHashedPassword",


            // Influencer properties
            IsInfluencerVerified = true,
            VerificationDate = DateTime.UtcNow,
            DisplayName = "GetOne Test Influencer",
            FirstName = "Test",
            LastName = "User",
            ProfileImageUrl = "https://example.com/profile.jpg",
            Age = 30,
            Gender = "Female",
            Country = "Denmark",
            InfluencerState = "Hovedstaden",
            City = "Copenhagen",
            InfluencerLanguage = "Danish",
            Biography = "This is a test biography for GetOne.",
            InstagramProfileUrl = "https://instagram.com/getone_test",
            InstagramFollowers = 12345,
            YouTubeProfileUrl = "https://youtube.com/getone_test",
            YouTubeFollowers = 54321,
            TikTokProfileUrl = "https://tiktok.com/@getone_test",
            TikTokFollower = 9999,
            SnapchatProfileUrl = "https://snapchat.com/add/getone_test",
            SnapchatFollowers = 1111,
            XProfileUrl = "https://x.com/getone_test",
            XFollowers = 2222,
            ContactPhoneNumber = "+45 12 34 56 78",
            //            ContactEmailAddress = $"contact_getone_{uniqueSuffix}@example.com",
            ContactEmailAddress = $"c_{uniqueSuffix}@x.com",
            InfluencerDomains = new System.Collections.Generic.List<string>
                {
                    "#Fashion",
                    "#Beauty",
                    "#Lifestyle"
                }
        };

        int createdUserId = _influencerDao.Create(influencerToCreate);



        /////////////
        // - Act - //
        /////////////

        Influencer? foundInfluencer = _influencerDao.GetOne(createdUserId);



        ////////////////
        // - Assert - //
        ////////////////

        Assert.That(foundInfluencer, Is.Not.Null, "GetOne should return an influencer for an existing UserId.");
        Assert.That(foundInfluencer!.UserId, Is.EqualTo(createdUserId));

        // Checks if the base fields have the expected values
        Assert.That(foundInfluencer.LoginEmail, Is.EqualTo(influencerToCreate.LoginEmail));
        Assert.That(foundInfluencer.DisplayName, Is.EqualTo(influencerToCreate.DisplayName));
        Assert.That(foundInfluencer.FirstName, Is.EqualTo(influencerToCreate.FirstName));
        Assert.That(foundInfluencer.LastName, Is.EqualTo(influencerToCreate.LastName));

        // Check domains were loaded from InfluencerDomains
        Assert.That(foundInfluencer.InfluencerDomains, Is.Not.Null);
        Assert.That(foundInfluencer.InfluencerDomains.Count, Is.EqualTo(3));
        CollectionAssert.AreEquivalent(influencerToCreate.InfluencerDomains, foundInfluencer.InfluencerDomains, "InfluencerDomains should match the domains inserted during Create.");

    }


    /// <summary>
    /// Test for ID: 011 - GetOne Influencer
    /// </summary>
    /// 
    /// <remarks>
    /// Verifies that GetOne returns null when provided with a non-existing UserId.
    /// </remarks>
    [Test]
    public void GetOne_WithNonExistingUserId_ShouldReturnNull()
    {
        /////////////////
        // - Arrange - //
        /////////////////

        int nonExistingUserId = 500;



        /////////////
        // - Act - //
        /////////////

        Influencer? foundInfluencer = _influencerDao.GetOne(nonExistingUserId);



        ////////////////
        // - Assert - //
        ////////////////

        Assert.That(foundInfluencer, Is.Null, "GetOne should return null for a non-existing UserId.");
    }

    #endregion

    #region Helper methods

    /// <summary>
    /// Cleanup method to remove test data from the database after each test.
    /// </summary>
    public void Cleanup(int newInfluencerId)
    {

        // Prepares for connecting to the database
        using SqlConnection connection = new SqlConnection(_dataBaseConnectionString);

        // Establishes the connection to the database
        connection.Open();

        connection.Execute(@"DELETE FROM Influencers WHERE userId = @UserId;
          DELETE FROM Users WHERE userId = @UserId;", new { UserId = newInfluencerId });
    }
    #endregion
}