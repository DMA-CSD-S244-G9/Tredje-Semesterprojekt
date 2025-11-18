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
}
