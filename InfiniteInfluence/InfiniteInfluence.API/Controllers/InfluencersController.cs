using Microsoft.AspNetCore.Mvc;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;


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
}
