using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using RestSharp;


namespace InfiniteInfluence.ApiClient;


/// <summary>
/// Provides methods for interacting with the Company API, including creating, retrieving, and deleting company records.
/// </summary>
/// 
/// <remarks>
/// This client is designed to communicate with a RESTful API server using the specified base URI.  It
/// provides functionality to perform operations such as creating a new company, retrieving a specific company by ID, 
/// and deleting a company. The client uses RestSharp for HTTP communication.
/// </remarks>
public class CompanyApiClient : ICompanyDao
{
    #region attributes and constructor
    //The address of the API server
    //Its a specific URI e.g. "https://localhost:7777"
    private readonly string _apiUri;

    //the rest client from restsharp to call the server
    //RestClient is thread-safe and intended to be reused for multiple requests
    private readonly RestClient _restClient;

    /// <summary> 
    /// Initializes a new instance of the <see cref="CompanyApiClient"/> class with the specified API URI.
    /// </summary>>
    public CompanyApiClient(string apiUri)
    {
        _apiUri = apiUri;
        _restClient = new RestClient(apiUri);
    }
    #endregion



    #region Create Method
    /// <summary>
    /// Creates a new company by sending a POST request to the server.
    /// </summary>
    /// 
    /// <remarks>
    /// This method sends a JSON payload representing the company to the server's "companys"
    /// endpoint. Ensure that the company parameter is properly populated before calling this method.
    /// 
    /// Exception:
    /// Thrown if the server does not respond, or if the server responds with an error. The exception message includes
    /// the HTTP status code, status description, and response body for debugging purposes.
    /// </remarks>
    /// 
    /// <returns>
    /// The unique identifier of the newly created company as an integer.
    /// </returns>
    public int Create(Company company)
    {
        //prepare the request and set the method to POST
        RestRequest request = new RestRequest("companys", Method.Post);

        //add the company object as json body
        request.AddJsonBody(company);

        //execute the request and get the response
        RestResponse<int> response = _restClient.Execute<int>(request);

        //check for errors
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
    #endregion



    #region Delete
    //TODO: Implement the Delete method to remove a company by userId
    // NOTE: Currently this is added because we have delete in the DAO, and since we use the DAO's interface
    // we must implement it here too, although the delete is curerntly just used in a test and not fully integrated.
    public bool Delete(int userId)
    {
        throw new NotImplementedException();
    }
    #endregion



    #region GetOne Methods
    /// <summary>
    /// Retrieves a single Company object associated with the specified user ID.
    /// </summary>
    /// 
    /// <remarks>
    /// This method sends a GET request to the server to retrieve the company data for the specified
    /// user ID. Ensure that the server is reachable and the user ID is valid before calling this method.
    /// 
    /// Exception:
    /// Thrown when the server does not respond, or when the server responds with an error status code.
    /// </remarks>
    /// 
    /// <returns>
    /// Company object associated with the specified user ID, or null if no data is returned.
    /// </returns>
    public Company? GetOne(int userId)
    {
        // Prepare the request and set the method to GET
        RestRequest? request = new RestRequest($"companys/{userId}", Method.Get);

        RestResponse<Company> response = _restClient.Execute<Company>(request);

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
    #endregion
}
