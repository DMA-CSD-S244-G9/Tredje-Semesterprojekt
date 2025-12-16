using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;
using InfiniteInfluence.DataAccessLibrary.Model;
using Microsoft.AspNetCore.Mvc;


namespace InfiniteInfluence.API.Controllers;


/// <summary>
/// Provides API endpoints for managing company data, including creating, retrieving, 
/// and deleting companies.
/// </summary>
/// 
/// <remarks>
/// This controller handles HTTP requests related to company operations. 
/// It uses dependency injection to access the company data access object 
/// (ICompanyDao) and a logger (ILogger{TCategoryName}). 
/// The controller includes endpoints for creating a new company, retrieving a company
/// by userId, and deleting a company.
/// </remarks>

// Applys attributes to define this class as an API controller and set the route
// The route is set to the name of the controller without the "Controller" suffix
[ApiController]
[Route("[controller]")]
public class CompanysController : Controller
{

    #region Attributes and Constructor
    // Logger for logging information and errors about the controller's operations
    private readonly ILogger<CompanysController> _logger;

    // Data access object for performing operations related to companies
    ICompanyDao _companyDao;


    // This constructor might appear to have no references, but are called internally by the framework
    public CompanysController(ILogger<CompanysController> logger, ICompanyDao companyDao)
    {
        // Logger used for logging errors and information about API context
        _logger = logger;
        _companyDao = companyDao;
    }
    #endregion



    #region create Company

    /// <summary>
    /// Creates a new company in the system.
    /// </summary>
    /// 
    /// <remarks>
    /// Method:      POST  
    /// Controller:  Company  
    /// Endpoint:    /companys
    /// 
    /// This endpoint creates a new company record and returns its generated identifier.
    /// On success, the API responds with HTTP 201 (Created).
    /// </remarks>
    /// 
    /// <param name="company">
    /// The company object containing the information required to create a new company.
    /// </param>
    /// 
    /// <returns>
    /// Returns the generated identifier of the newly created company.
    /// </returns>
    [HttpPost]
    public ActionResult<int> Create(Company company)
    {
        try
        {
            // Calls upon the DAO class to create the new object and return its newly generated ID
            int createdCompanyId = _companyDao.Create(company);

            // Returns the HTTP status code 201 (Created) along with the Id of the object that was created
            return StatusCode(StatusCodes.Status201Created, createdCompanyId);
        }

        catch (Exception exception)
        {
            // Logs the exception and returns a 500 Internal Server Error response with error details.
            _logger.LogError(exception, "An error has occured when attempting to create an company.");

            // Retrieves the inner exception message if available.
            string? innerMessage = exception.InnerException?.Message;

            // Returns a 500 Internal Server Error response with the error message and inner exception details.
            return StatusCode(500, $"Error: {exception.Message} | Inner: {innerMessage}");
        }
    }
    #endregion



    #region Get One Company by UserId 

    /// <summary>
    /// Retrieves a single company based on the associated user identifier.
    /// </summary>
    /// 
    /// <remarks>
    /// Method:      GET  
    /// Controller:  Company  
    /// Endpoint:    /companys/{userId}
    /// 
    /// This endpoint retrieves a company linked to the specified user identifier.
    /// If no company exists, a client error response is returned.
    /// </remarks>
    /// 
    /// <param name="userId">
    /// The identifier of the user associated with the company.
    /// </param>
    /// 
    /// <returns>
    /// Returns the company if found; otherwise an error response.
    /// </returns>
    [HttpGet("{userId}")]
    public ActionResult<Company> GetOne(int userId)
    {
        try
        {
            // Attempts to retrieve the company by userId with CompanyDao GetOne method.
            Company? company = _companyDao.GetOne(userId);

            // If the company with the specified userId does not exist, a 400 Bad Request response is returned.
            if (company == null)
            {
                return StatusCode(400, "The company could not be found.");
            }

            // Returns the company information with a 200 OK response if found.
            return Ok(company);
        }

        catch (Exception exception)
        {
            // Logs the exception and returns a 500 Internal Server Error response with error details.
            _logger.LogError(exception, $"An error occurred trying to retrieve the company with user id {userId}.");

            // Retrieves the inner exception message if available.
            string? innerMessage = exception.InnerException?.Message;

            //// Returns a 500 Internal Server Error response with the error message and inner exception details.
            return StatusCode(500, $"Error: {exception.Message} | Inner: {innerMessage}");
        }
    }
    #endregion



    #region Delete Company by UserId
    // TODO: Fully implement the delete functionality in the system, so it extends
    // to the ApiClient and actual MVC's interface.

    /// <summary>
    /// Deletes a company associated with the specified user identifier.
    /// </summary>
    /// 
    /// <remarks>
    /// Method:      DELETE  
    /// Controller:  Company  
    /// Endpoint:    /companys/{userId}
    /// 
    /// This endpoint attempts to delete a company linked to the specified user identifier.
    /// If no company exists for the given userId, HTTP 204 (No Content) is returned.
    /// On successful deletion, HTTP 200 (OK) is returned with a boolean confirmation.
    /// </remarks>
    /// 
    /// <param name="userId">
    /// The identifier of the user associated with the company to be deleted.
    /// </param>
    /// 
    /// <returns>
    /// Returns true if the company was successfully deleted; otherwise false if no company was found.
    /// </returns>
    [HttpDelete("{userId}")]
    public ActionResult<bool> Delete(int userId)
    {
        try
        {
            // Attempt to delete the user by userId
            bool deleted = _companyDao.Delete(userId);
            
            if (!deleted)
            {
                return NoContent();
            }

            return Ok(true);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"An error occurred trying to delete the author with id {userId}.");

            string? innerMessage = exception.InnerException?.Message;

            return StatusCode(500, $"Error: {exception.Message} | Inner: {innerMessage}");
        }
    }
    #endregion
}
