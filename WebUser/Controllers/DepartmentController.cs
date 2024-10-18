using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebUser.SRV.Interfaces;
using WebUser.SRV.ModelsDTO;
using WebUser.SRV.Services;

namespace WebUser.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _departmentService.GetDepartmentAsync();

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpGet("{departmentId}")]
        public async Task<IActionResult> GetById(int departmentId)
        {
            var response = await _departmentService.GetDepartmentByIdAsync(departmentId);

            if (response.Success)
            {
                return Ok(response.Data);
            }

            return NotFound(response.Message);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DepartmentDTO dep)
        {
            var result = await _departmentService.CreateDepartmentAsync(dep);

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] DepartmentDTO dep)
        {
            var result = await _departmentService.UpdateDepartmentAsync(dep);

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _departmentService.DeleteDepartmentAsync(id);

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }
    }
}

