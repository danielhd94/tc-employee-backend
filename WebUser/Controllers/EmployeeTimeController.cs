using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebUser.SRV.Interfaces;
using WebUser.SRV.ModelsDTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;
using Newtonsoft.Json;

namespace WebUser.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class EmployeeTimeController : ControllerBase
    {
        private readonly IEmployeeTimeService _employeeTimeService;

        public EmployeeTimeController(IEmployeeTimeService employeeTimeService)
        {
            _employeeTimeService = employeeTimeService;
        }

        // GET: api/EmployeeTimes
        [HttpGet]
        public async Task<IActionResult> GetEmployeeTimes()
        {
            var response = await _employeeTimeService.GetEmployeeTimesAsync();
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        // GET: api/EmployeeTimes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeTime(int id)
        {
            var response = await _employeeTimeService.GetEmployeeTimeByIdAsync(id);
            if (response.Success)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        // POST: api/EmployeeTimes
        [HttpPost]
        public async Task<IActionResult> CreateEmployeeTime([FromBody] EmployeeTimeDTO employeeTimeDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _employeeTimeService.CreateEmployeeTimeAsync(employeeTimeDTO);
            if (response.Success)
            {
                return CreatedAtAction(nameof(GetEmployeeTime), new { id = response.Data.EmployeeTimeId }, response);
            }
            return BadRequest(response);
        }

        // PUT: api/EmployeeTimes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployeeTime(int id, [FromBody] EmployeeTimeDTO employeeTimeDTO)
        {
            if (id != employeeTimeDTO.EmployeeTimeId)
            {
                return BadRequest("ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _employeeTimeService.UpdateEmployeeTimeAsync(employeeTimeDTO);
            if (response.Success)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        // DELETE: api/EmployeeTimes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeTime(int id)
        {
            var response = await _employeeTimeService.DeleteEmployeeTimeAsync(id);
            if (response.Success)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpPost("time-records")]
        public async Task<IActionResult> PostTimeRecords([FromBody] Dictionary<string, JsonElement> timeData)
        {
            var records = new List<EmployeeTimeDTO>();

            foreach (var date in timeData.Keys)
            {
                var employeeData = timeData[date];

                foreach (var employeeId in employeeData.EnumerateObject())
                {
                    
                    // Deserializa directamente el valor a EmployeeTimeDTO
                    var record = JsonConvert.DeserializeObject<EmployeeTimeDTO>(employeeId.Value.GetRawText());
                    if (record != null)
                    {
                        record.Date = DateTime.Parse(date);
                        record.EmployeeId = int.Parse(employeeId.Name);
                        records.Add(record);
                    }
                }
            }

            var response = await _employeeTimeService.AddTimeRecordsAsync(records);

            if (response.Success)
            {
                return Ok(response);
            }
            return NotFound(response);
        }



    }
}

