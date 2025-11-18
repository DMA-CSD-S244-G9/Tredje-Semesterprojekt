using Dapper;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;


namespace InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;



public class AnnouncementDao : BaseConnectionDao, IAnnouncementDao
{
    #region Constructors
    public AnnouncementDao(string dataBaseConnectionString) : base(dataBaseConnectionString)
    {

    }
    #endregion



    // Please note that the list ListOfAssociatedInfluencers is not included in the
    // AnnouncementDao's Create method as the list is initially instantiated as an
    // empty list so that influencers can eventually apply to it.
    public int Create(Announcement announcement)
    {
        // SQL statement for inserting a new Announcement and retrieving the new AnnouncementId
        string queryInsertAnnouncement = @"
        INSERT INTO Announcements (
            userId, title, creationDateTime, lastEditDateTime,
            startDisplayDateTime, endDisplayDateTime, currentApplicants,
            maximumApplicants, minimumFollowersRequired, communicationType,
            announcementLanguage, isKeepProducts, isPayoutNegotiable,
            totalPayoutAmount, shortDescriptionText, additionalInformationText,
            statusType, isVisible )
        
        VALUES (
            @UserId, @Title, @CreationDateTime, @LastEditDateTime,
            @StartDisplayDateTime, @EndDisplayDateTime, @CurrentApplicants,
            @MaximumApplicants, @MinimumFollowersRequired, @CommunicationType,
            @AnnouncementLanguage, @IsKeepProducts, @IsPayoutNegotiable,
            @TotalPayoutAmount, @ShortDescriptionText, @AdditionalInformationText,
            @StatusType, @IsVisible );

        SELECT CAST(SCOPE_IDENTITY() AS INT); ";
        // ^-- We use Scope_Identity to retrieve the new ID that was created


        // SQL for inserting subjects into the AnnouncementSubjects table
        string queryInsertAnnouncementSubject = @"
        INSERT INTO AnnouncementSubjects (announcementId, announcementSubject)
        VALUES (@AnnouncementId, @AnnouncementSubject); ";


        // Creates and opens the database connection
        using IDbConnection connection = CreateConnection();
        connection.Open();


        // Begins a transaction Since we have to make changes by performing multiple queries we have to
        // use a transaction to ensure that all inserts succeed together or fail together thereby enforcing atomicity
        using IDbTransaction transaction = connection.BeginTransaction();

        try
        {
            // Uses dapper to insert into the Announcement table and return the newest generated AnnouncementId using SCOPE_IDENTITY()
            int newAnnouncementId = connection.QuerySingle<int>(queryInsertAnnouncement, new {
                UserId = announcement.UserId,
                announcement.Title,
                announcement.CreationDateTime,
                announcement.LastEditDateTime,
                announcement.StartDisplayDateTime,
                announcement.EndDisplayDateTime,
                announcement.CurrentApplicants,
                announcement.MaximumApplicants,
                announcement.MinimumFollowersRequired,
                announcement.CommunicationType,
                announcement.AnnouncementLanguage,
                announcement.IsKeepProducts,
                announcement.IsPayoutNegotiable,
                announcement.TotalPayoutAmount,
                announcement.ShortDescriptionText,
                announcement.AdditionalInformationText,
                announcement.StatusType,
                announcement.IsVisible
            }, transaction );


            // Assigns the announcement's AnnouncementId to match the newly generated AnnouncementId
            announcement.AnnouncementId = newAnnouncementId;

            
            // Inserts zero or more subject domains in to the AnnouncementSubjects table 
            if (announcement.ListOfSubjects != null)
            {
                foreach (string subject in announcement.ListOfSubjects)
                {
                    connection.Execute(queryInsertAnnouncementSubject, new { AnnouncementId = newAnnouncementId, AnnouncementSubject = subject }, transaction);
                }
            }


            // Commits the transactions
            transaction.Commit();

            // Returns the newly generated primary key
            return newAnnouncementId;
        }


        catch (Exception exception)
        {
            // If something went wrong during the insertion then we roll back to ensure atomicity and a stable database
            transaction.Rollback();

            throw new TransactionAbortedException("Transaction failed: Something went wrong during the transaction, and a rollback to a stable version prior to the insertion has been performed. See inner exception for details.", exception);
        }
    }





    /// <summary>
    /// Returns all of the announcements from the database with their respective subjects from the AnnouncementSubjects table,
    /// and includes the related company name from the Companys table.
    /// Retrieves all announcements from the database, including their related company name from the Companys table and their
    /// list of subjects from the AnnouncementSubjects table
    /// </summary>
    /// 
    /// <returns>
    /// A collection of <see cref="Announcement"/> objects, each populated with
    /// its company name and a list of associated subjects.
    /// </returns>
    public IEnumerable<Announcement> GetAll()
    {
        // SQL query that returns all announcements and their related company name
        // We join on userId to connect Announcements with Companys
        const string SqlAnnouncements = @"
        SELECT
            a.*,
            c.companyName AS CompanyName
        FROM Announcements a
        INNER JOIN Companys c ON a.userId = c.userId";

        // SQL query that returns all subjects for a given set of announcement IDs
        // Results will be grouped in memory by AnnouncementId
        const string SqlAnnouncementSubjects = @"
        SELECT
            announcementId AS AnnouncementId,
            announcementSubject AS AnnouncementSubject
        FROM AnnouncementSubjects
        WHERE announcementId IN @Ids";

        // Creates and opens the database connection
        using IDbConnection databaseConnection = CreateConnection();
        databaseConnection.Open();

        // 1) Get all announcements with their company name from the database
        var announcements = databaseConnection.Query<Announcement>(SqlAnnouncements).ToList();

        
        // If there are no announcements we will simply return the empty list
        if (announcements.Count == 0)
        {
            return announcements;
        }

        // Collect the IDs of all announcements we just fetched these are used to fetch all related subjects in a single query
        var announcementIds = announcements.Select(announcement => announcement.AnnouncementId).ToArray();

        // Retrieve all subject rows for the announcements in one batch
        var announcementSubjectRows = databaseConnection.Query<AnnouncementSubjectRow>(SqlAnnouncementSubjects, new { Ids = announcementIds }).ToList();


        // Group the subjects by AnnouncementId so we can easily look them up per announcement
        var subjectsLookupByAnnouncementId = announcementSubjectRows.GroupBy(subjectRow => subjectRow.AnnouncementId).ToDictionary(group => group.Key, group => group.Select(subjectRow => subjectRow.AnnouncementSubject).ToList());

        // Assigns the ListOfSubjects property on each Announcement
        foreach (var announcement in announcements)
        {
            // Tries to get the list of subjects for the current announcement
            if (subjectsLookupByAnnouncementId.TryGetValue(announcement.AnnouncementId, out var subjectsForAnnouncement))
            {
                // Subjects found are assigned to the list
                announcement.ListOfSubjects = subjectsForAnnouncement;
            }

            else
            {
                // If there are no subjects then an empty list is assigned to avoid null checks elsewhere
                announcement.ListOfSubjects = new List<string>();
            }
        }

        // Return all announcements, each enriched with its subjects and company name
        return announcements;
    }



    /// <summary>
    /// An helper class used to map rows from the AnnouncementSubjects query and represents a single subject entry for a specific announcement.
    /// </summary>
    private class AnnouncementSubjectRow
    {
        public int AnnouncementId { get; set; }
        public string AnnouncementSubject { get; set; } = string.Empty;
    }
}
