using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;
using InfiniteInfluence.DataAccessLibrary.Model;

namespace InfiniteInfluence.Tests;

public class CompanyDaoTests
{
    private const string _dataBaseConnectionString = "Data Source=localhost;Initial Catalog=InfiniteInfluence;User ID=sa;Password=@12tf56so;Trust Server Certificate=True";

    private ICompanyDao _companyDao;
    private Company? _testCompany;

    [SetUp]
    public void Setup()
    {
        _companyDao = new CompanyDao(_dataBaseConnectionString);
    }

    ///<summary> 
    /// Test Driven Development for ID: 002. 
    /// Acceptance Criteria: 
    /// - Jeg vil kunne oprette en profil med navn, og andre generelle oplysninger. 
    /// - Min oprettede profil skal gemmes i databasen. 
    /// </summary>
    [Test]
    public void CreateCompanyProfile_WithNameAndGeneralInfo_ShouldSucceed()
    {
        // Arrange
        _testCompany = new Company(
            loginEmail: "catchSushi@gmail.com",
            passwordHash: "hashpassword",
            isCompanyVerified: false,
            verificationDate: DateTime.UtcNow,
            companyName: "Catch Sushi - CompanyName",
            companyLogoUrl: "companyLogoUrl",
            companyDomains: new List<string> { "Food", "Vlog", "Starters"},
            ceoName: "Catch Ceo Name",
            dateOfEstablishment: new DateTime(1995, 3, 1),
            organisationNumber: "12345OrgNumber",
            standardIndustryClassification: "12345StandardIC",
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
        _companyDao.Create(_testCompany);

        // Assert
        // If the insert is successful, the userId should be set 
        Assert.IsNotNull(_testCompany.UserId); 
    }

    /// <summary>
    /// Cleanup method to remove test data from the database after each test.
    /// </summary>
    [TearDown]
    public void Cleanup()
    {
        if (_testCompany != null)
        {
            _companyDao.Delete(_testCompany.UserId);   
        }
    }

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
