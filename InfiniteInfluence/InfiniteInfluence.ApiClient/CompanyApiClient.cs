using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using System.Net;
using RestSharp;


namespace InfiniteInfluence.ApiClient;


/// <summary>
/// Provides methods for interacting with the Company API, including creating, retrieving, and /an unimplemented/ deleting company records.
/// </summary>
/// 
/// <remarks>
/// This client is designed to communicate with a RESTful API server using the specified base URI. It
/// provides functionality to perform operations such as creating a new company, retrieving a specific company by ID, 
/// and deleting a company. The client uses RestSharp for HTTP communication.
/// </remarks>
public class CompanyApiClient : ICompanyDao
{
    #region attributes and constructor
    // The address of the REST Web API server's URI e.g. "https://localhost:7777"
    private readonly string _apiUri;

    // The rest client from restsharp to call the server the RestClient is threadsafe and intended to be reused for multiple requests
    private readonly RestClient _restClient;


    /// <summary> 
    /// Initializes a new instance of the <see cref="CompanyApiClient"/> class with the specified API URI
    /// utilizing dependency injection from Website project's Program.cs.
    /// </summary>>
    public CompanyApiClient(string apiUri)
    {
        _apiUri = apiUri;
        _restClient = new RestClient(apiUri);
    }
    #endregion



    #region Create Method
    /// <summary>
    /// Sends a request to the REST Web API to create a new company.
    /// </summary>
    /// 
    /// <remarks>
    /// Method:      POST  
    /// Controller:  Company  
    /// Endpoint:    /companys
    ///
    /// This method serializes the supplied <see cref="Company"/> object to JSON and sends it
    /// as the body of a HTTP POST request to the REST Web API.
    /// The API is expected to create a new company and return the generated company user ID
    /// along with the HTTP status code 201 (Created).
    /// </remarks>
    /// 
    /// <param name="company">
    /// The <see cref="Company"/> object containing the data required to create a new company.
    /// </param>
    /// 
    /// <returns>
    /// The unique identifier (user ID) of the newly created company.
    /// </returns>
    /// 
    /// <exception cref="Exception">
    /// Thrown if the API returns a non-success HTTP status code, or if the expected
    /// 201 (Created) status code is not returned.
    /// </exception>
    public int Create(Company company)
    {
        // Creates a rest request representing a HTTP request
        RestRequest restHttpRequest = new RestRequest("companys", Method.Post);

        // RestSharp serializes the c# object to a JSON format and adds it to the body in the HTTP request, and when
        // the request is sent the API automatically deserializes the JSON back to a C# object
        restHttpRequest.AddJsonBody(company);

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
        int generatedCompanyUserId = restHttpResponse.Data;

        return generatedCompanyUserId;
    }
    #endregion



    #region GetOne Methods
    /// <summary>
    /// Retrieves a single company from the REST Web API by its unique identifier.
    /// </summary>
    /// 
    /// <remarks>
    /// Method:      GET  
    /// Controller:  Company  
    /// Endpoint:    /companys/{userId}
    ///
    /// This method sends a HTTP GET request to the REST Web API to retrieve a company.
    /// The API is expected to return the company as a JSON object, which RestSharp
    /// automatically deserializes into a <see cref="Company"/> instance.
    /// </remarks>
    /// 
    /// <param name="userId">
    /// The unique identifier of the company to retrieve.
    /// </param>
    /// 
    /// <returns>
    /// A <see cref="Company"/> instance if found; otherwise <c>null</c>.
    /// </returns>
    /// 
    /// <exception cref="Exception">
    /// Thrown if the API returns a 404 (Not Found) status code or any other non-success
    /// HTTP response.
    /// </exception>
    public Company? GetOne(int userId)
    {
        // Creates a rest request representing a HTTP request
        RestRequest? restHttpRequest = new RestRequest($"companys/{userId}", Method.Get);

        // Sends the HTTP request to the Rest Web API and receives a HTTP response which contains:
        // a HTTP status code, whether the call was a success, potential exception info and the 
        // deserialized C# object that restsharp has already mapped for us under the hood
        RestResponse<Company> restHttpResponse = _restClient.Execute<Company>(restHttpRequest);

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
        Company? foundCompany = restHttpResponse.Data;

        return foundCompany;
    }
    #endregion



    #region Delete
    // TODO: Implement the Delete method to remove a company by userId
    // NOTE: Currently this is added because we have delete in the DAO, and since we use the DAO's interface
    // we must implement it here too, although the delete is curerntly just used in a test and not fully integrated.
    public bool Delete(int userId)
    {
        throw new NotImplementedException();
    }
    #endregion
}
