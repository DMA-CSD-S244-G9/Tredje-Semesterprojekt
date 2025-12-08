using Dapper;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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

    
    #region SQL Queries - Create

    // This SQL query inserts a new announcement into the Announcements table.
    // The VALUES section uses parameter placeholders like @userId which are filled by Dapper to prevent SQL injections.
    // 
    // Upon inserting the new row the SQL query returns the newly generated announcementId through using SCOPE_IDENTITY().
    // The Cast as INTT ensures that the returned value is treated like an integer, since it is our auto incremented value.
    private string _sqlQueryInsertAnnouncementAndReturnId = @"
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
    private string _sqlQueryInsertAnnouncementSubject = @"
        INSERT INTO AnnouncementSubjects (announcementId, announcementSubject)
        VALUES (@AnnouncementId, @AnnouncementSubject); ";

    #endregion


    #region SQL Queries - GetAll
    
    // This SQL query retrieves all of the announcement records from the database's announcement table, the name of the company that created each announcement is also included.
    // The company name information is obtained by joining the Announcements table with the Companys table, based on the userId column that they share.
    //
    // The INNER JOIN ensures that the announcement is only included, if a matching company exists, but if no matching company is found the announcement is not returned, the
    // reason for this choice is that the company information is deemed a crucial part of an announcement, that the Influencer should not be without.
    //
    // Both "announcement" and "company" are table aliases used inside the query to make the SQL easier to read.
    private const string _sqlQueryGetAllAnnouncementsAndCompanyName = @"
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
            announcement.RowVersion,

            -- Also select the companyName from the Companys table which we will map into the Announcement.CompanyName property in C#
            company.companyName AS CompanyName

        FROM Announcements announcement

        -- This announcement is only included, if a matching company exists if no company exist the announcement is not returned
        INNER JOIN Companys company ON announcement.userId = company.userId ";



    // This SQL query retrieves all of the subjects from the AnnouncementSubjects table, for a list of announcements,
    // the 'in @ids' in this context means that we return every subject row that belongs to any of those announcementIds from the list.
    private const string _sqlQueryGetSubjectsForAnnouncementsByIds = @"
        SELECT
            announcementId AS AnnouncementId,
            announcementSubject AS AnnouncementSubject
        FROM AnnouncementSubjects

        -- Only return subjects where the announcementId is one of the IDs we requested
        WHERE announcementId IN @Ids";

    #endregion


    #region SQL Queries - GetOne
    
    // This SQL query retrieves ONE specific announcement from the database, based on the supplied @AnnouncementId and then
    // also LEFT JOINs the "Companys" table so we can get the company name belonging to the user who created the announcement.
    //
    // The LEFT JOIN ensures that if a company record exists thenn we get a company.companyName. But if no company record exists then
    // the announcement is still returned, but with the companyName being null
    //
    // Both "announcement" and "company" are table aliases used inside the query to make the SQL easier to read.
    private const string SqlQueryGetAnnouncementByIdAndCompanyName = @"
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
            announcement.RowVersion,

            company.companyName  

        FROM Announcements announcement
        LEFT JOIN Companys company ON announcement.userId = company.userId
        WHERE announcement.announcementId = @AnnouncementId; ";



    // This SQL query retrieves all of the subjects from the AnnouncementSubjects table, for an announcement
    // that has an id matching the specified @AnnouncementId. Each announcement can have 0 or up to 3 subjects.
    private const string _sqlQueryGetSubjectsByAnnouncementId = @"
        SELECT announcementSubject
        FROM AnnouncementSubjects
        WHERE announcementId = @AnnouncementId; ";



    // This SQL query retrieves all influencers who have applied to the specific announcement
    //
    // We use JOIN InfluencerAnnouncements with Influencers in order to get both the applicationState and the influencer info
    //
    // Returns one row per applicant containing influencer information and if no influencers have applied an empty list is returned instead
    private const string _sqlQueryGetInfluencerApplicantsFromAnnouncement = @"
    SELECT 
        influencerAnnouncement.userId           AS InfluencerUserId,
        influencer.displayName                  AS DisplayName,
        influencer.contactPhoneNumber           AS ContactPhoneNumber,
        influencer.contactEmailAddress          AS ContactEmailAddress,
        influencerAnnouncement.applicationState AS ApplicationState
        
    FROM InfluencerAnnouncements influencerAnnouncement
        
    JOIN Influencers influencer 
        ON influencerAnnouncement.userId = influencer.userId

    WHERE influencerAnnouncement.announcementId = @AnnouncementId;";
    #endregion


    #region SQL Queries - AddInfluencerApplication
    /////////////////////////////////////////////////
    // - AddInfluencerApplication Method Queries - //
    /////////////////////////////////////////////////


    // SQL query that retrieves the currentApplicants, maximumApplicants and the
    // RowVersion for the announcement with a matching announcementId.
    private const string _sqlQueryGetAnnouncementWithConcurrency = @"
    SELECT 
        currentApplicants      AS CurrentApplicants,
        maximumApplicants      AS MaximumApplicants,
        RowVersion             AS RowVersion
    FROM Announcements
    WHERE announcementId = @AnnouncementId;";


    // SQL query that checks whether a specific influencer (userId)
    // has already applied to the same announcement.
    //
    // The purpose is to prevent duplicate applications.
    // If the COUNT(*) result is > 0, the influencer has already applied,
    // so the system should block the new application attempt.
    private const string _sqlQueryCheckIfInfluencerAlreadyApplied = @"
        SELECT COUNT(*)
        FROM InfluencerAnnouncements
        WHERE announcementId = @AnnouncementId
            AND userId = @UserId;";


    // SQL query that inserts a new application from an influencer
    // into the InfluencerAnnouncements table.
    //
    // This creates a link between:
    //   • the influencer (userId)
    //   • the announcement (announcementId)
    //
    // applicationState is set to 'Pending' to indicate that the influencer
    // has applied but has not yet been accepted or declined.
    private const string _sqlQueryInsertInfluencerApplication = @"
        INSERT INTO InfluencerAnnouncements (userId, announcementId, applicationState)
        VALUES (@UserId, @AnnouncementId, 'Pending');";


    // SQL query that increments (adds +1 to) the currentApplicants field
    // on an announcement after a new influencer applies.
    //
    // This ensures that the UI and system always show the correct number
    // of total applicants.
    //
    // Example:
    //   Before insert: currentApplicants = 4
    //   After insert:  currentApplicants = 5
    private const string _sqlQueryUpdateCurrentNumberOfApplicants = @"
    UPDATE Announcements
    SET currentApplicants = currentApplicants + 1
    WHERE announcementId = @AnnouncementId
      AND RowVersion = @RowVersion;";



    #endregion


    #region SQL Queries - DeleteAnnouncement
    // SQL Query that deletes the announcement with the specified announcementId
    // from the Announcements table.
    // 
    // Because of the database structure using 'on delte cascade' the announements
    // associated AnnouncementSubjects and InfluencerAnnouncements tables will also
    // be deleted when the announcement is deleted, making this query very short and
    // yet very functional.
    const string _sqlQueryDeleteAnnouncementById = @"
        DELETE FROM Announcements
        WHERE announcementId = @AnnouncementId;";

    #endregion


    #region SQL Queries - Delete

    // This SQL query delete all of the subjects from the AnnouncementSubjects table, with the matching announcementId
    private const string _sqlQueryDelteSubjectsByAnnouncementId = @"
        DELETE FROM AnnouncementSubjects
        WHERE announcementId = @AnnouncementId;";

    #endregion


    #region SQL Queries - Update
    // This SQL query updates an existing announcement in the Announcements table.
    private const string SqlQueryUpdateAnnouncementByAnnouncementId = @"
        UPDATE Announcements
        SET 
            title = @Title,
            lastEditDateTime = @LastEditDateTime,
            startDisplayDateTime = @StartDisplayDateTime,
            endDisplayDateTime = @EndDisplayDateTime,
            maximumApplicants = @MaximumApplicants,
            minimumFollowersRequired = @MinimumFollowersRequired,
            communicationType = @CommunicationType,
            announcementLanguage = @AnnouncementLanguage,
            isKeepProducts = @IsKeepProducts,
            isPayoutNegotiable = @IsPayoutNegotiable,
            totalPayoutAmount = @TotalPayoutAmount,
            shortDescriptionText = @ShortDescriptionText,
            additionalInformationText = @AdditionalInformationText,
            statusType = @StatusType,
            isVisible = @IsVisible
        WHERE AnnouncementId = @AnnouncementId AND RowVersion = @RowVersion;";
    #endregion


    #region Create Method
    // Please note that the list ListOfAssociatedInfluencers is not included in the
    // AnnouncementDao's Create method as the list is initially instantiated as an
    // empty list so that influencers can eventually apply to it.
    public int Create(Announcement announcement)
    {
        // Creates and opens the database connection
        using IDbConnection connection = CreateConnection();
        connection.Open();


        // Begins a transaction Since we have to make changes by performing multiple queries we have to
        // use a transaction to ensure that all inserts succeed together or fail together thereby enforcing atomicity
        using IDbTransaction transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

        try
        {
            // Uses dapper to insert into the Announcement table and return the newest generated AnnouncementId using SCOPE_IDENTITY()
            int newAnnouncementId = connection.QuerySingle<int>(_sqlQueryInsertAnnouncementAndReturnId, new
            {
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
            }, transaction);


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
                    connection.Execute(_sqlQueryInsertAnnouncementSubject, new { AnnouncementId = newAnnouncementId, AnnouncementSubject = subject }, transaction);
                }
            }


            // Commits the transactions if everything went as expected
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
    #endregion


    #region GetAll Method
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
        // Creates and opens the database connection
        using IDbConnection databaseConnection = CreateConnection();
        databaseConnection.Open();

        // Executes the SQL query that retrieves all of the announcements, including their company name, 
        // returning a list of Announcement objects, corrosponding to one object per row from the SQL result.
        List<Announcement> announcements = databaseConnection.Query<Announcement>(_sqlQueryGetAllAnnouncementsAndCompanyName).ToList();

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
        List<AnnouncementSubjectRow> announcementSubjectRows = databaseConnection.Query<AnnouncementSubjectRow>(_sqlQueryGetSubjectsForAnnouncementsByIds, new { Ids = announcementIds }).ToList();

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
        foreach (Announcement announcement in announcements)
        {
            // Tries to get the list of subjects for the current announcement
            if (subjectsLookupByAnnouncementId.TryGetValue(announcement.AnnouncementId, out List<String>? subjectsForAnnouncement))
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
    #endregion


    #region helper classes
    /// <summary>
    /// An helper class used to map rows from the AnnouncementSubjects query and represents a single subject entry for a specific announcement.
    /// </summary>
    private class AnnouncementSubjectRow
    {
        public int AnnouncementId { get; set; }
        public string AnnouncementSubject { get; set; } = string.Empty;
    }
    #endregion


    #region GetOne Method
    /// <summary>
    /// Returns one single announcements from the database with the matching AnnouncementId that was supplied in the parameter,
    /// also retrurns the respective subjects associated with the announcement from the AnnouncementSubjects table.
    /// </summary>
    /// 
    /// <param name="announcementId">The unique announcementID to look for and retrieve from the database's Announcement table</param>
    /// 
    /// <returns>
    /// One single <see cref="Announcement"/> object or returns null, if no announcement with the specified id was found
    /// </returns>
    public Announcement? GetOne(int announcementId)
    {
        // Creates and opens the database connection
        using IDbConnection connection = CreateConnection();
        connection.Open();

        // Executes the SQL query that retrieves the announcement with the specified ID, and returns the announcement object if a record matching the id exists, or returns null if none is found.
        Announcement? announcement = connection.QuerySingleOrDefault<Announcement>(SqlQueryGetAnnouncementByIdAndCompanyName, new { AnnouncementId = announcementId });

        // If no announcement was found then execute this section
        if (announcement == null)
        {
            return null;
        }

        // Executes the SQL query that retrieves all of the subjects belonging to the announcement, in the form of an Ienumerable<string> where each element one subject.
        IEnumerable<String> subjects = connection.Query<string>(_sqlQueryGetSubjectsByAnnouncementId, new { AnnouncementId = announcementId });

        // Converts the list of subjects into a List<string> and assign it to the Announcement's ListOfSubjects property
        announcement.ListOfSubjects = subjects.ToList();

        // Retrieves the applicants of the announcement with the specified ID and stores it as an IEnumerable        
        IEnumerable<AnnouncementApplicant> applicants = connection.Query<AnnouncementApplicant>(_sqlQueryGetInfluencerApplicantsFromAnnouncement, new { AnnouncementId = announcementId });

        // Converts the IEnumerable of applicants to a list and assigns it to teh announcements applicants property
        announcement.Applicants = applicants.ToList();

        // ListOfAssociatedInfluencers bliver ikke loadet i denne version
        return announcement;
    }
    #endregion


    #region AddInfluencerApplication Method
    /// <summary>
    /// This method will attempt to add an influencers application to hte announcement with the specificed id.
    ///
    /// 
    /// The method also incorporates the following business rules:
    ///     The announcement must not already have a maximum number of applicants
    ///     The influencer must not already have applied to the announcement
    ///     The application is inserted with a default state of pending and +1 is added to the current application counter 
    ///</summary>
    /// 
    /// <returns>
    /// True if the influencer's application was successfully submitted and inserted in to the database else false
    /// </returns>
    ///
    /// <remarks>
    /// Exceptions:
    ///   InvalidOperationException is thrown if the maximum number of applicants is reached or if the influencer has already applied to this announcement
    ///   TransactionAbortedException is thrown if any of the steps fail and the transaction is rolled back as a result thereof
    /// </remarks>
    public bool AddInfluencerApplication(int announcementId, int influencerUserId)
    {
        // Creates and opens the database connection
        using IDbConnection connection = CreateConnection();
        connection.Open();

        // Begins a transaction since we have to make changes by performing multiple queries we have to
        // use a transaction to ensure that all inserts succeed together or fail together thereby enforcing atomicity
        using IDbTransaction transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

        try
        {
            // Retrieves the current number of applicants, the maximum number of applicants, and current row version for the announcement with the specified announcementid
            AnnouncementConcurrencyInfo? announcementWithConcurrencyInfo = connection.QuerySingleOrDefault<AnnouncementConcurrencyInfo>(_sqlQueryGetAnnouncementWithConcurrency, new { AnnouncementId = announcementId }, transaction);

            // If the announcement with the specified id was not retrieved from the database then execute this section
            if (announcementWithConcurrencyInfo == null)
            {
                throw new InvalidOperationException("The announcement was unable to be found");
            }

            // Assigns the retrieved values to the variables
            int currentNumberOfApplicants = announcementWithConcurrencyInfo.CurrentApplicants;
            int maximumNumberOfApplicants = announcementWithConcurrencyInfo.MaximumApplicants;
            byte[] rowVersion = announcementWithConcurrencyInfo.RowVersion;

            // If there already are the same or more appicants than the specified maximum of applicants then execute this section
            // Note that it would be unlikely there are more unless we make a mistake with our test data in which case this can be the case, else it is unlikely if we can get our concurrency control to work later on
            if (currentNumberOfApplicants >= maximumNumberOfApplicants)
            {
                throw new InvalidOperationException("The maximum number of applicants has been reached for this announcement.");
            }

            // Retrieves the number of application entries for the given influencer userid on the specific application
            int alreadyApplied = connection.ExecuteScalar<int>(_sqlQueryCheckIfInfluencerAlreadyApplied, new { AnnouncementId = announcementId, UserId = influencerUserId }, transaction);

            // If the influencer has applied to this announcement once or more already then execute this esction
            if (alreadyApplied > 0)
            {
                throw new InvalidOperationException("You have already applied to this announcement.");
            }

            // Inserts the influencer's application for a collaboration with a status of Pending into the database's InfluencerAnnouncements table
            int rowsInserted = connection.Execute(_sqlQueryInsertInfluencerApplication, new { UserId = influencerUserId, AnnouncementId = announcementId }, transaction);

            // If something went wrong during the insertion then execute this esction
            if (rowsInserted == 0)
            {
                throw new InvalidOperationException("Your submitted application could not be saved");
            }

            // Only if the rowVersion is the correct value then adds +1 to the current number of applicants on the announcement so that it is displayed correctly in the announcement
            int rowsUpdated = connection.Execute(_sqlQueryUpdateCurrentNumberOfApplicants, new { AnnouncementId = announcementId, RowVersion = rowVersion }, transaction);

            // If 0 rows were updated it means the rowVersion no longer was a match, meaning some other user updated it first already
            if (rowsUpdated == 0)
            {
                // NOTE: We probably could throw another exception informing the user that somebody else took the last spot, but this exception should suffice I believe?
                throw new InvalidOperationException("The maximum number of applicants has been reached for this announcement.");
            }

            // Commits the transactions if everything went as expected
            transaction.Commit();

            return true;
        }

        catch (InvalidOperationException)
        {
            // If something went wrong during the insertion then we roll back to ensure atomicity and a stable database
            transaction.Rollback();

            throw;
        }

        catch (Exception exception)
        {
            // If something went wrong during the insertion then we roll back to ensure atomicity and a stable database
            transaction.Rollback();

            throw new TransactionAbortedException("Transaction failed: Something went wrong during the transaction, and a rollback to a stable version prior to the insertion has been performed. See inner exception for details.", exception);
        }
    }
    #endregion


    #region Update method
    public bool Update(Announcement announcement)
    {
        // Creates and opens the database connection
        using IDbConnection connection = CreateConnection();
        connection.Open();

        // Begins a transaction since we have to make changes by performing multiple queries we have to
        // use a transaction to ensure that all inserts succeed together or fail together thereby enforcing atomicity
        using IDbTransaction transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

        try
        {
            // Updates the information of the announcement with the matching announcementId and returns the amount of rows that was affected ideally this is 1
            int rowsAffected = connection.Execute(SqlQueryUpdateAnnouncementByAnnouncementId, new
            {
                AnnouncementId = announcement.AnnouncementId,
                Title = announcement.Title,
                LastEditDateTime = announcement.LastEditDateTime,
                StartDisplayDateTime = announcement.StartDisplayDateTime,
                EndDisplayDateTime = announcement.EndDisplayDateTime,
                MaximumApplicants = announcement.MaximumApplicants,
                MinimumFollowersRequired = announcement.MinimumFollowersRequired,
                CommunicationType = announcement.CommunicationType,
                AnnouncementLanguage = announcement.AnnouncementLanguage,
                IsKeepProducts = announcement.IsKeepProducts,
                IsPayoutNegotiable = announcement.IsPayoutNegotiable,
                TotalPayoutAmount = announcement.TotalPayoutAmount,
                ShortDescriptionText = announcement.ShortDescriptionText,
                AdditionalInformationText = announcement.AdditionalInformationText,
                StatusType = announcement.StatusType,
                IsVisible = announcement.IsVisible,
                RowVersion = announcement.RowVersion,
            }, transaction);

            // If rowsAffected is 0 then the update failed most likely due to concurrency issues and an umatching RowVersion and we throw an exception to indicate another transaction happened first
            if (rowsAffected == 0)
            {
                throw new InvalidOperationException("The announcement was edited by another user. Please reload and try again.");
            }

            // Deletes all existing subjects for the announcement
            // This is done to simplify the update process by removing all old subjects and re-inserting the current list
            connection.Execute(_sqlQueryDelteSubjectsByAnnouncementId, new { AnnouncementId = announcement.AnnouncementId }, transaction);


            // Inserts zero or more subject domains in to the AnnouncementSubjects table
            if (announcement.ListOfSubjects != null)
            {
                foreach (string subject in announcement.ListOfSubjects)
                {
                    connection.Execute(_sqlQueryInsertAnnouncementSubject, new { AnnouncementId = announcement.AnnouncementId, AnnouncementSubject = subject }, transaction);
                }
            }

            // Commits the transactions if everything went as expected
            transaction.Commit();

            return true;
        }

        catch (InvalidOperationException)
        {
            // If something went wrong during the insertion then we roll back to ensure atomicity and a stable database
            transaction.Rollback();

            throw;
        }

        catch (Exception exception)
        {
            // If something went wrong during the insertion then we roll back to ensure atomicity and a stable database
            transaction.Rollback();

            throw new TransactionAbortedException("Transaction failed: Something went wrong during the transaction, and a rollback to a stable version prior to the insertion has been performed. See inner exception for details.", exception);
        }
    }
    #endregion


    #region Delete method
    /// <summary>
    /// A Helper class used to map rows from the query for retrieving Announcement
    /// </summary>
    private class AnnouncementConcurrencyInfo
    {
        public int CurrentApplicants { get; set; }
        public int MaximumApplicants { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }



    /// <summary>
    /// This method will attempt to remove an announcement that matches the specified id.
    /// 
    /// Upon the announcement being deleted within the Announcement table, a cascade deletion will ensure
    /// causing the announcement's associated rows within the AnnouncementSubjects and InfluencerAnnouncements tables
    /// to also be removed.
    /// 
    /// 
    /// Returns:
    ///   true if one or multiple rows were affected by the deletion, else returns false
    ///
    /// </summary>
    public bool Delete(int announcementId)
    {
        // Creates and opens the database connection
        using IDbConnection connection = CreateConnection();
        connection.Open();

        // Returns the number of rows that had the matching AnnouncementId within the Announcement table and that was deleted
        int rowsAffected = connection.Execute(_sqlQueryDeleteAnnouncementById, new { AnnouncementId = announcementId });

        // If the number of affected rows are more than 0 then returns true else false
        return rowsAffected > 0;
    }
     #endregion
}
