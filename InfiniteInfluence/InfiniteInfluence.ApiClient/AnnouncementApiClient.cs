using InfiniteInfluence.API.Dtos;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using System.Net;
using RestSharp;


namespace InfiniteInfluence.ApiClient;


public class AnnouncementApiClient : IAnnouncementDao
{
    #region attributes and constructor
    // The address of the REST Web API server's URI e.g. "https://localhost:7777"
    private readonly string _apiUri;

    // The rest client from restsharp to call the server the RestClient is threadsafe and intended to be reused for multiple requests
    private readonly RestClient _restClient;


    /// <summary> 
    /// Initializes a new instance of the <see cref="AnnouncementApiClient"/> class with the specified API URI
    /// utilizing dependency injection from Website project's Program.cs.
    /// </summary>>
    public AnnouncementApiClient(string apiUri)
    {
        _apiUri = apiUri;
        _restClient = new RestClient(apiUri);
    }
    #endregion



    #region Create announcement
    /// <summary>
    /// Sends a request to the REST Web API to create a new announcement.
    /// </summary>
    /// 
    /// <remarks>
    /// Method:      POST  
    /// Controller:  Announcement  
    /// Endpoint:    /announcements
    ///
    /// This method serializes the supplied <see cref="Announcement"/> object to JSON and sends it
    /// as the body of a HTTP POST request to the REST Web API.
    /// The API is expected to create a new announcement and return the generated announcement ID
    /// along with the HTTP status code 201 (Created).
    /// </remarks>
    /// 
    /// <param name="announcement">
    /// The <see cref="Announcement"/> object containing the data required to create a new announcement.
    /// </param>
    /// 
    /// <returns>
    /// The unique identifier of the newly created announcement.
    /// </returns>
    /// 
    /// <exception cref="Exception">
    /// Thrown if the API returns a non-success HTTP status code, or if the expected
    /// 201 (Created) status code is not returned.
    /// </exception>
    public int Create(Announcement announcement)
    {
        // Creates a rest request representing a HTTP request
        RestRequest? restHttpRequest = new RestRequest("announcements", Method.Post);

        // RestSharp serializes the c# object to a JSON format and adds it to the body in the HTTP request, and when
        // the request is sent the API automatically deserializes the JSON back to a C# object
        restHttpRequest.AddJsonBody(announcement);

        // Sends the HTTP request to the Rest Web API and receives a HTTP response which contains:
        // a HTTP status code, whether the call was a success, potential exception info and the 
        // deserialized C# object that restsharp has already mapped for us under the hood
        RestResponse<int> restHttpResponse = _restClient.Execute<int>(restHttpRequest);

        // If the response's HTTP status code is not in the 200 series, no network errors and no transport level errors occured then execute this section
        if (!restHttpResponse.IsSuccessful)
        {
            throw new Exception($"The API returned with the following error. Status: {(int)restHttpResponse.StatusCode} - {restHttpResponse.StatusDescription}. Body: {restHttpResponse.Content}");
        }

        // If the response's HTTP status code was not 201 (Created) then execute this section
        if (restHttpResponse.StatusCode != HttpStatusCode.Created)
        {
            throw new Exception("The API returned a successful HTTP response, but the expected 201 (Created) status code was not returned.");
        }

        // Retrieves the deserialized object from the HTTP respones. The response.Data contains the JSON
        // response from the API which RestSharp automatically has deserialized and mapped to the specific type of C# object, or null if no data was retrieved
        int generatedAnnouncementId = restHttpResponse.Data;

        return generatedAnnouncementId;
    }
    #endregion



