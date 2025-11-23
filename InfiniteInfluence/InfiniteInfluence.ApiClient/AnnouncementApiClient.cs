using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using RestSharp;


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
        var request = new RestRequest($"announcements/{announcementId}", Method.Get);

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
}
