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
    public CompanyDao(string connectionString) : base(connectionString)
    {
    }

    public int CreateCompany(Company company)
    {
        
            // SQL statement for inserting a new user and retrieving the new UserId
            string? queryInsertUser = @"
            INSERT INTO [Users] (loginEmail, passwordHash)
            VALUES (@LoginEmail, @PasswordHash);

            SELECT CAST(SCOPE_IDENTITY() AS INT); ";

            // 2. Insert into Companys table
            string? queryInsertCompany = @"INSERT INTO Companys(
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
                        @State, 
                        @City,
                        @Address, 
                        @Languages, 
                        @Biography, 
                        @ContactPerson, 
                        @ContactEmailAddress,
                        @ContactPhoneNumber);";

            // SQL for inserting domains into the InfluencerDomains table
            string? queryInsertCompanyDomain = @"
            INSERT INTO CompanyDomains (userId, domain)
            VALUES (@UserId, @Domain); ";

        using var connection = CreateConnection();
        connection.Open();

        // Begins a transaction Since we have to make changes by performing multiple queries we have to
        // use a transaction to ensure that all inserts succeed together or fail together thereby enforcing atomicity
        using IDbTransaction transaction = connection.BeginTransaction();
        
        try
        {
            // Uses dapper to insert into the Users table and return the newest generated UserId using SCOPE_IDENTITY()
            int newUserId = connection.QuerySingle<int>(queryInsertUser, new { company.LoginEmail, company.PasswordHash }, transaction);

            // Updates the influencer's UserId to match the newly generated UserId
            company.UserId = newUserId;

            connection.Execute(queryInsertCompany,
                new
                {
                    UserId = newUserId,
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
                    company.State,
                    company.City,
                    company.Address,
                    company.Languages,
                    company.Biography,
                    company.ContactPerson,
                    company.ContactEmailAddress,
                    company.ContactPhoneNumber
                },
                transaction
            );

            // 3. Insert domains
            if (company.ListOfCompanyDomains != null)
            {
                foreach (var domain in company.ListOfCompanyDomains)
                {
                    connection.Execute(
                        queryInsertCompanyDomain,
                        new { UserId = newUserId, Domain = domain },
                        transaction
                    );
                }
            }

            transaction.Commit();

            return newUserId;
        }
        catch (Exception exception)
        {
            // If something went wrong during the insertion then we roll back to ensure atomicity and a stable database
            transaction.Rollback();

            throw new TransactionAbortedException("Transaction failed: Something went wrong during the transaction, and a rollback to a stable version prior to the insertion has been performed. See inner exception for details.", exception);
        }
    }

    /*
    public int CreateCompany(Company company)
    {
        using var connection = CreateConnection();
        connection.Open(); 
        using var transaction = connection.BeginTransaction();

        try
        {
            // 1. Insert into Users table
            string userSql = @"INSERT INTO Users (loginEmail, passwordHash)
                                    OUTPUT INSERTED.userId
                                    VALUES (@Email, @PasswordHash);";

            int newUserId = connection.QuerySingle<int>(userSql,
                new
                {
                    Email = company.LoginEmail,
                    PasswordHash = BCryptTool.HashPassword(company.PasswordHash)
                },
                transaction
            );

            // 2. Insert into Companys table
            string companySql = @"INSERT INTO Companys(
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
                        @State, 
                        @City,
                        @Address, 
                        @Languages, 
                        @Biography, 
                        @ContactPerson, 
                        @ContactEmailAddress,
                        @ContactPhoneNumber);";

            connection.Execute(companySql,
                new
                {
                    UserId = newUserId,
                    IsCompanyVerified = company.IsCompanyVerified,
                    VerificationDate = company.VerificationDate,
                    CompanyName = company.CompanyName,
                    CompanyLogoUrl = company.CompanyLogoUrl,
                    CeoName = company.CeoName,
                    DateOfEstablishment = company.DateOfEstablishment,
                    OrganisationNumber = company.OrganisationNumber,
                    StandardIndustryClassification = company.StandardIndustryClassification,
                    WebsiteUrl = company.WebsiteUrl,
                    CompanyEmail = company.CompanyEmail,
                    CompanyPhoneNumber = company.CompanyPhoneNumber,
                    Country = company.Country,
                    State = company.State,
                    City = company.City,
                    Address = company.Address,
                    Languages = company.Languages,
                    Biography = company.Biography,
                    ContactPerson = company.ContactPerson,
                    ContactEmailAddress = company.ContactEmailAddress,
                    ContactPhoneNumber = company.ContactPhoneNumber
                },
                transaction
            );

            // 3. Insert domains
            if (company.ListOfCompanyDomains != null)
            {
                foreach (var domain in company.ListOfCompanyDomains)
                {
                    connection.Execute(
                        "INSERT INTO CompanyDomains (userId, domain) VALUES (@UserId, @Domain)",
                        new { UserId = newUserId, Domain = domain },
                        transaction
                    );
                }
            }

            transaction.Commit();
            return newUserId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }*/

    public bool DeleteCompany(int userId)
    {
        var sqlQuery = "DELETE FROM Companys WHERE userId = @UserId";
        using var connection = CreateConnection();
        var rows = connection.Execute(sqlQuery, new { UserId = userId });
        return rows > 0;
    }

    public Company? GetOneCompany(int userId)
    {
        var query = "SELECT * FROM Companys WHERE userId = @UserId";
        using var connection = CreateConnection();
        return connection.QuerySingleOrDefault<Company>(query, new { UserId = userId });
    }
}
