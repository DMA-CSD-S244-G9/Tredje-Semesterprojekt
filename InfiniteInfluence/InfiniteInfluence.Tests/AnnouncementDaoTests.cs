using Dapper;
using InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;
using InfiniteInfluence.DataAccessLibrary.Model;
using Microsoft.Data.SqlClient;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Transactions;


namespace InfiniteInfluence.DataAccessLibrary.Tests;


[TestFixture]
public class AnnouncementDaoTests
{
    private const string _dataBaseConnectionString = "Data Source=localhost;Initial Catalog=InfiniteInfluence;Persist Security Info=True;User ID=sa;Password=@12tf56so;Encrypt=True;Trust Server Certificate=True";
    private AnnouncementDao _announcementDao;


    // Arrange the same setup for all tests
    [SetUp]
    public void SetUp()
    {
        _announcementDao = new AnnouncementDao(_dataBaseConnectionString);
    }



    // Creates a minimum required announcement object that uses the company 6 userid which represents NordicTech from the insert script.
    private Announcement CreateTestAnnouncement(int userId = 6)
    {
        Announcement announcement = new Announcement()
        {
            UserId = userId,
            Title = "Unit test announcement",
            CreationDateTime = DateTime.Now,
            LastEditDateTime = DateTime.Now,
            StartDisplayDateTime = DateTime.Now,
            EndDisplayDateTime = DateTime.Now.AddDays(14),
            CurrentApplicants = 0,
            MaximumApplicants = 5,
            MinimumFollowersRequired = 1000,
            CommunicationType = "Email",
            AnnouncementLanguage = "English",
            IsKeepProducts = false,
            IsPayoutNegotiable = true,
            TotalPayoutAmount = 123.45m,
            ShortDescriptionText = "Short description from test",
            AdditionalInformationText = "Additional info from test",
            StatusType = "Active",
            IsVisible = true,
            ListOfSubjects = new List<string>()
        };

        return announcement;
    }
    


    /// <summary>
    /// Creates an announcement and store the announcements property values correctly
    /// in the database, and then validates if the announcement is added by checking if
    /// the announcement object count has been increased by 1.
    /// </summary>
    [Test]
    public void Create_ValidAnnouncement_IncreasesCountAndPersistsData()
    {
        /////////////////
        // - Arrange - //
        /////////////////

        // Prepares for connecting to the database
        using var connection = new SqlConnection(_dataBaseConnectionString);
        
        // Creates an announcement object with the very basic requirements
        Announcement announcement = CreateTestAnnouncement();



        /////////////
        // - Act - //
        /////////////

        // Establishes the connection to the database
        connection.Open();

        // Gets the amount of registered announcements in the Announcement table before the insertion of a new announcement
        int countBefore = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Announcements");

        // This line calls the AnnouncementDao.Create(announcement) method and does the following:
        // Opens a database connection and starts a transaction
        // Inserts the Announcement into the database
        // Uses SCOPE_IDENTITY() to retrieve the newly generated announcementId.
        // Inserts any subjects that may be associated with the announcement
        // Commits the transaction if everything goes as planned
        // And lastly returns the new announcementId.
        // The returned ID is then stored in the variable newAnnouncementId
        int newAnnouncementId = _announcementDao.Create(announcement);

        // Gets the amount of registered announcements in the Announcement table after the creation of the announcement
        int countAfter = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Announcements");

        var announcementTableRow = connection.QuerySingle<dynamic>("SELECT * FROM Announcements WHERE announcementId = @Id", new { Id = newAnnouncementId });



        ////////////////
        // - Assert - //
        ////////////////
        
        Assert.That(newAnnouncementId, Is.GreaterThan(0));
        Assert.That(countAfter, Is.EqualTo(countBefore + 1));

        Assert.That((int)announcementTableRow.userId, Is.EqualTo(6));
        Assert.That((string)announcementTableRow.title, Is.EqualTo("Unit test announcement"));



        //////////////////
        // - Clean up - //
        //////////////////

        // Perform clean up in the database by removing the inserted data
        connection.Execute("DELETE FROM AnnouncementSubjects WHERE announcementId = @Id", new { Id = newAnnouncementId });
        connection.Execute("DELETE FROM Announcements WHERE announcementId = @Id", new { Id = newAnnouncementId });
    }



