using InfiniteInfluence.DataAccessLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;

public interface ICompanyDao
{
    int CreateCompany(Company company);

    Company? GetOneCompany(int userId);

    bool DeleteCompany(int userId);

}
