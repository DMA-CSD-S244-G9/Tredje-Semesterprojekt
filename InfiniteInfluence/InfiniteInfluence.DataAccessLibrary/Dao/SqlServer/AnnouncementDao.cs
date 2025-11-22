using Dapper;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static System.Net.Mime.MediaTypeNames;


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
        // This SQL query inserts a new announcement into the Announcements table.
        // The VALUES section uses parameter placeholders like @userId which are filled by Dapper to prevent SQL injections.
        // 
        // Upon inserting the new row the SQL query returns the newly generated announcementId through using SCOPE_IDENTITY().
        // The Cast as INTT ensures that the returned value is treated like an integer, since it is our auto incremented value.
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


        // This SQL query inserts a single subject for a specific announcement, each of the subjects are stored as a seperate row in the AnnouncementSubjects table.
        // The query is executed once for every subject.
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
                    // This executes the SQL statement using Dapper's Execute method to insert a single subject into the AnnouncementSubjects table
                    // The transaction parameter ensrues taht hte insert is part of the same databaes transaction as the announcement insert,
                    // which means that if any subject insert fails teh entire operation of adding the announcement and each subject is rolled back
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
    /// 
    /// <returns>
    /// A collection of <see cref="Announcement"/> objects, each populated with
    /// its company name and a list of associated subjects.
    /// </returns>
    public IEnumerable<Announcement> GetAll()
    {
        // This SQL query retrieves all of the announcement records from the database's announcement table, the name of the company that created each announcement is also included.
        // The company name information is obtained by joining the Announcements table with the Companys table, based on the userId column that they share.
        //
        // The INNER JOIN ensures that the announcement is only included, if a matching company exists, but if no matching company is found the announcement is not returned, the
        // reason for this choice is that the company information is deemed a crucial part of an announcement, that the Influencer should not be without.
        //
        // Both "announcement" and "company" are table aliases used inside the query to make the SQL easier to read.
        const string SqlAnnouncements = @"
        SELECT
            announcement.announcementId,
            announcement.userId,
            announcement.title,
            announcement.creationDateTime,
            announcement.lastEditDateTime,
            announcement.startDisplayDateTime,
            announcement.endDisplayDateTime,
            announcement.currentApplicants,
            announcement.maximumApplicants,
            announcement.minimumFollowersRequired,
            announcement.communicationType,
            announcement.announcementLanguage,
            announcement.isKeepProducts,
            announcement.isPayoutNegotiable,
            announcement.totalPayoutAmount,
            announcement.shortDescriptionText,
            announcement.additionalInformationText,
            announcement.statusType,
            announcement.isVisible,

            -- Also select the companyName from the Companys table which we will map into the Announcement.CompanyName property in C#
            company.companyName AS CompanyName

        FROM Announcements announcement

        -- This announcement is only included, if a matching company exists if no company exist the announcement is not returned
        INNER JOIN Companys company ON announcement.userId = company.userId ";



        // This SQL query retrieves all of the subjects from the AnnouncementSubjects table, for a list of announcements,
        // the 'in @ids' in this context means that we return every subject row that belongs to any of those announcementIds from the list.
        const string SqlAnnouncementSubjects = @"
        SELECT
            announcementId AS AnnouncementId,
            announcementSubject AS AnnouncementSubject
        FROM AnnouncementSubjects

        -- Only return subjects where the announcementId is one of the IDs we requested
        WHERE announcementId IN @Ids";


        // Creates and opens the database connection
        using IDbConnection databaseConnection = CreateConnection();
        databaseConnection.Open();

        // Executes the SQL query that retrieves all of the announcements, including their company name, 
        // returning a list of Announcement objects, corrosponding to one object per row from the SQL result.
        List<Announcement> announcements = databaseConnection.Query<Announcement>(SqlAnnouncements).ToList();
        
        // If there are no announcements we will simply return the empty list
        if (announcements.Count == 0)
        {
            return announcements;
        }

        // Creates an array that holds integers that contains only announcementId values from all Announcement objects previously retrieved
        // This array of integers will later be used to fetch all subjects for all announcements in one batch
        int[] announcementIds = announcements.Select(announcement => announcement.AnnouncementId).ToArray();

        // Executes the SQL query that fetches all of the subjects for all of the announcement IDs retrieved above resulting in a list of AnnouncementSubjectRow objects,
        // where every object represents one subject belonging to only one announcement.
        List<AnnouncementSubjectRow> announcementSubjectRows = databaseConnection.Query<AnnouncementSubjectRow>(SqlAnnouncementSubjects, new { Ids = announcementIds }).ToList();

        // Takes the list of subject rows returned from the database which represents all subject rows for multiple announcements
        IEnumerable<AnnouncementSubjectRow> subjectRowsList = announcementSubjectRows;

        // Defines a function that extracts the announcementId from a subject row.
        // This function will be used to determine how items should be grouped, so that all subject rows gets grouped by their announcementId
        Func<AnnouncementSubjectRow, int> groupKeySelector = row => row.AnnouncementId;

        // Groups all subject rows based on their announcementId using the selector function
        IEnumerable<IGrouping<int, AnnouncementSubjectRow>> groupedByAnnouncementId = subjectRowsList.GroupBy(groupKeySelector);

        // Creates an empty dictionary where the key is an announcement's AnnouncementId and the value is a list of subjects that belong to the announcement
        Dictionary<int, List<string>> subjectsLookupByAnnouncementId = new Dictionary<int, List<string>>();

        // Iterates through each grouped collection of subject rows, each group represents all subject rows that belong to one announcement
        foreach (IGrouping<int, AnnouncementSubjectRow> subjectsForAnnouncement in groupedByAnnouncementId)
        {
            // Extracts the announcementId that this group of subjects belongs to and assigns it to the announcementId varaible
            int announcementId = subjectsForAnnouncement.Key;

            // Convertts all subject rows in this group in to a List<string> where each string is the subejct text
            List<string> subjects = subjectsForAnnouncement.Select(row => row.AnnouncementSubject).ToList();

            // Add an entry to the lookup dictionary where the unique announcementId is the key and subjects is the list of subject strings for that announcement
            subjectsLookupByAnnouncementId.Add(announcementId, subjects);
        }


        // Assigns the ListOfSubjects property to each Announcement
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



    /// <summary>
    /// Returns one single announcements from the database with the matching AnnouncementId that was supplied in the parameter,
    /// also retrurns the respective subjects associated with the announcement from the AnnouncementSubjects table.
    /// </summary>
    /// 
    /// 
    /// <param name="announcementId">The unique announcementID to look for and retrieve from the database's Announcement table</param>
    /// 
    /// 
    /// <returns>
    /// One single <see cref="Announcement"/> object or returns null, if no announcement with the specified id was found
    /// </returns>
    public Announcement? GetOne(int announcementId)
    {
        // This SQL query retrieves ONE specific announcement from the database, based on the supplied @AnnouncementId.
        // It also LEFT JOINs the "Companys" table so we can get the company name belonging to the user who created the announcement.
        //
        // The LEFT JOIN ensures that if a company record exists thenn we get a company.companyName. But if no company record exists then
        // the announcement is still returned, but with the companyName being null
        //
        // Both "announcement" and "company" are table aliases used inside the query to make the SQL easier to read.
        const string sqlAnnouncement = @"
        SELECT 
            announcement.announcementId,
            announcement.userId,
            announcement.title,
            announcement.creationDateTime,
            announcement.lastEditDateTime,
            announcement.startDisplayDateTime,
            announcement.endDisplayDateTime,
            announcement.currentApplicants,
            announcement.maximumApplicants,
            announcement.minimumFollowersRequired,
            announcement.communicationType,
            announcement.announcementLanguage,
            announcement.isKeepProducts,
            announcement.isPayoutNegotiable,
            announcement.totalPayoutAmount,
            announcement.shortDescriptionText,
            announcement.additionalInformationText,
            announcement.statusType,
            announcement.isVisible,

            company.companyName  

        FROM Announcements announcement
        LEFT JOIN Companys company ON announcement.userId = company.userId
        WHERE announcement.announcementId = @AnnouncementId; ";



        // This SQL query retrieves all of the subjects from the AnnouncementSubjects table, for an announcement
        // that has an id matching the specified @AnnouncementId. Each announcement can have 0 or up to 3 subjects.
        const string sqlSubjects = @"
        SELECT announcementSubject
        FROM AnnouncementSubjects
        WHERE announcementId = @AnnouncementId; ";


        // Creates and opens the database connection
        using IDbConnection connection = CreateConnection();
        connection.Open();

        // Executes the SQL query that retrieves the announcement with the specified ID, and returns the announcement object if a record matching the id exists, or returns null if none is found.
        Announcement? announcement = connection.QuerySingleOrDefault<Announcement>(sqlAnnouncement, new { AnnouncementId = announcementId } );

        // If no announcement was found then execute this section
        if (announcement == null)
        {
            return null;
        }

        // Executes the SQL query that retrieves all of the subjects belonging to the announcement, in the form of an Ienumerable<string> where each element one subject.
        IEnumerable<String> subjects = connection.Query<string>(sqlSubjects, new { AnnouncementId = announcementId } );

        // Converts the list of subjects into a List<string> and assign it to the Announcement's ListOfSubjects property
        announcement.ListOfSubjects = subjects.ToList();

        // ListOfAssociatedInfluencers bliver ikke loadet i denne version
        return announcement;
    }
}