    #region Get all announcements
    /// <summary>
    /// Retrieves all announcements from the REST Web API.
    /// </summary>
    /// 
    /// <remarks>
    /// Method:      GET  
    /// Controller:  Announcement  
    /// Endpoint:    /announcements
    /// 
    /// This method sends an HTTP GET request to the REST Web API in order to retrieve
    /// all existing announcements. The API is expected to return a collection of
    /// <see cref="Announcement"/> objects serialized as JSON.
    /// 
    /// A successful response (HTTP 200) will return a list of announcements.  
    /// If the API returns HTTP 204 (No Content), or if the response body could not
    /// be deserialized, an empty collection is returned instead.
    /// 
    /// Any non-successful HTTP response (outside the 2xx range) will result in an
    /// exception being thrown.
    /// </remarks>
    /// 
    /// <returns>
    /// An <see cref="IEnumerable{Announcement}"/> containing all announcements,
    /// or an empty collection if no announcements were found.
    /// </returns>
    /// 
    /// <exception cref="Exception">
    /// Thrown if the REST Web API returns a non-successful HTTP status code.
    /// </exception>
    public IEnumerable<Announcement> GetAll()
    {
        // Creates a rest request representing a HTTP request
        RestRequest? restHttpRequest = new RestRequest("announcements", Method.Get);

        // Sends the HTTP request to the Rest Web API and receives a HTTP response which contains:
        // a HTTP status code, whether the call was a success, potential exception info and the 
        // deserialized C# object that restsharp has already mapped for us under the hood
        RestResponse<List<Announcement>> restHttpResponse = _restClient.Execute<List<Announcement>>(restHttpRequest);

        // If the response's HTTP status code is not in the 200 series, no network errors and no transport level errors occured then execute this section
        if (!restHttpResponse.IsSuccessful)
        {
            throw new Exception($"The API returned with the following error. Status: {(int)restHttpResponse.StatusCode} - {restHttpResponse.StatusDescription}. Body: {restHttpResponse.Content}");
        }

        // If the response's HTTP status code is 204 (No Content), likely because the response.Data was null then execute this section
        if (restHttpResponse.StatusCode == HttpStatusCode.NoContent)
        {
            return new List<Announcement>();
        }

        // If RestSharp failed to deserialize the content into specified object then execute this esction
        if (restHttpResponse.Data == null)
        {
            return new List<Announcement>();
        }

        return restHttpResponse.Data;
    }
    #endregion



    #region Get one announcement by announcementid
    /// <summary>
    /// Retrieves a single announcement from the REST Web API by its unique identifier.
    /// </summary>
    /// 
    /// <remarks>
    /// Method:      GET  
    /// Controller:  Announcement  
    /// Endpoint:    /announcements/{announcementId}
    ///
    /// This method sends a HTTP GET request to the REST Web API to retrieve an announcement.
    /// The API is expected to return the announcement as a JSON object, which RestSharp
    /// automatically deserializes into an <see cref="Announcement"/> instance.
    /// </remarks>
    /// 
    /// <param name="announcementId">
    /// The unique identifier of the announcement to retrieve.
    /// </param>
    /// 
    /// <returns>
    /// An <see cref="Announcement"/> instance if found; otherwise <c>null</c>.
    /// </returns>
    /// 
    /// <exception cref="Exception">
    /// Thrown if the API returns a 404 (Not Found) status code or any other non-success
    /// HTTP response.
    /// </exception>
    public Announcement? GetOne(int announcementId)
    {
        // Creates a rest request representing a HTTP request
        RestRequest? restHttpRequest = new RestRequest($"announcements/{announcementId}", Method.Get);

        // Sends the HTTP request to the Rest Web API and receives a HTTP response which contains:
        // a HTTP status code, whether the call was a success, potential exception info and the 
        // deserialized C# object that restsharp has already mapped for us under the hood
        RestResponse<Announcement> restHttpResponse = _restClient.Execute<Announcement>(restHttpRequest);

        // If the response's HTTP status is the 404 (Not Found) indicating a null return then execute this section
        if (restHttpResponse.StatusCode == HttpStatusCode.NotFound)
        {
            throw new Exception($"The API returned with the status code 404 (Not found). Possibly caused by the object being null.");
        }

        // If the response's HTTP status code is not in the 200 series, no network errors and no transport level errors occured then execute this section
        if (!restHttpResponse.IsSuccessful)
        {
            throw new Exception($"The API returned with the following error. Status: {(int)restHttpResponse.StatusCode} - {restHttpResponse.StatusDescription}. Body: {restHttpResponse.Content}");
        }

        // Retrieves the deserialized object from the HTTP respones. The response.Data contains the JSON
        // response from the API which RestSharp automatically has deserialized and mapped to the specific type of C# object, or null if no data was retrieved
        Announcement? foundAnnouncement = restHttpResponse.Data;

        return foundAnnouncement;
    }
    #endregion



