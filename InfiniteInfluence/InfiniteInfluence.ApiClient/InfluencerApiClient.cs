using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using System.Net;
using RestSharp;


namespace InfiniteInfluence.ApiClient;


public class InfluencerApiClient : IInfluencerDao
{
    #region attributes and constructor
    // The address of the REST Web API server's URI e.g. "https://localhost:7777"
    private readonly string _apiUri;

    // The rest client from restsharp to call the server the RestClient is threadsafe and intended to be reused for multiple requests
    private readonly RestClient _restClient;


    /// <summary> 
    /// Initializes a new instance of the <see cref="InfluencerApiClient"/> class with the specified API URI
    /// utilizing dependency injection from Website project's Program.cs.
    /// </summary>>
    public InfluencerApiClient(string apiUri)
    {
        _apiUri = apiUri;
        _restClient = new RestClient(apiUri);
    }
    #endregion



    /// <summary>
    /// Sends a request to the REST Web API to create a new influencer.
    /// </summary>
    /// 
    /// <remarks>
    /// Method:      POST  
    /// Controller:  Influencer  
    /// Endpoint:    /influencers
    ///
    /// This method serializes the supplied <see cref="Influencer"/> object to JSON and sends it
    /// as the body of a HTTP POST request to the REST Web API.
    /// The API is expected to create a new influencer and return the generated influencer ID
    /// along with the HTTP status code 201 (Created).
    /// </remarks>
    /// 
    /// <param name="influencer">
    /// The <see cref="Influencer"/> object containing the data required to create a new influencer.
    /// </param>
    /// 
    /// <returns>
    /// The unique identifier (ID) of the newly created influencer.
    /// </returns>
    /// 
    /// <exception cref="Exception">
    /// Thrown if the API returns a non-success HTTP status code, or if the expected
    /// 201 (Created) status code is not returned.
    /// </exception>
    public int Create(Influencer influencer)
    {
        // Creates a rest request representing a HTTP request
        RestRequest? restHttpRequest = new RestRequest("influencers", Method.Post);

        // RestSharp serializes the c# object to a JSON format and adds it to the body in the HTTP request, and when
        // the request is sent the API automatically deserializes the JSON back to a C# object
        restHttpRequest.AddJsonBody(influencer);

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
        int generatedInfluencerUserId = restHttpResponse.Data;

        return generatedInfluencerUserId;
    }



    /// <summary>
    /// Retrieves a single influencer from the REST Web API by its unique identifier.
    /// </summary>
    /// 
    /// <remarks>
    /// Method:      GET  
    /// Controller:  Influencer  
    /// Endpoint:    /influencers/{userId}
    ///
    /// This method sends a HTTP GET request to the REST Web API to retrieve an influencer.
    /// The API is expected to return the influencer as a JSON object, which RestSharp
    /// automatically deserializes into a <see cref="Influencer"/> instance.
    /// </remarks>
    /// 
    /// <param name="userId">
    /// The unique identifier of the influencer to retrieve.
    /// </param>
    /// 
    /// <returns>
    /// A <see cref="Influencer"/> instance if found; otherwise <c>null</c>.
    /// </returns>
    /// 
    /// <exception cref="Exception">
    /// Thrown if the API returns a 404 (Not Found) status code or any other non-success
    /// HTTP response.
    /// </exception>
    public Influencer? GetOne(int userId)
    {
        // Creates a rest request representing a HTTP request
        RestRequest? restHttpRequest = new RestRequest($"influencers/{userId}", Method.Get);

        // Sends the HTTP request to the Rest Web API and receives a HTTP response which contains:
        // a HTTP status code, whether the call was a success, potential exception info and the 
        // deserialized C# object that restsharp has already mapped for us under the hood
        RestResponse<Influencer> restHttpResponse = _restClient.Execute<Influencer>(restHttpRequest);

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
        Influencer? foundInfluencer = restHttpResponse.Data;

        return foundInfluencer;
    }
}