using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebUser.SRV.ModelsDTO;
using WebUser.SRV.Response;
using WebUser.SRV.Interfaces;
using WebUser.SRV.Models;
using System.Linq;
using Azure;

namespace WebUser.SRV.Services
{
	public class EmployeeTimeService : IEmployeeTimeService
    {
        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;

        public EmployeeTimeService(MyDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<TResponse<IEnumerable<EmployeeTimeDTO>>> AddTimeRecordsAsync(IEnumerable<EmployeeTimeDTO> timeRecords)
        {
            try
            {
                // Mapeo de DTO a entidad
                var employeeTimes = timeRecords.Select(record => new EmployeeTime
                {
                    EmployeeId = record.EmployeeId,
                    Date = record.Date,
                    EntryTime = record.EntryTime,
                    ExitTime = record.ExitTime,
                    OvertimeHours = record.OvertimeHours,
                    SickLeaveHours = record.SickLeaveHours,
                    VacationHours = record.VacationHours,
                    HolidayHours = record.HolidayHours,
                    OtherHours = record.OtherHours
                });

                // Agregar los registros al DbContext
                await _dbContext.EmployeeTimes.AddRangeAsync(employeeTimes);

                // Guardar cambios en la base de datos
                await _dbContext.SaveChangesAsync();

                // Mapear las entidades guardadas de vuelta a DTOs
                var savedRecordsDTO = employeeTimes.Select(et => new EmployeeTimeDTO
                {
                    EmployeeId = et.EmployeeId,
                    Date = et.Date,
                    EntryTime = et.EntryTime,
                    ExitTime = et.ExitTime,
                    OvertimeHours = et.OvertimeHours,
                    SickLeaveHours = et.SickLeaveHours,
                    VacationHours = et.VacationHours,
                    HolidayHours = et.HolidayHours,
                    OtherHours = et.OtherHours
                }).ToList();

                return TResponse<IEnumerable<EmployeeTimeDTO>>.Create(true, savedRecordsDTO, "Records added successfully.");
            }
            catch (Exception ex)
            {
                return TResponse<IEnumerable<EmployeeTimeDTO>>.Create(false, null, $"Error: {ex.Message}");
            }
        }



        public async Task<TResponse<IEnumerable<EmployeeTimeDTO>>> GetEmployeeTimesAsync()
        {
            try
            {
                var times = await _dbContext.EmployeeTimes
                    .Include(et => et.Employee)
                    .ToListAsync();

                var timesDTO = times.Select(time => _mapper.Map<EmployeeTimeDTO>(time)).ToList();

                return TResponse<IEnumerable<EmployeeTimeDTO>>.Create(true, timesDTO, "Employee times retrieved successfully.");
            }
            catch (Exception ex)
            {
                return TResponse<IEnumerable<EmployeeTimeDTO>>.Create(false, null, $"Error: {ex.Message}");
            }
        }

        public async Task<TResponse<EmployeeTimeDTO>> GetEmployeeTimeByIdAsync(int employeeTimeId)
        {
            try
            {
                var time = await _dbContext.EmployeeTimes
                    .Include(et => et.Employee)
                    .FirstOrDefaultAsync(et => et.EmployeeTimeId == employeeTimeId);

                if (time == null)
                    return TResponse<EmployeeTimeDTO>.Create(false, null, "Employee time not found.");

                var timeDTO = _mapper.Map<EmployeeTimeDTO>(time);
                return TResponse<EmployeeTimeDTO>.Create(true, timeDTO, "Employee time retrieved successfully.");
            }
            catch (Exception ex)
            {
                return TResponse<EmployeeTimeDTO>.Create(false, null, $"Error: {ex.Message}");
            }
        }

        public async Task<TResponse<EmployeeTimeDTO>> CreateEmployeeTimeAsync(EmployeeTimeDTO employeeTimeDTO)
        {
            try
            {
                var newEmployeeTime = _mapper.Map<EmployeeTime>(employeeTimeDTO);

                _dbContext.EmployeeTimes.Add(newEmployeeTime);
                await _dbContext.SaveChangesAsync();

                var createdTimeDTO = _mapper.Map<EmployeeTimeDTO>(newEmployeeTime);
                return TResponse<EmployeeTimeDTO>.Create(true, createdTimeDTO, "Employee time created successfully.");
            }
            catch (Exception ex)
            {
                return TResponse<EmployeeTimeDTO>.Create(false, null, $"Error: {ex.Message}");
            }
        }

        public async Task<TResponse<EmployeeTimeDTO>> UpdateEmployeeTimeAsync(EmployeeTimeDTO employeeTimeDTO)
        {
            try
            {
                var existingTime = await _dbContext.EmployeeTimes.FindAsync(employeeTimeDTO.EmployeeTimeId);
                if (existingTime == null)
                    return TResponse<EmployeeTimeDTO>.Create(false, null, "Employee time not found.");

                _mapper.Map(employeeTimeDTO, existingTime);

                _dbContext.EmployeeTimes.Update(existingTime);
                await _dbContext.SaveChangesAsync();

                var updatedTimeDTO = _mapper.Map<EmployeeTimeDTO>(existingTime);
                return TResponse<EmployeeTimeDTO>.Create(true, updatedTimeDTO, "Employee time updated successfully.");
            }
            catch (Exception ex)
            {
                return TResponse<EmployeeTimeDTO>.Create(false, null, $"Error: {ex.Message}");
            }
        }

        public async Task<TResponse<string>> DeleteEmployeeTimeAsync(int employeeTimeId)
        {
            try
            {
                var time = await _dbContext.EmployeeTimes.FindAsync(employeeTimeId);
                if (time == null)
                    return TResponse<string>.Create(false, null, "Employee time not found.");

                _dbContext.EmployeeTimes.Remove(time);
                await _dbContext.SaveChangesAsync();

                return TResponse<string>.Create(true, null, "Employee time deleted successfully.");
            }
            catch (Exception ex)
            {
                return TResponse<string>.Create(false, null, $"Error: {ex.Message}");
            }
        }
    }
}

