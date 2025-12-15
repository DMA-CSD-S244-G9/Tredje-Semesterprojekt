using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using RestSharp;


namespace InfiniteInfluence.ApiClient;


public class InfluencerApiClient : IInfluencerDao
{
    // The rest client from restsharp to call the server
    private readonly RestClient _restClient;
    private readonly string _apiUri;


    // Utilises Dependency injection from the Website project's Program.cs 
    public InfluencerApiClient(string apiUri)
    {
        _apiUri = apiUri;
        _restClient = new RestClient(_apiUri);
    }



    //TODO: Make this code prettier during refactoring
    public int Create(Influencer influencer)
    {
        RestRequest? request = new RestRequest("influencers", Method.Post);

        // Sends the Announcement object as JSON format in the request body
        request.AddJsonBody(influencer);

        // Calls upon the API and expects an integer back in the form of a UserId
        RestResponse<int> response = _restClient.Execute<int>(request);

        if (response == null)
        {
            throw new Exception("Connection Failure: There were no response from the server.");
        }

        if (!response.IsSuccessful)
        {
            throw new Exception($"Step 1: Server replied with error. Status: {(int)response.StatusCode} - {response.StatusDescription}. Body: {response.Content}");
        }

        return response.Data;
    }



    public Influencer? GetOne(int userId)
    {
        RestRequest? request = new RestRequest($"influencers/{userId}", Method.Get);

        // Calls upon the API and expects an Influencer object back
        RestResponse<Influencer> response = _restClient.Execute<Influencer>(request);

        if (response == null)
        {
            //No response received from server
            throw new Exception("Connection Failure: There were no response from the server.");
        }

        if (!response.IsSuccessful)
        {
            // Network or connection error returns  - was there http technical error?
            throw new Exception($"Step 1: Server replied with error. Status: {(int)response.StatusCode} - {response.StatusDescription}. Body: {response.Content}");
        }

        return response.Data;
    }
}
