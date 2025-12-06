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

public class CompanyApiClientTests
{
    private const string _dataBaseConnectionString = "Data Source=localhost;Initial Catalog=InfiniteInfluence;User ID=sa;Password=@12tf56so;Trust Server Certificate=True";

    private ICompanyDao _companyApiClient;

    [SetUp]
    public void Setup()
    {
        _companyApiClient = new CompanyDao(_dataBaseConnectionString);
    }


    #region Test for ID: 012 - GetOne Company

    /// <summary> 
    /// Test for ID: 012  - GetOne Company 
    /// Acceptance Criteria: 
    /// - Den specifikke virksomheds profils data kan hentes fra databasen.
    /// 
    /// </summary>>
    /// <remarks>
    /// Intergration test to verify that the ApiClient for getting a company profile works as expected.
    /// </remarks>
    [Test]
    public void GetOneCompanyProfile_FromApiClient()
    {
        /////////////////
        // - Arrange - //
        /////////////////

        // Instantiate with a userId that is known to exist in the test database
        int existingCompanyUserId = 6; 

        /////////////
        // - Act - //
        /////////////
        Company? company = _companyApiClient.GetOne(existingCompanyUserId);

        ////////////////
        // - Assert - //
        ////////////////
        Assert.That(company, Is.Not.Null, "API returned null – check that userId 6 exists.");
        Assert.That(company!.UserId, Is.EqualTo(existingCompanyUserId));
        Assert.That(company.CompanyName, Is.Not.Empty);
    }
    #endregion

}