    /// <summary>
    /// Creates an announcemen and also inserts subjects in to the AnnouncementSubjects table.
    /// Validating whether the subjects are inserted correctly.
    /// </summary>
    [Test]
    public void Create_WithSubjects_InsertsIntoAnnouncementSubjects()
    {
        /////////////////
        // - Arrange - //
        /////////////////

        // Prepares for connecting to the database
        using var connection = new SqlConnection(_dataBaseConnectionString);

        // Creates an announcement object with the very basic requirements
        Announcement announcement = CreateTestAnnouncement();



        /////////////
        // - Act - //
        /////////////

        // Establishes the connection to the database
        connection.Open();

        // Changse the titel and adds three subjects to the announcement to also put information in to the AnnouncementSubjects table
        announcement.Title = "With subjects";
        announcement.ListOfSubjects = new List<string> { "Fashion", "Lifestyle", "Tech" };

        // This line calls the AnnouncementDao.Create(announcement) method and does the following:
        // Opens a database connection and starts a transaction
        // Inserts the Announcement into the database
        // Uses SCOPE_IDENTITY() to retrieve the newly generated announcementId.
        // Inserts any subjects that may be associated with the announcement
        // Commits the transaction if everything goes as planned
        // And lastly returns the new announcementId.
        // The returned ID is then stored in the variable newAnnouncementId
        int newAnnouncementId = _announcementDao.Create(announcement);



        ////////////////
        // - Assert - //
        ////////////////

        // We uses an IEnumerable to retrieve all the subjects found in the AnnouncementSubjects table that matches the announcementId
        IEnumerable<string> announcementSubjects = connection.Query<string>("SELECT announcementSubject FROM AnnouncementSubjects WHERE announcementId = @Id", new { Id = newAnnouncementId });

        // We assert that the inserted values match the ones within the announcementSubjects variable
        CollectionAssert.AreEquivalent(new[] { "Fashion", "Lifestyle", "Tech" }, announcementSubjects);



        //////////////////
        // - Clean up - //
        //////////////////

        // Perform clean up in the database by removing the inserted data
        connection.Execute("DELETE FROM AnnouncementSubjects WHERE announcementId = @Id", new { Id = newAnnouncementId });
        connection.Execute("DELETE FROM Announcements WHERE announcementId = @Id", new { Id = newAnnouncementId });
    }



    /// <summary>
    /// If an error occurs during insertion of subjects, then the whole transaction should eb 
    /// rolled back so there wont be created an announcement at all.
    /// </summary>
    [Test]
    public void Create_WhenSubjectInsertFails_RollsBackAnnouncement()
    {
        /////////////////
        // - Arrange - //
        /////////////////

        // Prepares for connecting to the database
        using var connection = new SqlConnection(_dataBaseConnectionString);

        // Creates an announcement object with the very basic requirements
        Announcement announcement = CreateTestAnnouncement();

        // Gets the amount of registered announcements in the Announcement table before the insertion of a new announcement
        int countBefore = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Announcements");



        /////////////
        // - Act - //
        /////////////

        // Establishes the connection to the database
        connection.Open();

        announcement.Title = "Testing error and rollback";

        // Each subject can only hold 20 characters, so we insert more than 20 to provoke an error
        announcement.ListOfSubjects = new List<string> { "This is a very long subject and should cause an error upon insertion leading to a rollback " };

        

        ////////////////
        // - Assert - //
        ////////////////

        // Assert.Throws expects a TestDelegate which is a method with no parameters that NUnit
        // can call internally to check whether it throws the expected exception.
        // 
        // The lamda's (_) creates an anonymous method that matches the delegates signature.
        // The lamda does not execute the _announcementDao.Create() immediately but instead it simply
        // provides the assert.Throws with a callable piece of code.
        // 
        // 
        // When assert.throws runs, it then invokes the delegate which then calls the _announcementDao.Create(announcement)
        // The NUnit 3 testing framework captures the exception and verifies that it is of the type
        // traansaction aborted exception and verifies TransactionAbortedException.
        // If it is any other type then it will cause the test to fail.
        Assert.Throws<TransactionAbortedException>(() => _announcementDao.Create(announcement));

        // Gets the amount of registered announcements in the Announcement table after the attempted creation of the announcement the before and after should be equal in this case since the insertion should have faiked
        int countAfter = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Announcements");
        
        // The counters from before and after are expected to be equals since the insertion of data should have failed due
        Assert.That(countAfter, Is.EqualTo(countBefore), "A new Announcement should not be added when the transaction has performed a rollback.");



        //////////////////
        // - Clean up - //
        //////////////////
        
        // Note: clean up shouldn't be needed here, because the transaction did this for us by rolling back so nothing was ever inserted
    }



