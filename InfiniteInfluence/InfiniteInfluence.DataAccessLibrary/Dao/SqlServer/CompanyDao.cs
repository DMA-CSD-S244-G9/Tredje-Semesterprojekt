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

/// <summary>
/// Provides data access operations for managing company in the database.
/// </summary>
/// 
/// <remarks>This class is responsible for creating, retrieving, and deleting company records, 
/// It ensures atomicity of operations by utilizing transactions for multi-step database interactions.
/// </remarks>
public class CompanyDao : BaseConnectionDao, ICompanyDao
{
    #region Constructors
 
    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyDao"/> class 
    /// with the specified database connection string.
    /// 
    /// How it works:
    /// - In API's program, CompanyDao is instansiated with a connection string
    /// - It sends the string to BaseConnectionDao which stores it
    /// - Every time CompanyDAO needs to access the database, it calls CreateConnection()
    /// - CreateConnection() creates a SqlConnection using that stored connection string
    /// </summary>
    /// 
    /// <param name="connectionString">
    /// The connection string used to establish a connection to the database.
    /// </param>
    public CompanyDao(string connectionString) : base(connectionString)
    {

    }
    #endregion

    #region Prepared SQL Statements

    ////////////////////////////////////
    // - SQL statements for Company - //
    ////////////////////////////////////

    // SQL statement for inserting a new user and retrieving the new UserId
    private readonly string queryInsertUser = @"INSERT INTO [Users] (loginEmail, passwordHash) 
    VALUES (@LoginEmail, @PasswordHash);
    SELECT CAST(SCOPE_IDENTITY() AS INT); ";
    // ^-- We use Scope_Identity to retrieve the new ID that was created


    // SQL statement for inserting the company specific details
    private readonly string queryInsertCompany = @"
        INSERT INTO Companys(
            userId, isCompanyVerified, verificationDate, companyName, 
            companyLogoUrl, ceoName, dateOfEstablishment, organisationNumber, 
            standardIndustryClassification, websiteUrl, companyEmail, 
            companyPhoneNumber, country, companyState, city, 
            companyAddress, companyLanguage, biography, contactPerson, 
            contactEmailAddress, contactPhoneNumber)

            VALUES
            (@UserId, @IsCompanyVerified, @VerificationDate, @CompanyName, 
            @CompanyLogoUrl, @CeoName, @DateOfEstablishment, @OrganisationNumber, 
            @StandardIndustryClassification, @WebsiteUrl, @CompanyEmail, 
            @CompanyPhoneNumber, @Country, @CompanyState, @City, 
            @CompanyAddress, @CompanyLanguage, @Biography, @ContactPerson, 
            @ContactEmailAddress, @ContactPhoneNumber);";


    // SQL for inserting domains into the companyDomains table
    private readonly string queryInsertCompanyDomain = @" INSERT INTO CompanyDomains (userId, domain) VALUES (@UserId, @Domain);";

    #endregion

    #region DAO methods
    
    /// <summary>
    /// This method creates a company in the database.
    /// It performs multiple insert operations within a transaction to ensure atomicity.
    /// It uses Dapper for database interactions.
    /// </summary>
    /// <param name="company"></param>
    /// <returns></returns>
    /// <exception cref="TransactionAbortedException"></exception>
    public int Create(Company company)
    {
        // Creates and opens a new database connection using the baseConnetionDao method CreateConnection()
        // It closes the connection automatically at the end of the using block
        using IDbConnection connection = CreateConnection();
        connection.Open();

        // Begins a transaction Since we have to make changes by performing multiple queries we have to
        // use a transaction to ensure that all inserts succeed together or fail together thereby enforcing atomicity
        using IDbTransaction transaction = connection.BeginTransaction();
        
        try
        {
            // Uses dapper to insert into the Users table and return the newest generated UserId using SCOPE_IDENTITY()
            // Dapper parameterizes the query to prevent SQL injection attacks
            int newUserId = connection.QuerySingle<int>(queryInsertUser, new { company.LoginEmail, company.PasswordHash }, transaction);

            // Updates the company's UserId to match the newly generated UserId
            company.UserId = newUserId;

            // Inserts into the Companys table using the newly generated UserId
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


    //TODO: write comments
    public bool Delete(int userId)
    {
        var sqlQueryDelete = "DELETE FROM Companys WHERE userId = @UserId";
        using var connection = CreateConnection();
        var rows = connection.Execute(sqlQueryDelete, new { UserId = userId });
        return rows > 0;
    }


    //TODO: write comments
    public Company? GetOne(int userId)
    {
        var query = "SELECT * FROM Companys WHERE userId = @UserId";
        using var connection = CreateConnection();
        return connection.QuerySingleOrDefault<Company>(query, new { UserId = userId });
    }
    #endregion
}
