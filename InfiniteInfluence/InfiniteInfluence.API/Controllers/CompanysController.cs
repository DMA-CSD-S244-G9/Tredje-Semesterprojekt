using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using Microsoft.AspNetCore.Mvc;

namespace InfiniteInfluence.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanysController : Controller
    {
        ICompanyDao _companyDao;

        public CompanysController(ICompanyDao companyDao)
        {
            _companyDao = companyDao;
        }

        [HttpPost]
        public ActionResult<int> CreateCompany(Company company)
        {
            try
            {
                return Ok(_companyDao.CreateCompany(company));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred trying to create the company.");
            }
        }

        [HttpGet("{userId}")]
        public ActionResult<Company> Get(int userId)
        {
            try
            {
                Company? company = _companyDao.GetOneCompany(userId);
                if (company == null)
                {
                    return NoContent();
                }

                return Ok(company);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred trying to retrieve the company with user id {userId}.");
            }
        }

        [HttpDelete("{userId}")]
        public ActionResult<bool> DeleteCompany(int userId)
        {
            try
            {

                var deleted = _companyDao.DeleteCompany(userId);
                if (!deleted)
                {
                    return NoContent();
                }

                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred trying to delete the author with id {userId}.");
            }
        }
    }
}
