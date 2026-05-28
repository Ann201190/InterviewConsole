using EmployeeService;
using EmployeeWeb.Models;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace EmployeeWeb.Controllers
{
    [RoutePrefix("api/employee")]
    public class EmployeeController : ApiController
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET  api/employee/GetEmployeeById?id=1
        [HttpGet]
        [Route("GetEmployeeById")]
        public async Task<IHttpActionResult> GetEmployeeByIdAsync(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);

                if (employee == null)
                {
                    return BadRequest(); 
                }
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex); 
            }
        }

        // PUT api/employee/EnableEmployee?id=1
        // Body: { "Enable": 1 }
        [HttpPut]
        [Route("EnableEmployee")]
        public async Task<IHttpActionResult> EnableEmployeeAsync(int id, [FromBody] EnableRequest request)
        {
            try
            {
                if (request == null) return BadRequest("The request body is empty");

                await _employeeService.EnableEmployeeAsync(id, request.Enable);
                return Ok(new { message = "Status updated" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}