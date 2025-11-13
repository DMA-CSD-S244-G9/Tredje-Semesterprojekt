using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;
using InfiniteInfluence.DataAccessLibrary.Model;

namespace InfiniteInfluence.Tests;


public class InfluencerDaoTests
{
    private const string _dataBaseConnectionString = "Data Source=.;Integrated Security=True;initial catalog=BlogSharp2025; trust server certificate=true";

    
    private IInfluencerDao CreateDao()
    {
        return new InfluencerDao(_dataBaseConnectionString);
    }


    [SetUp]
    public void Setup()
    {
        
    }


    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}