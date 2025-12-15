using Dapper;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using InfiniteInfluence.DataAccessLibrary.Tools;
using System.Data;
using System.Transactions;


namespace InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;


/// <summary>
/// Provides data access operations for managing company in the database.
/// </summary>
/// 
/// <remarks>
/// This class is responsible for creating, retrieving, and deleting company records, 
/// It ensures atomicity of operations by utilizing transactions for multi-step database interactions.
/// It implements the ICompanyDao interface and extends the BaseConnectionDao for database connection management.
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


    #region SQL Query - create

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


    #region SQL Query - get one
    // SQL statement for retrieving a company by userId
    // Uses an INNER JOIN to combine data from Users and Companys tables
    // To ensure that the userId entered is a company Id and also to get access to the loginEmail and passwordHash from Users table when login is implemented
    private readonly string? queryFindCompany = @"
                SELECT
                    u.userId, u.loginEmail, u.passwordHash,

                    c.isCompanyVerified, c.verificationDate,
                    c.companyName, c.companyLogoUrl, c.ceoName,
                    c.dateOfEstablishment, c.organisationNumber, c.standardIndustryClassification, 
                    c.websiteUrl, c.companyEmail, c.companyPhoneNumber, c.country,
                    c.companyState, c.city, c.companyAddress,
                    c.companyLanguage, c.biography, c.contactPerson,
                    c.contactEmailAddress, c.contactPhoneNumber


                FROM [Users] u
                INNER JOIN Companys c ON c.userId = u.userId
                WHERE u.userId = @UserId;";

    // SQL statement for retrieving company domains by userId
    private readonly string? queryFindCompanyDomains = @"
                SELECT domain
                FROM CompanyDomains
                WHERE userId = @UserId;";
    #endregion


    #region Create a company

    /// <summary>
    /// This method creates a company in the database.
    /// It performs multiple insert operations within a transaction to ensure atomicity.
    /// It uses Dapper for database interactions.
    /// </summary>
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
            // Hashes the password before storing it in the database
            string passwordHash = BCryptTool.HashPassword(company.PasswordHash);

            // Uses dapper to insert into the Users table and return the newest generated UserId using SCOPE_IDENTITY()
            // Dapper parameterizes the query to prevent SQL injection attacks
            int newUserId = connection.QuerySingle<int>(queryInsertUser, new { company.LoginEmail, passwordHash }, transaction);

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
                // Loops through each domain and inserts it into the CompanyDomains table
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
    #endregion


    #region Get one company
    /// <summary>
    /// Retrieves a company associated with the specified user ID.
    /// </summary>
    /// 
    /// <remarks>
    /// This method queries the database to retrieve the company details and its associated domains
    /// for the given user ID. If no company is found for the specified user ID, the method returns null.
    /// </remarks>
    /// 
    /// <returns>
    /// A Company object containing the company's details and associated domains if the user ID is found;
    /// otherwise,null.
    /// </returns>
    public Company? GetOne(int userId)
    {
        using IDbConnection connection = CreateConnection();
        try
        {
            // Dapper maps both the BaseUser and the Company classes' properties
            Company? foundCompany = connection.QuerySingleOrDefault<Company>(queryFindCompany, new { UserId = userId });

            // Returns null if the Company is not found using the guard clause
            if (foundCompany == null)
            {
                return null;
            }

            // If the company is found then attempt to find the 'Companys' influencing domains from the CompanyDomains table
            IEnumerable<string> domains = connection.Query<string>(queryFindCompanyDomains, new { UserId = userId });

            // Adds the found domains to the list of company domains
            foundCompany.CompanyDomains = domains.ToList();

            return foundCompany;
        }
        // Catches any exceptions that occur during the database operations and wraps them in a DataException
        catch (Exception exception)
        {
            throw new DataException("Failed to retrieve company from the database.", exception);
        }
    }
    #endregion


    #region Delete company
    //TODO: write comments
    public bool Delete(int userId)
    {
        string sqlQueryDelete = "DELETE FROM Companys WHERE userId = @UserId";
        using IDbConnection connection = CreateConnection();
        int rows = connection.Execute(sqlQueryDelete, new { UserId = userId });
        return rows > 0;
    }

    #endregion
}
