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


    // POST
    // ENDPOINT: /announcements/create
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



    // GET
    // ENDPOINT: /announcements/
    [HttpGet]
    public ActionResult<IEnumerable<Announcement>> GetAll()
    {
        try
        {
            return Ok(_announcementDao.GetAll());
        }

        catch (Exception exception)
        {
            _logger.LogError(exception, "An error has occured when attempting to get all Announcements.");

            var innerMessage = exception.InnerException?.Message;

            return StatusCode(500, $"Error: {exception.Message} | Inner: {innerMessage}");
        }
    }



    // GET
    // ENDPOINT: /announcements/{id}
    [HttpGet("{id:int}")]
    public ActionResult<Announcement> GetOne(int id)
    {
        try
        {
            Announcement? announcement = _announcementDao.GetOne(id);

            // If no announcement was found then execute this section
            if (announcement == null)
            {
                return NoContent();
            }

            return Ok(announcement);
        }

        catch (Exception exception)
        {
            _logger.LogError(exception, "An error has occurred when attempting to get an announcement with id {Id}.", id);
            
            var innerMessage = exception.InnerException?.Message;
            
            return StatusCode(500, $"Error: {exception.Message} | Inner: {innerMessage}");
        }
    }
}
