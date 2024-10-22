using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebUser.SRV.Interfaces;
using WebUser.SRV.ModelsDTO;

namespace WebUser.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase {

        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            this._employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _employeeService.GetEmployeesAsync();

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetEmployeeById(int employeeId)
        {
            var response = await _employeeService.GetEmployeeByIdAsync(employeeId);

            if (response.Success)
            {
                return Ok(response.Data);
            }

            return NotFound(response.Message);
        }

        [HttpGet("with-times")]
        public async Task<IActionResult> GetEmployeesWithTimes()
        {
            var response = await _employeeService.GetEmployeesWithTimesAsync();

            if (response.Success)
            {
                return Ok(response);
            }

            return StatusCode(500, response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(EmployeeDTO emp)
        {
            var result = await _employeeService.CreateEmployeeAsync(emp);

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] EmployeeDTO emp)
        {
            var result = await _employeeService.UpdateEmployeeAsync(emp);

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _employeeService.DeleteEmployeeAsync(id);

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [Route("SaveFile")]
        [HttpPost]
        public async Task<IActionResult> SaveFile(IFormFile file)
        {
            var result = await _employeeService.SaveFile(file);

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpGet("gender-count")]
        public async Task<IActionResult> GetGenderCount()
        {
            var result = await _employeeService.GetGenderCountAsync();

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

    }
}

