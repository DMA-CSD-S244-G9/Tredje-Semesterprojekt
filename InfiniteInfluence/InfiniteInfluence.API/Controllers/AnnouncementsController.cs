using InfiniteInfluence.API.Dtos;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;
using InfiniteInfluence.DataAccessLibrary.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System;

/// <summary>
/// This controller handles HTTP requests related to Announcements.
/// </summary>>
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

    #region Create Announcement
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
    #endregion


    #region Get All Announcements
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
    #endregion


    #region Get one Announcement by ID
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
    #endregion


    #region Submit Application to Announcement
    /// <summary>
    /// Submits an application for an influencer to an announcement. </summary>
    /// 
    /// <remarks>
    /// This method logs any unexpected errors that occur during the operation. </remarks>
    /// 
    /// <param name="announcementId">
    /// The unique identifier of the announcement to which the application is being submitted.</param>
    /// 
    /// <param name="request">
    /// The application details, including the influencer's user ID.</param>
    /// 
    /// 
    /// <returns>
    /// A status code indicating the result of the operation: 
    /// OkObjectResult - with a value of "true" if the application is successfully
    /// submitted.
    /// 
    /// StatusCodeResult - with a status code of 500 if
    /// the application could not be saved.
    /// 
    /// BadRequestObjectResult - if the operation fails due to an invalid state, such as the influencer already applying or the maximum number of
    /// applicants being reached.
    /// 
    /// StatusCodeResult - with a status code of 500 if an unexpected error occurs, including the error details in the response
    /// body.
    /// </returns>
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
    #endregion


    #region Update Announcement
    /// <summary>
    /// Updates an existing announcement with the specified ID using the provided data.
    /// </summary>
    /// 
    /// <remarks>
    /// This method validates that the provided ID matches the ID in the announcementDto. 
    /// If the IDs do not match, a BadRequestResult is returned. 
    /// 
    /// If an error occurs during the update process, a 500 Internal Server Error is returned with details about the
    /// exception.
    /// 
    /// The unique identifier of the announcement to be updated. Must match the ID in announcementDto
    /// 
    /// announcementDtoThe data transfer object containing the updated announcement details. 
    /// The AnnouncementUpdateDto.AnnouncementId must match the id.</remarks>
    /// 
    /// <returns>True if the announcement was successfully updated; NoContentResult if the update
    /// operation did not modify any data; or an appropriate HTTP status code indicating the result of the operation.</returns>
    // POST
    // ENDPOINT: /announcements/{announcementId}
    [HttpPut("{id}")]
    public ActionResult<bool> Update(int id, [FromBody] AnnouncementUpdateDto announcementDto)
    {
        try
        {
            // If the DTO is null or the DTO's id does not match the specified one then execute this section
            if (announcementDto == null || id != announcementDto.AnnouncementId)
            {
                return BadRequest("The inserted Announcement ID did not match the announcement ID.");
            }
            
            // Calls upon the Data access layer to retrieve an announcement with the specified id
            Announcement existingAnnouncement = _announcementDao.GetOne(id);
            
            // If no announcement with a matching id could be found then execute this section
            if (existingAnnouncement == null)
            {
                return NotFound();
            }

            // Attempts to update the announcement to reflect the DTO including the RowVersion
            Announcement announcementToUpdate = MapToUpdatedAnnouncement(announcementDto);

            // Attempts to update the information of the announcement with the specified id to have the newly changed properties
            bool isAnnouncementUpdated = _announcementDao.Update(announcementToUpdate);

            // If the updating of the announcement failed then execute this section
            if (!isAnnouncementUpdated)
            {
                return StatusCode(500, "The announcement could not be updated.");
            }

            return Ok(true);
        }


        catch (InvalidOperationException exception)
        {
            // This exception will usually occur from a concurrency 409 status code conflict in the DAO
            return Conflict(exception.Message); 
        }


        catch (Exception exception)
        {
            _logger.LogError(exception, "An error has occurred when attempting to update an announcement with id {id}.", id);

            string? innerMessage = exception.InnerException?.Message;

            return StatusCode(500, $"Error: {exception.Message} | Inner: {innerMessage}");
        }
    }
    #endregion


    #region helper method
    /// <summary>
    /// Maps an AnnouncementUpdateDto object to a new Announcement object.
    /// The parameter, data transfer object(DTO), containing the updated announcement details.
    /// </summary>
    /// 
    /// <returns>
    /// A new Announcement object populated with the values from the specified <paramref name="dto"/>.</returns>
    private Announcement MapToUpdatedAnnouncement(AnnouncementUpdateDto dto)
    {
        // Convert RowVersion from Base64 string into byte[] (or empty array if missing)
        byte[] rowVersionInBytes;

        if (string.IsNullOrWhiteSpace(dto.RowVersion))
        {
            rowVersionInBytes = Array.Empty<byte>();
        }
        else
        {
            rowVersionInBytes = Convert.FromBase64String(dto.RowVersion);
        }

        return new Announcement
        {
            AnnouncementId              = dto.AnnouncementId,
            Title                       = dto.Title,
            LastEditDateTime            = dto.LastEditDateTime,
            StartDisplayDateTime        = dto.StartDisplayDateTime,
            EndDisplayDateTime          = dto.EndDisplayDateTime,
            MaximumApplicants           = dto.MaximumApplicants,
            MinimumFollowersRequired    = dto.MinimumFollowersRequired,
            CommunicationType           = dto.CommunicationType,
            AnnouncementLanguage        = dto.AnnouncementLanguage,
            IsKeepProducts              = dto.IsKeepProducts,
            IsPayoutNegotiable          = dto.IsPayoutNegotiable,
            TotalPayoutAmount           = dto.TotalPayoutAmount,
            ShortDescriptionText        = dto.ShortDescriptionText,
            AdditionalInformationText   = dto.AdditionalInformationText,
            StatusType                  = dto.StatusType,
            IsVisible                   = dto.IsVisible,
            ListOfSubjects              = dto.ListOfSubjects,
            RowVersion                  = rowVersionInBytes
        };
    }

    #endregion
}
