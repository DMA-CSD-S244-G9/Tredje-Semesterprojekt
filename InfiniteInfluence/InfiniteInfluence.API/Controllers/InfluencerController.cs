using Microsoft.AspNetCore.Mvc;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;

namespace InfiniteInfluence.API.Controllers;



[ApiController]
[Route("[controller]")]
public class InfluencerController : ControllerBase
{
    private readonly ILogger<InfluencerController> _logger;

    private IInfluencerDao _influencerDao;



    public InfluencerController(ILogger<InfluencerController> logger, IInfluencerDao influencerDao)
    {
        _logger = logger;

        _influencerDao = influencerDao;
    }



    [HttpPost]
    public ActionResult<int> Create(Influencer influencer)
    {
        try
        {
            //TODO: 
            //            return Ok(_influencerDao.Create(influencer));
            return Ok();
        }

        catch (Exception exception)
        {
            return StatusCode(500, $"An error has occured when attempting to create an Influencer.");
        }
    }
}
