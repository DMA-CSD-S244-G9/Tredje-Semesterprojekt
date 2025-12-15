using Microsoft.Data.SqlClient;
using System.Data;


namespace InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;


/// <summary>
/// The class is marked as abstract, to make it impossible to instantiate, and
/// only allow other classes e.g. InfluencerDao and CompanyDao to inherit from it,
/// the class contains the common functionality for establishing connection to the
/// database.
/// 
/// The class stores the connection string and provides a method for creating 
/// new database connections.
/// 
/// 
/// NOTE: The class relies on middleware in the form of a NuGet package
/// - Microsoft.Data.SqlClient
/// </summary>
public abstract class BaseConnectionDao
{
    /// <summary>
    /// The database connection string used to create connections.
    /// Publicly readable but can only be set internally.
    /// </summary>
    public string DataBaseConnectionString { get; private set; }


    /// <summary>
    /// The constructor used to initializes a new instance of the 
    /// <see cref="BaseConnectionDao"/> class using the specified connection string.
    /// </summary>
    /// <param name="connectionString">
    /// The connection string to be used for establishing database connections.
    /// </param>
    protected BaseConnectionDao(string connectionString)
    {
        DataBaseConnectionString = connectionString;
    }


    /// <summary>
    /// Creates and returns a new database connection based on the stored connection string.
    /// </summary>
    /// <returns>
    /// A new <see cref="SqlConnection"/> instance as an <see cref="IDbConnection"/>.
    /// </returns>
    public IDbConnection CreateConnection()
    {
        IDbConnection sqlDataBaseConnection = new SqlConnection(DataBaseConnectionString);

        return sqlDataBaseConnection;
    }
}