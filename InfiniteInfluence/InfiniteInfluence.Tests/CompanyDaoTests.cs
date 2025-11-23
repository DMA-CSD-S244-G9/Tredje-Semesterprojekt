using Dapper;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;
using InfiniteInfluence.DataAccessLibrary.Model;
using Microsoft.Data.SqlClient;

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

    ///<summary> 
    /// Test Driven Development 
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
    /// 
    /// Testing:
    /// Should verify that the profile is saved in the database, we will attempt to retrieve it by its UserId.
    /// </summary>
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
}