    #region Add influencer application to announcement
    /// <summary>
    /// Submits an application from an influencer to a specific announcement.
    /// </summary>
    /// 
    /// <param name="announcementId">
    /// The identifier of the announcement the influencer is applying to.
    /// </param>
    /// 
    /// <param name="influencerUserId">
    /// The identifier of the influencer submitting the application.
    /// </param>
    /// 
    /// <returns>
    /// Returns <c>true</c> if the application was successfully created; otherwise <c>false</c>.
    /// </returns>
    /// 
    /// <exception cref="InvalidOperationException">
    /// Thrown if the REST Web API rejects the request with a 400 (Bad Request),
    /// typically caused by business rule violations such as no remaining available slots,
    /// duplicate applications, or other domain-specific constraints.
    /// </exception>
    /// 
    /// <exception cref="Exception">
    /// Thrown if the REST Web API returns an unsuccessful HTTP status code (non-2xx),
    /// indicating a server-side, transport-level, or unexpected error.
    /// </exception>
    /// 
    /// <remarks>
    /// Method:      POST  
    /// Controller:  Announcement  
    /// Endpoint:    /announcements/{announcementId}
    /// 
    /// This method sends the influencer identifier as a JSON payload to the REST Web API,
    /// which handles validation and business logic related to influencer applications.
    /// Business rule violations are communicated using HTTP 400 (Bad Request) and handled
    /// explicitly to allow user-friendly feedback in the presentation layer.
    /// </remarks>
    public bool AddInfluencerApplication(int announcementId, int influencerUserId)
    {
        // Creates a rest request representing a HTTP request
        RestRequest? restHttpRequest = new RestRequest($"announcements/{announcementId}", Method.Post);

        // RestSharp serializes the c# object to a JSON format and adds it to the body in the HTTP request, and when
        // the request is sent the API automatically deserializes the JSON back to a C# object
        restHttpRequest.AddJsonBody(new { InfluencerUserId = influencerUserId });

        // Sends the HTTP request to the Rest Web API and receives a HTTP response which contains:
        // a HTTP status code, whether the call was a success, potential exception info and the 
        // deserialized C# object that restsharp has already mapped for us under the hood
        RestResponse<bool> restHttpResponse = _restClient.Execute<bool>(restHttpRequest);

        // If the REST Web API returns a Bad Request this would typically be from our InvalidOperationException business logic exception
        if (restHttpResponse.StatusCode == HttpStatusCode.BadRequest)
        {
            // Creates a string variable to hold the message that should be displayed in the MVC view as a notification
            string exceptionResponseMessage;

            if (string.IsNullOrWhiteSpace(restHttpResponse.Content))
            {
                exceptionResponseMessage = "Unable to receive the request from the server.";
            }

            else
            {
                // Assigns the text from our InvalidOperationException from the REST Web Api 
                exceptionResponseMessage = restHttpResponse.Content;
            }

            throw new InvalidOperationException(exceptionResponseMessage);
        }

        // If the response's HTTP status code is not in the 200 series, no network errors and no transport level errors occured then execute this section
        if (!restHttpResponse.IsSuccessful)
        {
            throw new Exception($"The API returned with the following error. Status: {(int)restHttpResponse.StatusCode} - {restHttpResponse.StatusDescription}. Body: {restHttpResponse.Content}");
        }

        return restHttpResponse.Data;
    }
    #endregion



