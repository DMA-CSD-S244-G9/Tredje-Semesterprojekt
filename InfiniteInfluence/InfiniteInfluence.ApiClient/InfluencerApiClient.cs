using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using RestSharp;

namespace InfiniteInfluence.ApiClient;


public class InfluencerApiClient : IInfluencerDao
{
    //the rest client from restsharp to call the server
    private readonly RestClient _restClient;

    //The address of the API server
    private readonly string _apiUri = "https://localhost:7249/";


    //TODO: This should have the API url inside the constructor so it can be used from elsewhere later on
    public InfluencerApiClient()
    {
        _restClient = new RestClient(_apiUri);
    }





    public int Create(Influencer influencer)
    {
        var request = new RestRequest("authors", Method.Post);

        request.AddJsonBody(influencer);

        var response = _restClient.Execute<int>(request);

        if (response == null)
        {
            throw new Exception("Connection: There were no response from the server.");
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Server reply: Unsuccessful request");
        }

        return response.Data;
    }
}
