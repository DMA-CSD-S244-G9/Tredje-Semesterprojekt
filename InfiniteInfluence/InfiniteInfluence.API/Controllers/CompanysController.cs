using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;
using InfiniteInfluence.DataAccessLibrary.Model;
using Microsoft.AspNetCore.Mvc;
using System;

namespace InfiniteInfluence.API.Controllers;

/// <summary>
/// Provides API endpoints for managing company data, including creating, retrieving, 
/// and deleting companies.
/// </summary>
/// 
/// <remarks>
/// This controller handles HTTP requests related to company operations. 
/// It uses dependency injection to access the company data access object 
/// (<see cref="ICompanyDao"/>) and a logger (<see cref="ILogger{TCategoryName}"/>). 
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
        _logger = logger;
        _companyDao = companyDao;
    }

    #endregion
    /// <summary>
    /// Creates a new company record in the system.
    /// </summary>
    /// 
    /// <remarks>
    /// This method attempts to create a new company record using the provided <paramref
    /// name="company"/> object. Ensure that the input is valid and complete before calling this method. If an error
    /// occurs, the method logs the exception and returns an appropriate error response.
    /// </remarks>
    /// 
    /// <param name="company">
    /// The <see cref="Company"/> object containing the details of the company to be created. Cannot be null.
    /// </param>
    /// 
    /// <returns>An <see cref="ActionResult{T}"/> containing the unique identifier of the newly created company. Returns a status
    /// code of 500 if an error occurs during the operation.
    /// </returns>
    [HttpPost]
    public ActionResult<int> Create(Company company)
    {
        try
        {
            // Attempt to create the company and return the new company's userId
            return Ok(_companyDao.Create(company));
        }

        catch (Exception exception)
        {
            _logger.LogError(exception, "An error has occured when attempting to create a Company.");

            var innerMessage = exception.InnerException?.Message;

            return StatusCode(500, $"Error: {exception.Message} | Inner: {innerMessage}");
        }
    }


    /// <summary>
    /// Retrieves the company associated with the specified userId.
    /// </summary>
    /// 
    /// <remarks>
    /// This method attempts to retrieve the company information for the given user ID. If no company
    /// is found, a 204 No Content response is returned. In the event of an error, a 500 Internal Server Error response
    /// is returned, and the error is logged.
    /// </remarks>
    /// 
    /// <param name="userId">
    /// The unique identifier of the user whose company information is to be retrieved.
    /// </param>
    /// 
    /// <returns>
    /// An <see cref="ActionResult{T}"/> containing the company information if found; <see cref="NoContentResult"/> if
    /// no company is associated with the specified user ID; or a <see cref="StatusCodeResult"/> with a status code of
    /// 500 if an error occurs.
    /// </returns>
    [HttpGet("{userId}")]
    public ActionResult<Company> GetOne(int userId)
    {
        try
        {
            // Attempt to retrieve the company by userId and return it
            Company? company = _companyDao.GetOne(userId);
            if (company == null)
            {
                return StatusCode(400, "The company could not be found.");
            }

            return Ok(company);
        }

        catch (Exception exception)
        {
            _logger.LogError(exception, $"An error occurred trying to retrieve the company with user id {userId}.");

            var innerMessage = exception.InnerException?.Message;

            return StatusCode(500, $"Error: {exception.Message} | Inner: {innerMessage}");
        }
    }


    /// <summary>
    /// Deletes the user with the specified userId.
    /// </summary>
    /// 
    /// <remarks>
    /// If the user does not exist, the method returns a 204 No Content response. If an error occurs
    /// during the operation, a 500 Internal Server Error response is returned with details about the error.
    /// </remarks>
    /// 
    /// <param name="userId">
    /// The unique identifier of the user to delete.
    /// </param>
    /// 
    /// <returns>
    /// An <see cref="ActionResult{T}"/> containing <see langword="true"/> if the user was successfully deleted;
    /// otherwise, a status code indicating the result of the operation.
    /// </returns>
    [HttpDelete("{userId}")]
    public ActionResult<bool> Delete(int userId)
    {
        try
        {
            // Attempt to delete the user by userId
            var deleted = _companyDao.Delete(userId);
            if (!deleted)
            {
                return NoContent();
            }

            return Ok(true);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"An error occurred trying to delete the author with id {userId}.");

            var innerMessage = exception.InnerException?.Message;

            return StatusCode(500, $"Error: {exception.Message} | Inner: {innerMessage}");
        }
    }
}
