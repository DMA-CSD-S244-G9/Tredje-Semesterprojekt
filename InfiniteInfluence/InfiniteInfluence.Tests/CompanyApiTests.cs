using InfiniteInfluence.API.Controllers;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;
using InfiniteInfluence.DataAccessLibrary.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions; //NullLogger for testing


namespace InfiniteInfluence.Tests;

public class CompanyApiTests
{
    private const string _dataBaseConnectionString = "Data Source=localhost;Initial Catalog=InfiniteInfluence;User ID=sa;Password=@12tf56so;Trust Server Certificate=True";

    private ICompanyDao _companyDao;

    [SetUp]
    public void Setup()
    {
        _companyDao = new CompanyDao(_dataBaseConnectionString);
    }


    #region Getone
    /// <summary>
    /// Test for ID: 012  - GetOne Company 
    /// Acceptance Criteria: 
    /// - Den specifikke virksomheds profils data kan hentes fra databasen.
    /// Intergration test to verify that the API endpoint for getting a company profile works as expected.
    /// </summary>
    [Test]
    public void GetOneCompanyProfile_FromApi()
    {
        /////////////////
        // - Arrange - //
        /////////////////

        // Instance of Company ID to be retrieved, this ID should exist in the test database
        int expectedCompanyId = 6;

        // A mock logger, that doesnt log anything, just for testing purposes
        var nullLogger = NullLogger<CompanysController>.Instance;

        CompanysController companyApi = new CompanysController(nullLogger, _companyDao);


        /////////////
        // - Act - //
        /////////////
        // Using ActionResult to simulate API response
        ActionResult<Company> companyResult = companyApi.GetOne(expectedCompanyId);


        ////////////////
        // - Assert - //
        ////////////////
        Assert.IsNotNull(companyResult);
    }
    #endregion

}
