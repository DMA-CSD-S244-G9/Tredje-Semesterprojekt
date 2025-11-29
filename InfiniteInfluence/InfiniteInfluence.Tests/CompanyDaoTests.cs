using Dapper;
using InfiniteInfluence.API.Controllers;
using InfiniteInfluence.ApiClient;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;
using InfiniteInfluence.DataAccessLibrary.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging.Abstractions;
using RestSharp; //NullLogger for testing

namespace InfiniteInfluence.Tests;

public class CompanyDaoTests
{
    private const string _dataBaseConnectionString = "Data Source=localhost;Initial Catalog=InfiniteInfluence;User ID=sa;Password=@12tf56so;Trust Server Certificate=True";

    private ICompanyDao _companyDao;

    [SetUp]
    public void Setup()
    {
        _companyDao = new CompanyDao(_dataBaseConnectionString);
    }

    #region Test for ID: 002 - Create Company
    ///<summary> 
    /// Test for ID: 002 - Create Company 
    /// Acceptance Criteria: 
    /// - Jeg vil kunne oprette en profil med navn, og andre generelle oplysninger. 
    /// 
    /// Testing:
    /// Should verify that the profile is created and saved in the database, we will check if the UserId is set after creation.
    /// </summary>
    [Test]
    public void CreateCompanyProfile_WithNameAndGeneralInfo_ShouldSucceed()
    {
        // Arrange
        Company testCompany = new Company(
            loginEmail: "catchSushi@Sushigmail.com",
            passwordHash: "hashpassword",
            isCompanyVerified: false,
            verificationDate: DateTime.UtcNow,
            companyName: "Catch Sushi - CompanyName",
            companyLogoUrl: "companyLogoUrl",
            companyDomains: new List<string> { "Food", "Vlog", "Starters"},
            ceoName: "Catch Ceo Name",
            dateOfEstablishment: new DateTime(1995, 3, 1),
            organisationNumber: "12345OrgNumber",
            standardIndustryClassification: 1234,
            websiteUrl: "websiteUrl",
            companyEmail: "CatchSushi@gmail.com",
            companyPhoneNumber: "+4512341234",
            country: "Denmark",
            companyState: "Sjælland",
            city: "KBH",
            companyAddress: "Catch Sushi address",
            companyLanguage: "Danish",
            biography: "Biography om catch sushi",
            contactPerson: "Catch sushi contact person",
            contactEmailAddress: "SushiCatchContractPerson@gmail.com",
            contactPhoneNumber: "+4512341234"
        );

        // Act
        // Using the DAO to create the Company
        int newCompanyId = _companyDao.Create(testCompany);

        // Assert
        // If the insert is successful, the userId should be set 
        Assert.That(newCompanyId, Is.GreaterThan(0));

        //////////////////
        // - Clean up - //
        //////////////////

        // Perform clean up in the database by removing the inserted data
        Cleanup(newCompanyId);
    }

    #endregion

    #region Test for ID: 012 - GetOne Company
    ///<summary> 
    /// Test for ID: 012  - GetOne Company 
    /// Acceptance Criteria: 
    /// - Den specifikke virksomheds profils data kan hentes fra databasen.
    /// </summary>
    /// 
    /// <remarks>
    /// Testing:
    /// Should verify that the profile are retrieved from the database with an UserId.
    /// </remarks>
    [Test]
    public void GetOneCompanyProfile_ByUserId_ShouldReturnCompanyProfile()
    {
        /////////////////
        // - Arrange - //
        /////////////////


        /////////////
        // - Act - //
        /////////////
        Company company = _companyDao.GetOne(6);


        ////////////////
        // - Assert - //
        ////////////////
        Assert.IsNotNull(company);
    }


    /// <summary>
    /// Intergration test to verify that the API endpoint for getting a company profile works as expected.
    /// </summary>
    [Test]
    public void GetOneCompanyProfile_FromApi()
    {
        /////////////////
        // - Arrange - //
        /////////////////

        // A mock logger, that doesnt log anything, just for testing purposes
        var nullLogger = NullLogger<CompanysController>.Instance;

        CompanysController companyApi = new CompanysController(nullLogger, _companyDao);


        /////////////
        // - Act - //
        /////////////
        // Using ActionResult to simulate API response
        ActionResult<Company> companyResult = companyApi.GetOne(6);


        ////////////////
        // - Assert - //
        ////////////////
        Assert.IsNotNull(companyResult);
    }

    /// <summary> 
    /// TODO: Didnt work
    /// Intergration test to verify that the ApiClient for getting a company profile works as expected.
    /// </summary>>
    [Test]
    public void GetOneCompanyProfile_FromApiClient()
    {
        /////////////////
        // - Arrange - //
        /////////////////
        ICompanyDao companyApiClient = new CompanyApiClient("https://localhost:7777");


        /////////////
        // - Act - //
        /////////////
        Company? company = companyApiClient.GetOne(6);


        ////////////////
        // - Assert - //
        ////////////////
        Assert.IsNotNull(company);
    }

    #endregion

    #region Helper methods

    /// <summary>
    /// Cleanup method to remove test data from the database after each test.
    /// </summary>
    public void Cleanup(int newCompanyId)
    {

        // Prepares for connecting to the database
        using var connection = new SqlConnection(_dataBaseConnectionString);

        // Establishes the connection to the database
        connection.Open();

        connection.Execute(@"DELETE FROM Companys WHERE userId = @UserId;
          DELETE FROM Users WHERE userId = @UserId;", new { UserId = newCompanyId });
    }

    ///<summary> 
    /// Test Driven Development 
    /// Test for ID: 002 - Create Company 
    /// Acceptance Criteria:  
    /// - Min oprettede profil skal gemmes i databasen. 
    /// </summary>
    ///  
    /// <remarks>
    /// Testing:
    /// Should verify that the profile is saved in the database, we will attempt to retrieve it by its UserId.
    ///</remarks>
    [Test]
    public void Should_Return_User_By_UserId()
    {
        // Act
        //Existing user in DB with with UserId 6
        Company user = _companyDao.GetOne(6);

        // Assert
        Assert.IsNotNull(user);
        Assert.AreEqual(6, user.UserId);
    }
    #endregion
}
