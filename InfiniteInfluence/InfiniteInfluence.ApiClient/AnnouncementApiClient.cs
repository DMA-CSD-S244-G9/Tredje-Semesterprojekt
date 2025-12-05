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



    // POST
    // ENDPOINT: /announcements/{announcementId}/apply
    // 
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

    public bool Update(Announcement announcement)
    {
        // Validates the input parameters
        if (announcement == null)
        {
            // Throws an exception if the announcement object is null
            throw new ArgumentNullException(nameof(announcement));
        }

        // Maps the Announcement object to an AnnouncementUpdateDto object
        AnnouncementUpdateDto announcementdto = Map(announcement);

        // Creates the REST request to send to the API
        RestRequest? request = new RestRequest($"Api/Announcements/{announcement.AnnouncementId}", Method.Put);

        // Sends the mapped Announcement object as JSON format in the request body
        request.AddJsonBody(announcementdto);


        // Calls upon the API and expects an bool back in the form of an announcementId
        var response = _restClient.Execute<bool>(request);


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

    #region helper method
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
            LastEditDateTime = announcement.LastEditDateTime ?? DateTime.MinValue,
            StartDisplayDateTime = announcement.StartDisplayDateTime ?? DateTime.MinValue,
            EndDisplayDateTime = announcement.EndDisplayDateTime ?? DateTime.MinValue,
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

            // Convert byte[] til Base64 string, cause AnnouncementUpdateDto.RowVersion is string type and Announcement.RowVersion is byte[] type
            // Json serialization does not support byte[] directly and needs to be converted to a string format like Base64
            RowVersion = Convert.ToBase64String(announcement.RowVersion)
        };
    }
    #endregion
}
