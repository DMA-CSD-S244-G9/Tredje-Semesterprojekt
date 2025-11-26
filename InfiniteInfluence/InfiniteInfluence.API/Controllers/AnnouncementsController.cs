using InfiniteInfluence.API.Dtos;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;
using InfiniteInfluence.DataAccessLibrary.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;


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
    // ENDPOINT: /announcements/index
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
    // ENDPOINT: /announcements/{announcementId}
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



    // POST
    // ENDPOINT: /announcements/{announcementId}/apply
    [HttpPost("{announcementId}/apply")]
    public IActionResult Apply(int announcementId, [FromBody] ApplyRequest request)
    {
        try
        {
            bool isApplicationSubmitted = _announcementDao.AddInfluencerApplication(announcementId, request.InfluencerUserId);

            if (isApplicationSubmitted == false)
            {
                return StatusCode(500, "The application could not be saved.");
            }

            return Ok(true);
        }

        // TODO: Make a proper exception like max applicants already reached, or you already applied to this
        catch (InvalidOperationException exception)
        {
            // Returns a bad request containing the exception message and sends it as part of the body
            return BadRequest(exception.Message);
        }


        catch (Exception exception)
        {
            _logger.LogError(exception, "An error has occurred when attempting to add an influencer {influencerId} to an announcement with id {announcementId}.", request.InfluencerUserId, announcementId);

            var innerMessage = exception.InnerException?.Message;

            return StatusCode(500, $"Error: {exception.Message} | Inner: {innerMessage}");
        }
    }

}
