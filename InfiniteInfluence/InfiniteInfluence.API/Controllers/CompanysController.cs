using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;
using InfiniteInfluence.DataAccessLibrary.Model;
using Microsoft.AspNetCore.Mvc;
using System;

namespace InfiniteInfluence.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanysController : Controller
    {
        private readonly ILogger<CompanysController> _logger;
        ICompanyDao _companyDao;


        // This constructor might appear to have no references, but are called internally by the framework
        public CompanysController(ILogger<CompanysController> logger, ICompanyDao companyDao)
        {
            _logger = logger;
            _companyDao = companyDao;
        }



        [HttpPost]
        public ActionResult<int> Create(Company company)
        {
            try
            {
                //TODO: Validate input
                return Ok(_companyDao.Create(company));
            }

            catch (Exception exception)
            {
                _logger.LogError(exception, "An error has occured when attempting to create a Company.");

                var innerMessage = exception.InnerException?.Message;

                return StatusCode(500, $"Error: {exception.Message} | Inner: {innerMessage}");
            }
        }



        [HttpGet("{userId}")]
        public ActionResult<Company> GetOne(int userId)
        {
            try
            {
                Company? company = _companyDao.GetOne(userId);
                if (company == null)
                {
                    return NoContent();
                }

                return Ok(company);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"An error occurred trying to retrieve the company with user id {userId}.");

                var innerMessage = exception.InnerException?.Message;

                return StatusCode(500, $"Error: {exception.Message} | Inner: {innerMessage}");
            }
        }



        [HttpDelete("{userId}")]
        public ActionResult<bool> Delete(int userId)
        {
            try
            {

                var deleted = _companyDao.Delete(userId);
                if (!deleted)
                {
                    return NoContent();
                }

                return Ok(true);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"An error occurred trying to delete the author with id {userId}.");

                var innerMessage = exception.InnerException?.Message;

                return StatusCode(500, $"Error: {exception.Message} | Inner: {innerMessage}");
            }
        }
    }
}
