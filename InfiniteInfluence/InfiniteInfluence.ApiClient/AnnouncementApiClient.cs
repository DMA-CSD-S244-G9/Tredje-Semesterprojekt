using InfiniteInfluence.API.Dtos;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;
using InfiniteInfluence.DataAccessLibrary.Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace InfiniteInfluence.ApiClient;


public class AnnouncementApiClient : IAnnouncementDao
{
    // The rest client from restsharp to call the server
    private readonly RestClient _restClient;
    private readonly string _apiUri;


    // Utilises Dependency injection from the Website project's Program.cs 
    public AnnouncementApiClient(string apiUri)
    {
        _apiUri = apiUri;
        _restClient = new RestClient(apiUri);
    }

    #region Create announcement
    public int Create(Announcement announcement)
    {
        RestRequest? request = new RestRequest("announcements", Method.Post);

        // Sends the Announcement object as JSON format in the request body
        request.AddJsonBody(announcement);

        // Calls upon the API and expects an integer back in the form of an announcementId
        var response = _restClient.Execute<int>(request);


        if (response == null)
        {
            throw new Exception("Connection Failure: There were no response from the server.");
        }

        if (!response.IsSuccessful)
        {
            throw new Exception($"Step 1: Server replied with error. Status: {(int)response.StatusCode} - {response.StatusDescription}. Body: {response.Content}");
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Step 2: Server replied with error. Status: {(int)response.StatusCode} - {response.StatusDescription}. Body: {response.Content}");
        }

        return response.Data;
    }
    #endregion


    #region Get all announcements
    public IEnumerable<Announcement> GetAll()
    {
        RestRequest? request = new RestRequest("announcements", Method.Get);

        // Calls upon the API and expects a list of Announcement objects back
        var response = _restClient.Execute<List<Announcement>>(request);


        if (response == null)
        {
            throw new Exception("Connection Failure: There were no response from the server.");
        }

        if (!response.IsSuccessful)
        {
            throw new Exception($"Step 1: Server replied with error. Status: {(int)response.StatusCode} - {response.StatusDescription}. Body: {response.Content}");
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Step 2: Server replied with error. Status: {(int)response.StatusCode} - {response.StatusDescription}. Body: {response.Content}");
        }

        // Since the response data can be null then we return an empty list in that case
        if (response.Data == null)
        {
            return new List<Announcement>();
        }

        return response.Data;
    }
    #endregion


    #region Get one announcement by announcementid
    public Announcement? GetOne(int announcementId)
    {
        RestRequest? request = new RestRequest($"announcements/{announcementId}", Method.Get);

        var response = _restClient.Execute<Announcement>(request);

        if (response == null)
        {
            throw new Exception("Connection Failure: There were no response from the server.");
        }

        if (!response.IsSuccessful)
        {
            throw new Exception($"Step 1: Server replied with error. Status: {(int)response.StatusCode} - {response.StatusDescription}. Body: {response.Content}");
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Step 2: Server replied with error. Status: {(int)response.StatusCode} - {response.StatusDescription}. Body: {response.Content}");
        }

        return response.Data;
    }
    #endregion


    #region Add influencer application to announcement
    // POST
    // ENDPOINT: /announcements/{announcementId}/apply
    public bool AddInfluencerApplication(int announcementId, int influencerUserId)
    {
        RestRequest? request = new RestRequest($"announcements/{announcementId}/apply", Method.Post);

        request.AddJsonBody(new
        {
            InfluencerUserId = influencerUserId
        });


        RestResponse<bool> response = _restClient.Execute<bool>(request);

        if (response == null)
        {
            throw new Exception("Connection Failure: There were no response from the server.");
        }

        // If the REST Web API returns a Bad Request this would typically be from our InvalidOperationException business logic exception
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            // Creates a string variable to hold the message that should be displayed in the MVC view as a notification
            string exceptionResponseMessage;

            if (string.IsNullOrWhiteSpace(response.Content))
            {
                exceptionResponseMessage = "Unable to receive the request from the server.";
            }
            else
            {
                // Assigns the text from our InvalidOperationException from the REST Web Api 
                exceptionResponseMessage = response.Content;
            }

            throw new InvalidOperationException(exceptionResponseMessage);
        }


        if (!response.IsSuccessful)
        {
            throw new Exception($"Step 1: Server replied with error. Status: {(int)response.StatusCode} - {response.StatusDescription}. Body: {response.Content}");
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Step 2: Server replied with error. Status: {(int)response.StatusCode} - {response.StatusDescription}. Body: {response.Content}");
        }

        return response.Data;
    }
    #endregion


    #region Update announcement
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

        // Creates the REST request to send to the API
        RestRequest request = new RestRequest($"announcements/{announcement.AnnouncementId}", Method.Put);

        // Sends the mapped Announcement object as JSON format in the request body
        request.AddJsonBody(announcementUpdateDto);

        // Calls upon the API and expects a bool indicating true if the announcement with the announcementId was updated elsewise false
        RestResponse<bool> response = _restClient.Execute<bool>(request);

        if (response == null)
        {
            throw new Exception("Connection Failure: There were no response from the server.");
        }

        // In case of an expected concurrency conflict
        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            string message;

            if (string.IsNullOrWhiteSpace(response.Content))
            {
                // Changes the messages's content to a user friendly error that can be shown in the MVC
                message = "The announcement was modified by another user. Please reload and try again.";
            }

            else
            {
                message = response.Content;
            }

            throw new InvalidOperationException(message);
        }

        if (!response.IsSuccessful || !response.IsSuccessStatusCode)
        {
            throw new Exception($"Server replied with error. Status: {(int)response.StatusCode} - {response.StatusDescription}. Body: {response.Content}");
        }

        return response.Data;
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
