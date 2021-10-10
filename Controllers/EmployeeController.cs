using DEHacker.Businesslogic;
using DEHacker.DataAccess.Model;
using DEHacker.Jwt.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DEHacker.Jwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IConfiguration _configure;
        private readonly IBusinessLayer _businessLayer;

        public EmployeeController(ILogger<EmployeeController> logger, IConfiguration configure, IBusinessLayer businessLayer)
        {
            _logger = logger;
            _configure = configure;
            _businessLayer = businessLayer;
        }
        [HttpGet]
        [Authorize]
        public IEnumerable<EmployeeDetails> Get()
        {
            _logger.LogInformation("Entering into Controller - GetEmployeeDetails method");
            var result = _businessLayer.GetEmployeeDetails();
            _logger.LogInformation("Exiting into Controller - GetEmployeeDetails method");
            return result;
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        [Authorize]
        public EmployeeDetails Get(int id)
        {
            _logger.LogInformation("Entering into Controller - GetEmployeeDetail method");
            var result = _businessLayer.GetEmployeeDetail(id);
            _logger.LogInformation("Exiting into Controller - GetEmployeeDetail method");
            return result;
        }

        // POST api/<ValuesController>
        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody] EmployeeDetails detail)
        {
            _logger.LogInformation("Entering into Controller - SaveEmployeeDetail method");
            if (detail == null)
            {
                return new JsonResult(new { message = "Employee details are required" }) { StatusCode = StatusCodes.Status428PreconditionRequired };
            }
            var result = _businessLayer.SaveEmployeeDetails(detail);
            _logger.LogInformation("Exiting into Controller - SaveEmployeeDetail method");
            return new JsonResult(new { message = result ? "Employee detail saved successfully." : "Employee detail failed to save." }) {  StatusCode = result ? StatusCodes.Status200OK : StatusCodes.Status412PreconditionFailed };
        }
    }
}
