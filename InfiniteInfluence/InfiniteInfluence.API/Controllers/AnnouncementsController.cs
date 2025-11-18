using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;
using InfiniteInfluence.DataAccessLibrary.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;


namespace InfiniteInfluence.API.Controllers;




[ApiController]
[Route("[controller]")]
public class AnnouncementsController : ControllerBase
{
    private readonly ILogger<AnnouncementsController> _logger;
    private readonly IAnnouncementDao _announcementDao;


    // This constructor might appear to have no references, but are called internally by the framework
    public AnnouncementsController(ILogger<AnnouncementsController> logger, IAnnouncementDao announcementDao)
    {
        _logger = logger;
        _announcementDao = announcementDao;
    }



    [HttpPost]
    public ActionResult<int> Create(Announcement announcement)
    {
        try
        {
            return Ok(_announcementDao.Create(announcement));
        }

        catch (Exception exception)
        {
            _logger.LogError(exception, "An error has occured when attempting to create an Announcement.");

            var innerMessage = exception.InnerException?.Message;

            return StatusCode(500, $"Error: {exception.Message} | Inner: {innerMessage}");
        }
    }
}