    /// <summary>
    /// Test for ID: 007 - GetOne Announcement
    /// Acceptance Criteria:
    /// - Det enkelte samarbejdsopslags fulde indhold kan findes fra databasen.
    /// 
    /// 
    /// Retrieves the data of an Announcement with the specified announcementId.
    /// Validates if the retrieved announcement object's propertys match with what is expected of the one with the announcementId 1.
    /// 
    /// We use the announcement with the AnnouncementId 1 that is generated by our insert script, and expect to get the below back
    /// - announcementId: 1
    /// - companyId: 6
    /// - Title: Review Our New Smart Device
    /// - CompanyName: NordicTech
    /// - Subjects: Technology Electronics Gadgets
    /// 
    /// </summary>
    [Test]
    public void GetOne_ValidId_ReturnsCorrectAnnouncement()
    {
        /////////////////
        // - Arrange - //
        /////////////////
        
        int announcementId = 1;
        
        int expectedUserId = 6;
        string expectedTitle = "Review Our New Smart Device";
        string expectedCompanyName = "NordicTech";
        List<string> expectedSubjects = new List<string>
        {
            "Technology", "Electronics", "Gadgets"
        };

        // Prepares for connecting to the database
        using var connection = new SqlConnection(_dataBaseConnectionString);



        /////////////
        // - Act - //
        /////////////

        // Finds the specified announcement that matches the specified announcementId from the database and returns the row as an announcement object
        Announcement? announcement = _announcementDao.GetOne(announcementId);

        // Establishes the connection to the database
        connection.Open();



        ////////////////
        // - Assert - //
        ////////////////
        
        Assert.That(announcement, Is.Not.Null, "GetOne should return an announcement for an existing ID.");
        Assert.That(announcement.AnnouncementId, Is.EqualTo(announcementId), "AnnouncementId should match the requested ID.");
        Assert.That(announcement.UserId, Is.EqualTo(expectedUserId), "UserId should match the value stored in the database.");

        Assert.That(announcement.Title, Is.EqualTo(expectedTitle), "Title should match the insert.sql dataset.");
        Assert.That(announcement.CompanyName, Is.EqualTo(expectedCompanyName), "CompanyName must come from the joined Companys table.");

        CollectionAssert.AreEquivalent(expectedSubjects, announcement.ListOfSubjects, "Subjects should match those listed in AnnouncementSubjects for ID 1.");
    }



