using Dapper;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using InfiniteInfluence.DataAccessLibrary.Tools;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;

public class CompanyDao : BaseConnectionDao, ICompanyDao
{
    public CompanyDao(string connectionString) : base(connectionString)
    {
    }

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
    }

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
