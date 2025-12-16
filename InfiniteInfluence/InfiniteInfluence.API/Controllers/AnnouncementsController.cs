using InfiniteInfluence.API.Dtos;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using Microsoft.AspNetCore.Mvc;


namespace InfiniteInfluence.API.Controllers;


/// <summary>
/// This controller handles HTTP requests related to Announcements.
/// </summary>>
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
    /// <summary>
    /// Creates a new announcement in the system.
    /// </summary>
    /// 
    /// <remarks>
    /// Method:      POST  
    /// Controller:  Announcements  
    /// Endpoint:    /announcements
    /// 
    /// This endpoint creates a new announcement based on the provided announcement data.
    /// On success, HTTP 201 (Created) is returned along with the generated announcement ID.
    /// </remarks>
    /// 
    /// <param name="announcement">
    /// The announcement object containing the data required to create a new announcement.
    /// </param>
    /// 
    /// <returns>
    /// Returns the unique identifier of the newly created announcement.
    /// </returns>
    [HttpPost]
    public ActionResult<int> Create(Announcement announcement)
    {
        try
        {
            // Calls upon the DAO class to create the new object and return its newly generated ID
            int createdAnnouncementId = _announcementDao.Create(announcement);

            // Returns the HTTP status code 201 (Created) along with the Id of the object that was created
            return StatusCode(StatusCodes.Status201Created, createdAnnouncementId);
        }

        catch (Exception exception)
        {
            _logger.LogError(exception, "An error has occured when attempting to create an Announcement.");

            string? innerMessage = exception.InnerException?.Message;

            return StatusCode(500, $"Error: {exception.Message} | Inner: {innerMessage}");
        }
    }
    #endregion



    #region Get All Announcements
    /// <summary>
    /// Retrieves all announcements available in the system.
    /// </summary>
    /// 
    /// <remarks>
    /// Method:      GET  
    /// Controller:  Announcements  
    /// Endpoint:    /announcements
    /// 
    /// This endpoint returns all announcements stored in the system.
    /// On success, HTTP 200 (OK) is returned with a list of announcements.
    /// </remarks>
    /// 
    /// <returns>
    /// A collection of announcements.
    /// </returns>
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

            string? innerMessage = exception.InnerException?.Message;

            return StatusCode(500, $"Error: {exception.Message} | Inner: {innerMessage}");
        }
    }
    #endregion



    #region Get one Announcement by ID
    /// <summary>
    /// Retrieves a single announcement by its unique identifier.
    /// </summary>
    /// 
    /// <remarks>
    /// Method:      GET  
    /// Controller:  Announcements  
    /// Endpoint:    /announcements/{announcementId}
    /// 
    /// This endpoint retrieves a specific announcement by its ID.
    /// If no announcement is found, HTTP 204 (No Content) is returned.
    /// </remarks>
    /// 
    /// <param name="announcementId">
    /// The unique identifier of the announcement.
    /// </param>
    /// 
    /// <returns>
    /// The announcement matching the specified identifier.
    /// </returns>
    [HttpGet("{announcementId:int}")]
    public ActionResult<Announcement> GetOne(int announcementId)
    {
        try
        {
            Announcement? announcement = _announcementDao.GetOne(announcementId);

            // If no announcement was found then execute this section
            if (announcement == null)
            {
                return NoContent();
            }

            return Ok(announcement);
        }

        catch (Exception exception)
        {
            _logger.LogError(exception, "An error has occurred when attempting to get an announcement with id {Id}.", announcementId);

            string? innerMessage = exception.InnerException?.Message;

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
    // ENDPOINT: /announcements/{announcementId}
    [HttpPost("{announcementId}")]
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

            string? innerMessage = exception.InnerException?.Message;

            return StatusCode(500, $"Error: {exception.Message} | Inner: {innerMessage}");
        }
    }



    /// <summary>
    /// Deletes an announcement with the specified identifier.
    /// </summary>
    /// 
    /// <remarks>
    /// Method:      DELETE  
    /// Controller:  Announcements  
    /// Endpoint:    /announcements/{announcementId}
    /// 
    /// This endpoint deletes an announcement by its ID.
    /// If the announcement does not exist, HTTP 404 (Not Found) is returned.
    /// On successful deletion, HTTP 200 (OK) is returned.
    /// </remarks>
    /// 
    /// <param name="announcementId">
    /// The unique identifier of the announcement to delete.
    /// </param>
    /// 
    /// <returns>
    /// True if the announcement was successfully deleted; otherwise false.
    /// </returns>
    [HttpDelete("{announcementId}")]
    public ActionResult<bool> Delete(int announcementId)
    {
        try
        {
            bool deleted = _announcementDao.Delete(announcementId);

            // If the deletion failed then execute this section
            if (!deleted)
            {
                // Produces a 404 status code signaling that the announcement with the specifeid id was not found
                return NotFound($"The announcement with id {announcementId} was not found.");
            }

            // Successfully deleted
            return Ok(true);
        }

        catch (Exception exception)
        {
            _logger.LogError(exception, "An error has occurred when attempting to delete an announcement with id {announcementId}.", announcementId);

            string? innerMessage = exception.InnerException?.Message;

            return StatusCode(500, $"Error: {exception.Message} | Inner: {innerMessage}");
        }
    }
    #endregion



    #region Update Announcement
    /// <summary>
    /// Updates an existing announcement.
    /// </summary>
    /// 
    /// <remarks>
    /// Method:      PUT  
    /// Controller:  Announcements  
    /// Endpoint:    /announcements/{announcementId}
    /// 
    /// This endpoint updates an existing announcement using optimistic concurrency control.
    /// If the announcement does not exist, HTTP 404 (Not Found) is returned.
    /// If a concurrency conflict occurs, HTTP 409 (Conflict) is returned.
    /// </remarks>
    /// 
    /// <param name="announcementId">
    /// The unique identifier of the announcement to update.
    /// </param>
    /// 
    /// <param name="announcementDto">
    /// The data transfer object containing updated announcement data, including RowVersion.
    /// </param>
    /// 
    /// <returns>
    /// True if the announcement was successfully updated.
    /// </returns>
    [HttpPut("{announcementId}")]
    public ActionResult<bool> Update(int announcementId, [FromBody] AnnouncementUpdateDto announcementDto)
    {
        try
        {
            // If the DTO is null or the DTO's id does not match the specified one then execute this section
            if (announcementDto == null || announcementId != announcementDto.AnnouncementId)
            {
                return BadRequest("The inserted Announcement ID did not match the announcement ID.");
            }

            // Calls upon the Data access layer to retrieve an announcement with the specified id
            Announcement existingAnnouncement = _announcementDao.GetOne(announcementId);

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
            _logger.LogError(exception, "An error has occurred when attempting to update an announcement with id {id}.", announcementId);

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
            AnnouncementId = dto.AnnouncementId,
            Title = dto.Title,
            LastEditDateTime = dto.LastEditDateTime,
            StartDisplayDateTime = dto.StartDisplayDateTime,
            EndDisplayDateTime = dto.EndDisplayDateTime,
            MaximumApplicants = dto.MaximumApplicants,
            MinimumFollowersRequired = dto.MinimumFollowersRequired,
            CommunicationType = dto.CommunicationType,
            AnnouncementLanguage = dto.AnnouncementLanguage,
            IsKeepProducts = dto.IsKeepProducts,
            IsPayoutNegotiable = dto.IsPayoutNegotiable,
            TotalPayoutAmount = dto.TotalPayoutAmount,
            ShortDescriptionText = dto.ShortDescriptionText,
            AdditionalInformationText = dto.AdditionalInformationText,
            StatusType = dto.StatusType,
            IsVisible = dto.IsVisible,
            ListOfSubjects = dto.ListOfSubjects,
            RowVersion = rowVersionInBytes
        };
    }

    #endregion
}