    /// <summary>
    /// Test for ID: 005 - AddInfluencerApplication Announcement
    /// Acceptance Criteria:
    /// - Når Submit Applikation klikkes på, tjekkes der om maksantallet af ansøgere allerede er nået, og hvis ikke så associeres influenceren med opslaget.
    /// 
    /// 
    /// Tests that an influencer can apply to an announcement and will be associated with the announcement
    /// 
    /// We use the announcement with the AnnouncementId 1 and InfluencerUserId 3 since the userid has not submitted an application yet
    /// - announcementId: 1
    /// - InfluencerUserId: 3
    /// 
    /// </summary>
    [Test]
    public void AddInfluencerApplication_Valid_AddRowAndIncrementCurrentApplicants()
    {
        /////////////////
        // - Arrange - //
        /////////////////

        const int announcementId = 1;
        const int influencerUserId = 3;

        // Prepares for connecting to the database
        using var connection = new SqlConnection(_dataBaseConnectionString);

        // Establishes the connection to the database
        connection.Open();

        // Retrieves the number of applicants of the announcement
        int currentApplicantsBefore = connection.ExecuteScalar<int>("SELECT currentApplicants FROM Announcements WHERE announcementId = @Id", new { Id = announcementId });

        int linkCountBefore = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM InfluencerAnnouncements WHERE announcementId = @AId AND userId = @UId", new { AId = announcementId, UId = influencerUserId });

        // Asset to confirm that an association does not already exist in the test data
        Assert.That(linkCountBefore, Is.EqualTo(0), "Seed data is expected not to contain an application for userId 3 on announcement 1.");

        bool result = false;



        /////////////
        // - Act - //
        /////////////

        try
        {
            result = _announcementDao.AddInfluencerApplication(announcementId, influencerUserId);

            int currentApplicantsAfter = connection.ExecuteScalar<int>("SELECT currentApplicants FROM Announcements WHERE announcementId = @Id", new { Id = announcementId });

            int linkCountAfter = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM InfluencerAnnouncements WHERE announcementId = @AId AND userId = @UId", new { AId = announcementId, UId = influencerUserId });



            ////////////////
            // - Assert - //
            ////////////////

            Assert.That(result, Is.True, "Should return true wwhen successful application.");
            Assert.That(linkCountAfter, Is.EqualTo(linkCountBefore + 1), "Only one association should be inserted into InfluencerAnnouncements.");
            Assert.That(currentApplicantsAfter, Is.EqualTo(currentApplicantsBefore + 1), "currentApplicants should increase by +1 for the announcement.");
        }



        //////////////////
        // - Clean up - //
        //////////////////

        finally
        {
            connection.Execute("DELETE FROM InfluencerAnnouncements WHERE announcementId = @AId AND userId = @UId", new { AId = announcementId, UId = influencerUserId });

            connection.Execute("UPDATE Announcements SET currentApplicants = @Count WHERE announcementId = @Id", new { Count = currentApplicantsBefore, Id = announcementId });
        }
    }



    /// <summary>
    /// Test for ID: 005 - AddInfluencerApplication Announcement
    /// Acceptance Criteria:
    /// - Når Submit Applikation klikkes på, tjekkes der om maksantallet af ansøgere allerede er nået, og hvis ikke så associeres influenceren med opslaget.
    /// 
    /// Tests that when an influencer applies to an application that already has reached a maximum of applicants
    /// then the influnecer wont be added as an applicant to the announcement.
    /// 
    /// 
    /// We use the announcement with the AnnouncementId 2 and InfluencerUserId 2 since the userid has not submitted an application yet
    /// - announcementId: 2
    /// - InfluencerUserId: 2
    /// - maximumApplicants = currentApplications (To simulate a full announcement)
    /// 
    /// </summary>
    [Test]
    public void AddInfluencerApplication_WhenMaxApplicantsReached_ThrowAndDoNotInsertData()
    {
        /////////////////
        // - Arrange - //
        /////////////////

        const int announcementId = 2;
        const int influencerUserId = 2;

        // Prepares for connecting to the database
        using var connection = new SqlConnection(_dataBaseConnectionString);

        // Establishes the connection to the database
        connection.Open();

        // Retrieves the number of applicants of the announcement
        int applicationsBefore = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM InfluencerAnnouncements WHERE announcementId = @Id", new { Id = announcementId });

        // Returns the currentApplicants and maximumApplicants for the announcement
        int currentApplicantsBefore = connection.ExecuteScalar<int>("SELECT currentApplicants FROM Announcements WHERE announcementId = @Id", new { Id = announcementId });
        int maxApplicantsOriginal = connection.ExecuteScalar<int>("SELECT maximumApplicants FROM Announcements WHERE announcementId = @Id", new { Id = announcementId });

        // Changes the value of maximumApplicants to be the current number of applications making the next application result in an exception
        connection.Execute("UPDATE Announcements SET maximumApplicants = @Max WHERE announcementId = @Id", new { Max = applicationsBefore, Id = announcementId });



        //////////////////////
        // - Act & Assert - //
        //////////////////////

        try
        {
            // TODO: This is bad practice, but unsure how to do split this up right now
            // Asserts that an exception is thrown due to exceeding the maximum number of applications
            Assert.Throws<InvalidOperationException>(() => _announcementDao.AddInfluencerApplication(announcementId, influencerUserId), "Should throw InvalidOperationException when current applicants exceed the maximum applicant for the announcement");

            // Retrieves the number of applications to ensure none were added
            int applicationsAfter = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM InfluencerAnnouncements WHERE announcementId = @Id", new { Id = announcementId });
            int currentApplicantsAfter = connection.ExecuteScalar<int>("SELECT currentApplicants FROM Announcements WHERE announcementId = @Id", new { Id = announcementId });

            Assert.That(applicationsAfter, Is.EqualTo(applicationsBefore), "No applications should have been inserted when maximumApplicants has been reached");
            Assert.That(currentApplicantsAfter, Is.EqualTo(currentApplicantsBefore), "currentApplicants should not change when the transaction is rolled back");
        }



        //////////////////
        // - Clean up - //
        //////////////////
        
        finally
        {
            connection.Execute("UPDATE Announcements SET maximumApplicants = @Max WHERE announcementId = @Id", new { Max = maxApplicantsOriginal, Id = announcementId });
        }
    }



