using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using Microsoft.AspNetCore.Mvc;
using System;


namespace InfiniteInfluence.API.Controllers;



[ApiController]
[Route("[controller]")]
public class InfluencersController : ControllerBase
{
    private readonly ILogger<InfluencersController> _logger;
    private IInfluencerDao _influencerDao;


    // This constructor might appear to have no references, but are called internally by the framework
    public InfluencersController(ILogger<InfluencersController> logger, IInfluencerDao influencerDao)
    {
        _logger = logger;
        _influencerDao = influencerDao;
    }



    [HttpPost]
    public ActionResult<int> Create(Influencer influencer)
    {
        try
        {
            return Ok(_influencerDao.Create(influencer));
        }

        catch (Exception exception)
        {
            _logger.LogError(exception, "An error has occured when attempting to create an Influencer.");

            var innerMessage = exception.InnerException?.Message;

            return StatusCode(500, $"Error: {exception.Message} | Inner: {innerMessage}");
        }
    }

    /// <summary>
    /// Retrieves an influencer by their user-Id.
    /// </summary>
    /// 
    /// <remarks>
    /// Returns a 200 OK response with the influencer if found. Returns a 400 Bad Request if the
    /// influencer does not exist. Returns a 500 Internal Server Error if an unexpected error occurs.</remarks>
    /// <param name="userId">The unique identifier of the influencer to retrieve.</param>
    /// 
    /// <returns>
    /// An <see cref="ActionResult{T}"/> containing the <see cref="Influencer"/> object if found; otherwise, a status
    /// code indicating the result of the operation.
    /// </returns>
    [HttpGet("{userId}")]
    public ActionResult<Influencer> GetOne(int userId)
    {
        try
        {
            Influencer? influencer = _influencerDao.GetOne(userId);
            
            // If the influencer with the specified userId does not exist, a 400 Bad Request response is returned.
            if (influencer == null)
            {
                return NoContent();
            }

            // If the influencer is found, it is returned with a 200 OK response.
            return Ok(influencer);
        }

        // If an error occurs during the operation, a 500 Internal Server Error response is returned with details about the error.
        catch (Exception exception)
        {
            _logger.LogError(exception, $"An error occurred trying to retrieve the influencer with user id {userId}.");
            var innerMessage = exception.InnerException?.Message;
            return StatusCode(500, $"Error: {exception.Message} | Inner: {innerMessage}");
        }
    }
}
