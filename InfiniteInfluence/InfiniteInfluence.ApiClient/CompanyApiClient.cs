using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfiniteInfluence.ApiClient;

public class CompanyApiClient : ICompanyDao
{
    #region attributes and constructor
    //The address of the API server
    private readonly string _apiUri;

    //the rest client from restsharp to call the server
    private readonly RestClient _restClient;

    public CompanyApiClient(string apiUri)
    {
        _apiUri = apiUri;
        _restClient = new RestClient(apiUri);
    }
    #endregion

    public int Create(Company company)
    {
        var request = new RestRequest("companys", Method.Post);
        request.AddJsonBody(company);

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
            // throw new Exception("Server reply: Unsuccessful request");
            throw new Exception($"Step 2: Server replied with error. Status: {(int)response.StatusCode} - {response.StatusDescription}. Body: {response.Content}");
        }
        return response.Data;
    }

    public bool Delete(int userId)
    {
        throw new NotImplementedException();
    }

    public Company? GetOne(int userId)
    {
        throw new NotImplementedException();
    }
}
