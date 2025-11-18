using InfiniteInfluence.DataAccessLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    /// TODO: Write comments
    Company? GetOne(int userId);

    /// TODO: Write comments
    bool Delete(int userId);

}
