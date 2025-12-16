using InfiniteInfluence.DataAccessLibrary.Model;


namespace InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;


public interface IInfluencerDao
{
    /// <summary>
    /// Creates an influencer and return an userId as int and return an int Id
    /// </summary>
    int Create(Influencer influencer);

    /// <summery>
    /// Gets an influencer by userId and return a influencer object.
    /// </summery>>
    Influencer? GetOne(int userId);
}