    #region Update announcement
    /// <summary>
    /// Updates an existing announcement in the system.
    /// </summary>
    /// 
    /// <param name="announcement">
    /// The <see cref="Announcement"/> object containing the updated data.
    /// </param>
    /// 
    /// <returns>
    /// Returns <c>true</c> if the announcement was successfully updated; otherwise <c>false</c>.
    /// </returns>
    /// 
    /// <exception cref="ArgumentNullException">
    /// Thrown if the supplied <paramref name="announcement"/> is <c>null</c>.
    /// </exception>
    /// 
    /// <exception cref="InvalidOperationException">
    /// Thrown if a concurrency conflict occurs (HTTP 409 Conflict), typically because the announcement
    /// was modified by another user since it was last retrieved.
    /// </exception>
    /// 
    /// <exception cref="Exception">
    /// Thrown if the REST Web API returns an unsuccessful HTTP status code (non-2xx),
    /// indicating a server-side or transport-level error.
    /// </exception>
    /// 
    /// <remarks>
    /// Method:      PUT  
    /// Controller:  Announcement  
    /// Endpoint:    /announcements/{announcementId}
    /// 
    /// If the API detects that the announcement has been modified by another user, a 409 (Conflict) status code is returned and handled explicitly.
    /// The announcement data is mapped to an <see cref="AnnouncementUpdateDto"/> before being sent to the API
    /// to ensure only updatable fields are transferred.
    /// </remarks>
    public bool Update(Announcement announcement)
    {
        // Validates that the announcement object is not null
        if (announcement == null)
        {
            // Throws a null pointer exception if the announcement object is null
            throw new ArgumentNullException(nameof(announcement));
        }

        // Maps the Announcement object to an AnnouncementUpdateDto object
        AnnouncementUpdateDto announcementUpdateDto = Map(announcement);

        // Creates a rest request representing a HTTP request
        RestRequest restHttpRequest = new RestRequest($"announcements/{announcement.AnnouncementId}", Method.Put);

        // Sends the mapped Announcement object as JSON format in the request body
        restHttpRequest.AddJsonBody(announcementUpdateDto);

        // Sends the HTTP request to the Rest Web API and receives a HTTP response which contains:
        // a HTTP status code, whether the call was a success, potential exception info and the 
        // deserialized C# object that restsharp has already mapped for us under the hood
        RestResponse<bool> restHttpResponse = _restClient.Execute<bool>(restHttpRequest);

        // In case of an expected concurrency conflict
        if (restHttpResponse.StatusCode == HttpStatusCode.Conflict)
        {
            string message;

            if (string.IsNullOrWhiteSpace(restHttpResponse.Content))
            {
                // Changes the messages's content to a user friendly error that can be shown in the MVC
                message = "The announcement was modified by another user. Please reload and try again.";
            }

            else
            {
                message = restHttpResponse.Content;
            }

            throw new InvalidOperationException(message);
        }

        // If the response's HTTP status code is not in the 200 series, no network errors and no transport level errors occured then execute this section
        if (!restHttpResponse.IsSuccessful)
        {
            throw new Exception($"The API returned with the following error. Status: {(int)restHttpResponse.StatusCode} - {restHttpResponse.StatusDescription}. Body: {restHttpResponse.Content}");
        }

        return restHttpResponse.Data;
    }
    #endregion



