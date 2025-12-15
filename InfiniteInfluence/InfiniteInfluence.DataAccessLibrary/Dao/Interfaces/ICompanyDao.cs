using InfiniteInfluence.DataAccessLibrary.Model;


namespace InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;


/// <summary>
/// Defines the contract for data access operations related to companies.
/// </summary>
/// 
/// <remarks>
/// This interface provides methods for creating, retrieving, and deleting company records.
/// Implementations of this interface are responsible for interacting with the underlying data store.
/// </remarks>
public interface ICompanyDao
{
    /// <summary>
    /// Creates a company and return an userId as int.
    /// </summary>
    int Create(Company company);

    /// <summery>
    /// Gets a company by userId and return a company object.
    /// </summery>>
    Company? GetOne(int userId);

    /// TODO: Write comments
    bool Delete(int userId);

}
