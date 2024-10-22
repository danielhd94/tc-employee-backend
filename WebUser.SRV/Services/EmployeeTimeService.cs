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
                var savedRecordsDTO = new List<EmployeeTimeDTO>();

                foreach (var record in timeRecords)
                {
                    // Buscar si el registro ya existe
                    var existingRecord = await _dbContext.EmployeeTimes
                        .FirstOrDefaultAsync(et => et.EmployeeId == record.EmployeeId && et.Date.Date == record.Date.Date);

                    if (existingRecord != null)
                    {
                        // Si el registro existe, actualizarlo
                        existingRecord.EntryTime = record.EntryTime;
                        existingRecord.ExitTime = record.ExitTime;
                        existingRecord.OvertimeHours = record.OvertimeHours;
                        existingRecord.SickLeaveHours = record.SickLeaveHours;
                        existingRecord.VacationHours = record.VacationHours;
                        existingRecord.HolidayHours = record.HolidayHours;
                        existingRecord.OtherHours = record.OtherHours;

                        // No agregar el registro de nuevo, solo guardamos cambios
                    }
                    else
                    {
                        // Si no existe, crear una nueva entidad
                        var newEmployeeTime = new EmployeeTime
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
                        };

                        // Agregar la nueva entidad
                        await _dbContext.EmployeeTimes.AddAsync(newEmployeeTime);
                        savedRecordsDTO.Add(new EmployeeTimeDTO
                        {
                            EmployeeId = newEmployeeTime.EmployeeId,
                            Date = newEmployeeTime.Date,
                            EntryTime = newEmployeeTime.EntryTime,
                            ExitTime = newEmployeeTime.ExitTime,
                            OvertimeHours = newEmployeeTime.OvertimeHours,
                            SickLeaveHours = newEmployeeTime.SickLeaveHours,
                            VacationHours = newEmployeeTime.VacationHours,
                            HolidayHours = newEmployeeTime.HolidayHours,
                            OtherHours = newEmployeeTime.OtherHours
                        });
                    }
                }

                // Guardar todos los cambios en la base de datos
                await _dbContext.SaveChangesAsync();

                return TResponse<IEnumerable<EmployeeTimeDTO>>.Create(true, savedRecordsDTO, "Records processed successfully.");
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

