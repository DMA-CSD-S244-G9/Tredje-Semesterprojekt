using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;
using InfiniteInfluence.DataAccessLibrary.Model;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Provides API endpoints for managing influencer data, including creating and retrieving influencers.
/// The controller uses dependency injection to access the influencer data access object IInfluencerDao.
/// 
/// </summary>

namespace InfiniteInfluence.API.Controllers;


[ApiController]
[Route("[controller]")]
public class InfluencersController : ControllerBase
{
    #region Attributes and Constructor
    private readonly ILogger<InfluencersController> _logger;
    private IInfluencerDao _influencerDao;


    // This constructor might appear to have no references, but are called internally by the framework
    public InfluencersController(ILogger<InfluencersController> logger, IInfluencerDao influencerDao)
    {
        // Logger used for logging errors and information about API context
        _logger = logger;
        _influencerDao = influencerDao;
    }
    #endregion


    #region create Influencer
    /// <summary>
    /// Creates a new influencer and returns the id of the created record.
    /// </summary>
    /// 
    /// <remarks>If the creation is successful, the method returns an HTTP 200 response with the new
    /// influencer's ID. If an error occurs during creation, the method returns an HTTP 500 response with error
    /// details.
    /// </remarks>
    /// 
    /// <returns>
    /// An ActionResult containing the id of the newly created influencer if successful;
    /// otherwise, an error response.
    /// </returns>
    [HttpPost]
    public ActionResult<int> Create(Influencer influencer)
    {
        try
        {
            // Calls upon the DAO class to create the new object and return its newly generated ID
            int createdInfluencerId = _influencerDao.Create(influencer);

            // Returns the HTTP status code 201 (Created) along with the Id of the object that was created
            return StatusCode(StatusCodes.Status201Created, createdInfluencerId);
        }

        catch (Exception exception)
        {
            // Logs the exception and returns a 500 Internal Server Error response with error details.
            _logger.LogError(exception, "An error has occured when attempting to create an Influencer.");

            // Retrieves the inner exception message if available.
            string? innerMessage = exception.InnerException?.Message;

            // Returns a 500 Internal Server Error response with the error message and inner exception details.
            return StatusCode(500, $"Error: {exception.Message} | Inner: {innerMessage}");
        }
    }
    #endregion



    #region Get One Influencer
    /// <summary>
    /// Retrieves an influencer by their user-Id.
    /// </summary>
    /// 
    /// <remarks>
    /// Returns a 200 OK response with the influencer if found. Returns a 400 Bad Request if the
    /// influencer does not exist. Returns a 500 Internal Server Error if an unexpected error occurs.
    /// </remarks>
    /// 
    /// <returns>
    /// An ActionResult containing the Influencer object if found; otherwise, a status
    /// code indicating the result of the operation.
    /// </returns>
    [HttpGet("{userId}")]
    public ActionResult<Influencer> GetOne(int userId)
    {
        try
        {
            // Attempts to retrieve the influencer by userId with influencerDao GetOne method.
            Influencer? influencer = _influencerDao.GetOne(userId);

            // If no influencer is found with the specified userId, a 404 Not Found response is returned.
            if (influencer == null)
            {
                // Returns the 404 status code, indicating no object matching the userid was found
                return NotFound();
            }

            // Returns the influencer information with a 200 OK response if found.
            return Ok(influencer);
        }

        // If an error occurs during the operation, a 500 Internal Server Error response is returned with details about the error.
        catch (Exception exception)
        {
            // Logs the exception and returns a 500 Internal Server Error response with error details.
            _logger.LogError(exception, $"An error occurred trying to retrieve the influencer with user id {userId}.");
            
            // Retrieves the inner exception message if available.
            string? innerMessage = exception.InnerException?.Message;

            // Returns a 500 Internal Server Error response with the error message and inner exception details.
            return StatusCode(500, $"Error: {exception.Message} | Inner: {innerMessage}");
        }
    }
    #endregion
}
