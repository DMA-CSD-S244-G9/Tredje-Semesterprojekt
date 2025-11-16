using InfiniteInfluence.DataAccessLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;

public interface ICompanyDao
{
    int Create(Company company);

    Company? GetOne(int userId);

    bool Delete(int userId);

}
