using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    /// <summary>
    /// Creates a new company by sending a POST request to the server.
    /// </summary>
    /// 
    /// <remarks>
    /// This method sends a JSON payload representing the company to the server's "companys"
    /// endpoint. Ensure that the <paramref name="company"/> parameter is properly populated before calling this
    /// method.
    /// </remarks>
    /// 
    /// <param name="company">
    /// The <see cref="Company"/> object containing the details of the company to be created.
    /// </param>
    /// 
    /// <returns>
    /// The unique identifier of the newly created company as an integer.
    /// </returns>
    /// 
    /// <exception cref="Exception">
    /// Thrown if the server does not respond, or if the server responds with an error. The exception message includes
    /// the HTTP status code, status description, and response body for debugging purposes.
    /// </exception>
    public int Create(Company company)
    {
        //prepare the request
        var request = new RestRequest("companys", Method.Post);

        //add the company object as json body
        request.AddJsonBody(company);

        //execute the request and get the response
        var response = _restClient.Execute<int>(request);

        //check for errors
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
            // throw new Exception("Server reply: Unsuccessful request");
            throw new Exception($"Step 2: Server replied with error. Status: {(int)response.StatusCode} - {response.StatusDescription}. Body: {response.Content}");
        }

        return response.Data;
    }


    //TODO: Implement the Delete method to remove a company by userId
    public bool Delete(int userId)
    {
        throw new NotImplementedException();
    }

    //TODO: Implement the GetOne method to retrieve a company by userId
    public Company? GetOne(int userId)
    {
        throw new NotImplementedException();
    }
}
