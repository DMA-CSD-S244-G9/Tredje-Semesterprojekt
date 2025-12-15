using InfiniteInfluence.DataAccessLibrary.Model;


namespace InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;


public interface IInfluencerDao
{
    int Create(Influencer influencer);
    Influencer? GetOne(int userId);
}