    /// <summary>
    /// Test for ID: 005 - AddInfluencerApplication Announcement
    /// Acceptance Criteria:
    /// - Når Submit Applikation klikkes på, tjekkes der om maksantallet af ansøgere allerede er nået, og hvis ikke så associeres influenceren med opslaget.
    /// 
    /// Tests that when an influencer applies to an application that the influencer has not already applied to this announcement
    /// 
    /// 
    /// We use the announcement with the AnnouncementId 1 and InfluencerUserId 2 since the userid has already submitted an application to this announcement
    /// - announcementId: 1
    /// - InfluencerUserId: 2
    /// 
    /// </summary>
    [Test]
    public void AddInfluencerApplication_WhenInfluencerAlreadyApplied_ThrowAndDoNotInsertData()
    {
        /////////////////
        // - Arrange - //
        /////////////////

        const int announcementId = 1;
        const int influencerUserId = 2; // already applied to announcement 1 in seed data

        using var connection = new SqlConnection(_dataBaseConnectionString);
        connection.Open();

        // Verify that seed data actually contains an application for (userId=2, announcementId=1)
        int existingLinkCountBefore = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM InfluencerAnnouncements WHERE announcementId = @AId AND userId = @UId", new { AId = announcementId, UId = influencerUserId });

        Assert.That(existingLinkCountBefore, Is.GreaterThanOrEqualTo(1), "Seed data is expected to contain at least one application for userId 2 on announcement 1.");

        // Total number of applications for this announcement
        int totalApplicationsBefore = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM InfluencerAnnouncements WHERE announcementId = @Id", new { Id = announcementId });

        int currentApplicantsBefore = connection.ExecuteScalar<int>("SELECT currentApplicants FROM Announcements WHERE announcementId = @Id", new { Id = announcementId });



        //////////////////////
        // - Act & Assert - //
        //////////////////////

        // TODO: This is bad practice, but unsure how to do split this up right now
        // Asserts that an exception is thrown due to the influencer already having applied to the applications
        Assert.Throws<InvalidOperationException>(() => _announcementDao.AddInfluencerApplication(announcementId, influencerUserId), "If an influencer already applied a method should throw an exception");

        // Retrieves the data after the attempted insertion
        int existingLinkCountAfter = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM InfluencerAnnouncements WHERE announcementId = @AId AND userId = @UId", new { AId = announcementId, UId = influencerUserId });
        int totalApplicationsAfter = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM InfluencerAnnouncements WHERE announcementId = @Id", new { Id = announcementId });
        int currentApplicantsAfter = connection.ExecuteScalar<int>("SELECT currentApplicants FROM Announcements WHERE announcementId = @Id", new { Id = announcementId });

        // Assert that noething changed
        Assert.That(existingLinkCountAfter, Is.EqualTo(existingLinkCountBefore), "No application was associated with the announcement");
        Assert.That(totalApplicationsAfter, Is.EqualTo(totalApplicationsBefore), "The number of applications did no change");
        Assert.That(currentApplicantsAfter, Is.EqualTo(currentApplicantsBefore), "currentApplicants should not change when an application attempt fails");
    }
}
