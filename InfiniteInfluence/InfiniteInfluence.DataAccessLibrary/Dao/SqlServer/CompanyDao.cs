using Dapper;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using InfiniteInfluence.DataAccessLibrary.Tools;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;


namespace InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;


public class CompanyDao : BaseConnectionDao, ICompanyDao
{
    #region Constructors
    public CompanyDao(string connectionString) : base(connectionString)
    {

    }
    #endregion


    public int Create(Company company)
    {
        // SQL statement for inserting a new user and retrieving the new UserId
        string? queryInsertUser = @"
        INSERT INTO [Users] (loginEmail, passwordHash)
        VALUES (@LoginEmail, @PasswordHash);

        SELECT CAST(SCOPE_IDENTITY() AS INT); ";
        // ^-- We use Scope_Identity to retrieve the new ID that was created


        // SQL statement for inserting the company specific details
        string? queryInsertCompany = @"
        INSERT INTO Companys(
            userId, 
            isCompanyVerified, 
            verificationDate, 
            companyName, 
            companyLogoUrl, 
            ceoName, 
            dateOfEstablishment, 
            organisationNumber, 
            standardIndustryClassification,
            websiteUrl, 
            companyEmail, 
            companyPhoneNumber, 
            country, 
            companyState, 
            city, 
            companyAddress, 
            companyLanguage, 
            biography, 
            contactPerson, 
            contactEmailAddress,
            contactPhoneNumber)

            VALUES
            (@UserId,
            @IsCompanyVerified, 
            @VerificationDate, 
            @CompanyName, 
            @CompanyLogoUrl,
            @CeoName, 
            @DateOfEstablishment, 
            @OrganisationNumber, 
            @StandardIndustryClassification,
            @WebsiteUrl, 
            @CompanyEmail, 
            @CompanyPhoneNumber, 
            @Country, 
            @CompanyState, 
            @City,
            @CompanyAddress, 
            @CompanyLanguage, 
            @Biography, 
            @ContactPerson, 
            @ContactEmailAddress,
            @ContactPhoneNumber);";


        // SQL for inserting domains into the companyDomains table
        string? queryInsertCompanyDomain = @"
        INSERT INTO CompanyDomains (userId, domain)
        VALUES (@UserId, @Domain); ";



        using IDbConnection connection = CreateConnection();
        connection.Open();


        // Begins a transaction Since we have to make changes by performing multiple queries we have to
        // use a transaction to ensure that all inserts succeed together or fail together thereby enforcing atomicity
        using IDbTransaction transaction = connection.BeginTransaction();
        
        try
        {
            // Uses dapper to insert into the Users table and return the newest generated UserId using SCOPE_IDENTITY()
            int newUserId = connection.QuerySingle<int>(queryInsertUser, new { company.LoginEmail, company.PasswordHash }, transaction);

            // Updates the company's UserId to match the newly generated UserId
            company.UserId = newUserId;

            connection.Execute(queryInsertCompany, new
                {
                    UserId = company.UserId,
                    company.IsCompanyVerified,
                    company.VerificationDate,
                    company.CompanyName,
                    company.CompanyLogoUrl,
                    company.CeoName,
                    company.DateOfEstablishment,
                    company.OrganisationNumber,
                    company.StandardIndustryClassification,
                    company.WebsiteUrl,
                    company.CompanyEmail,
                    company.CompanyPhoneNumber,
                    company.Country,
                    company.CompanyState,
                    company.City,
                    company.CompanyAddress,
                    company.CompanyLanguage,
                    company.Biography,
                    company.ContactPerson,
                    company.ContactEmailAddress,
                    company.ContactPhoneNumber
                }, transaction);


            // Inserts zero or more company domains in to the CompanyDomain table 
            if (company.CompanyDomains != null)
            {
                foreach (string domain in company.CompanyDomains)
                {
                    connection.Execute(queryInsertCompanyDomain, new { UserId = company.UserId, Domain = domain }, transaction);
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



    public bool Delete(int userId)
    {
        var sqlQuery = "DELETE FROM Companys WHERE userId = @UserId";
        using var connection = CreateConnection();
        var rows = connection.Execute(sqlQuery, new { UserId = userId });
        return rows > 0;
    }



    public Company? GetOne(int userId)
    {
        var query = "SELECT * FROM Companys WHERE userId = @UserId";
        using var connection = CreateConnection();
        return connection.QuerySingleOrDefault<Company>(query, new { UserId = userId });
    }
}