    #region Delete Announcement
    /// <summary>
    /// Deletes an existing announcement by its identifier.
    /// </summary>
    /// 
    /// <remarks>
    /// Method:      DELETE  
    /// Controller:  Announcement  
    /// Endpoint:    /announcements/{announcementId}
    ///
    /// Sends a HTTP DELETE request to the REST Web API in order to remove an announcement.
    /// The method interprets the returned HTTP status code to determine whether the deletion
    /// was successful, failed, or if the resource did not exist.
    /// </remarks>
    /// 
    /// <param name="announcementId">
    /// The unique identifier of the announcement to be deleted.
    /// </param>
    /// 
    /// <returns>
    /// Returns <c>true</c> if the announcement was successfully deleted (HTTP 200 or 204).
    /// Returns <c>false</c> if the announcement was not found (HTTP 404).
    /// Throws an exception for all other non-successful HTTP responses.
    /// </returns>
    /// 
    /// <exception cref="Exception">
    /// Thrown when the API returns a non-successful HTTP response other than 404 (Not Found),
    /// such as 400, 401, 403, 409 or 500-series errors.
    /// </exception>
    public bool Delete(int announcementId)
    {
        // Creates a rest request representing a HTTP request
        RestRequest restHttpRequest = new RestRequest($"announcements/{announcementId}", Method.Delete);

        // Sends the HTTP request to the Rest Web API and receives a HTTP response which contains:
        // a HTTP status code, whether the call was a success, potential exception info and the 
        // deserialized C# object that restsharp has already mapped for us under the hood
        RestResponse<bool> restHttpResponse = _restClient.Execute<bool>(restHttpRequest);

        // If the response's HTTP status is the 404 (Not Found) indicating a null return then execute this section
        if (restHttpResponse.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        // If the response's HTTP status code is not in the 200 series, no network errors and no transport level errors occured then execute this section
        if (!restHttpResponse.IsSuccessful)
        {
            throw new Exception($"The API returned with the following error. Status: {(int)restHttpResponse.StatusCode} - {restHttpResponse.StatusDescription}. Body: {restHttpResponse.Content}");
        }

        // If the response's HTTP status code is 200 (OK), the announcement was successfully deleted
        if (restHttpResponse.StatusCode == HttpStatusCode.OK)
        {
            return true;
        }

        // If the response's HTTP status code is 204 (No Content), the announcement was successfully deleted it is just that no body content was returned
        if (restHttpResponse.StatusCode == HttpStatusCode.NoContent)
        {
            return true;
        }

        // Any other successful status code is treated as a failure
        return false;
    }
    #endregion



    #region Helper method
    /// <summary>
    /// Maps an AnnouncementUpdateDto object to a new Announcement object.
    /// The parameter, data transfer object(DTO), containing the updated announcement details.
    /// </summary>
    /// 
    /// <returns>
    /// A new Announcement object populated with the values from the specified <paramref name="dto"/>.</returns>
    private AnnouncementUpdateDto Map(Announcement announcement)
    {
        return new AnnouncementUpdateDto
        {
            AnnouncementId = announcement.AnnouncementId,
            Title = announcement.Title,
            LastEditDateTime = DateTime.UtcNow,
            StartDisplayDateTime = announcement.StartDisplayDateTime,
            EndDisplayDateTime = announcement.EndDisplayDateTime,
            MaximumApplicants = announcement.MaximumApplicants,
            MinimumFollowersRequired = announcement.MinimumFollowersRequired,
            CommunicationType = announcement.CommunicationType,
            AnnouncementLanguage = announcement.AnnouncementLanguage,
            IsKeepProducts = announcement.IsKeepProducts,
            IsPayoutNegotiable = announcement.IsPayoutNegotiable,
            TotalPayoutAmount = announcement.TotalPayoutAmount,
            ShortDescriptionText = announcement.ShortDescriptionText,
            AdditionalInformationText = announcement.AdditionalInformationText,
            StatusType = announcement.StatusType,
            IsVisible = announcement.IsVisible,
            ListOfSubjects = announcement.ListOfSubjects,

            // Converts byte[] to a Base64 string because the AnnouncementUpdateDto.RowVersion is of the string type and Announcement.RowVersion is byte[] type
            // The JSON serialization does not support byte[] directly and therefore needs to be converted to a string format like a Base64 string
            RowVersion = Convert.ToBase64String(announcement.RowVersion)
        };
    }
    #endregion
}
