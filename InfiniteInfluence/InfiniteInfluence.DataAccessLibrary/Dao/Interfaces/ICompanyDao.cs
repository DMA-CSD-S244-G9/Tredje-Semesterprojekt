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
    /// This method creates a company in the database.
    /// </summary>
    /// 
    /// <param name="company">
    /// It takes a Company object as a parameter.
    /// </param>
    /// 
    /// <returns name="int">
    /// It returns an int which is The userId of the created company.
    /// </returns>
    int Create(Company company);

    /// <summery>
    /// This method gets a company by userId.
    /// </summery>>
    /// 
    ///<param name="userId">
    /// It takes an int userId as a parameter.
    ///</param>
    ///
    ///<returns name="Company?">
    /// It returns a Company object if found, otherwise null.
    ///</returns>>
    Company? GetOne(int userId);

    /// TODO: Write comments
    bool Delete(int userId);

}
