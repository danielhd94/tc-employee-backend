using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebUser.SRV.Interfaces;
using WebUser.SRV.ModelsDTO;
using WebUser.SRV.Models;
using WebUser.SRV.Response;
using static System.Collections.Specialized.BitVector32;

namespace WebUser.SRV.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly MyDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private const string FILE_NAME_DEFAULT = "anonymous.png";
        private readonly IMapper _mapper;

        public EmployeeService(IConfiguration configuration, IWebHostEnvironment env, MyDbContext dbContext, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
            _env = env;
            _dbContext = dbContext;
        }

        public async Task<TResponse<IEnumerable<EmployeeDTO>>> GetEmployeesAsync()
        {
            try
            {
                var employees = await _dbContext.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Gender)
                    .ToListAsync();

                var employeeDTOs = employees
                .Select(employee => _mapper.Map<EmployeeDTO>(employee))
                .ToList();

                return TResponse<IEnumerable<EmployeeDTO>>.Create(true, employeeDTOs, "Employees retrieved successfully.");
            }
            catch (Exception ex)
            {
                string detailedMessage = $"Error: {ex.Message}\nStackTrace: {ex.StackTrace}";
                return TResponse<IEnumerable<EmployeeDTO>>.Create(false, null, detailedMessage);
            }
        }

        public async Task<TResponse<EmployeeDTO>> GetEmployeeByIdAsync(int employeeId)
        {
            try
            {
                var employee = await _dbContext.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Gender)
                    .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

                var employeeOne = _mapper.Map<EmployeeDTO>(employee);

                if (employee != null)
                {
                    return TResponse<EmployeeDTO>.Create(true, employeeOne, "Employee retrieved successfully.");
                }
                else
                {
                    return TResponse<EmployeeDTO>.Create(false, null, "Employee not found.");
                }
            }
            catch (Exception ex)
            {
                string detailedMessage = $"Error: {ex.Message}\nStackTrace: {ex.StackTrace}";
                return TResponse<EmployeeDTO>.Create(false, null, detailedMessage);
            }
        }

        public async Task<TResponse<IEnumerable<EmployeeWithTimesDTO>>> GetEmployeesWithTimesAsync()
        {
            try
            {
                var employees = await _dbContext.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Gender)
                    .ToListAsync();

                var times = await _dbContext.EmployeeTimes
                    .Include(et => et.Employee)
                    .ToListAsync();

                var employeeDTOs = employees.Select(employee => _mapper.Map<EmployeeDTO>(employee)).ToList();
                var timesDTOs = times.Select(time => _mapper.Map<EmployeeTimeDTO>(time)).ToList();

                // Combinar empleados y tiempos
                var result = employeeDTOs.Select(employee => new EmployeeWithTimesDTO
                {
                    Employee = employee,
                    Times = timesDTOs.Where(t => t.EmployeeId == employee.EmployeeId).ToList() // Filtrar los tiempos para el empleado
                });

                return TResponse<IEnumerable<EmployeeWithTimesDTO>>.Create(true, result, "Employees with times retrieved successfully.");
            }
            catch (Exception ex)
            {
                string detailedMessage = $"Error: {ex.Message}\nStackTrace: {ex.StackTrace}";
                return TResponse<IEnumerable<EmployeeWithTimesDTO>>.Create(false, null, detailedMessage);
            }
        }

        /*public async Task<TResponse<EmployeeDTO>> CreateEmployeeAsync(EmployeeDTO employeeDTO)
        {
            try
            {
                if (employeeDTO == null)
                {
                    return TResponse<EmployeeDTO>.Create(false, null, "Invalid employee data.");
                }

                var newEmployeeEntity = new EmployeeDTO
                {
                    EmployeeName = employeeDTO.EmployeeName,
                    DateOfJoining = employeeDTO.DateOfJoining,
                    PhotoFileName = employeeDTO.PhotoFileName,
                    //DepartmentId = employeeDTO.Department.DepartmentId,
                    //GenderId = employeeDTO.Gender.GenderId
                    Department = employeeDTO.Department,
                    Gender = employeeDTO.Gender
                };

                var newEmployee = _dbContext.Employee.Add(newEmployeeEntity).Entity;
                await _dbContext.SaveChangesAsync();

                if (newEmployee != null)
                {
                    var newEmployeeDTO = new EmployeeDTO
                    {
                        EmployeeId = newEmployee.EmployeeId,
                        EmployeeName = newEmployee.EmployeeName,
                        DateOfJoining = newEmployee.DateOfJoining,
                        PhotoFileName = newEmployee.PhotoFileName,
                        Department = new DepartmentDTO
                        {
                            DepartmentId = newEmployee.Department.DepartmentId,
                            DepartmentName = newEmployee.Department.DepartmentName
                        },
                        Gender = new GenderDTO
                        {
                            GenderId = newEmployee.Gender.GenderId,
                            GenderName = newEmployee.Gender.GenderName
                        }
                    };

                    return TResponse<EmployeeDTO>.Create(true, newEmployeeDTO, "Employee added successfully.");
                }
                else
                {
                    return TResponse<EmployeeDTO>.Create(false, null, "Failed to retrieve the newly created employee.");
                }
            }
            catch (Exception ex)
            {
                string detailedMessage = $"Error: {ex.Message}\nStackTrace: {ex.StackTrace}";
                return TResponse<EmployeeDTO>.Create(false, null, detailedMessage);
            }
        }*/
        public async Task<TResponse<EmployeeDTO>> CreateEmployeeAsync(EmployeeDTO employeeDTO)
        {
            try
            {
                if (employeeDTO == null || !IsValidEmployee(employeeDTO))
                {
                    return TResponse<EmployeeDTO>.Create(false, null, "Invalid employee data.");
                }

                // Generar un código de empleado único
                string employeeCode = await GenerateUniqueEmployeeCodeAsync();

                // Mapea el objeto DTO a la entidad Employee
                var newEmployeeEntity = new Employee
                {
                    EmployeeCode = employeeCode,
                    EmployeeName = employeeDTO.EmployeeName,
                    DateOfJoining = employeeDTO.DateOfJoining,
                    PhotoFileName = employeeDTO.PhotoFileName,
                    DepartmentId = employeeDTO.Department.DepartmentId,
                    GenderId = employeeDTO.Gender.GenderId
                };


                // Agrega la entidad Employee a la base de datos
                var employeeCreated = _dbContext.Employees.Add(newEmployeeEntity).Entity;
                await _dbContext.SaveChangesAsync();

                // Mapea la entidad creada nuevamente al objeto DTO
                var createdEmployeeDTO = await GetEmployeeByIdAsync(employeeCreated.EmployeeId);
                return TResponse<EmployeeDTO>.Create(true, createdEmployeeDTO.Data, "Employee added successfully.");
            }
            catch (DbUpdateException ex)
            {
                // Maneja excepciones específicas del Entity Framework aquí
                string detailedMessage = $"Database error: {ex.Message}";
                return TResponse<EmployeeDTO>.Create(false, null, detailedMessage);
            }
            catch (Exception ex)
            {
                string detailedMessage = $"Error: {ex.Message}\nStackTrace: {ex.StackTrace}";
                return TResponse<EmployeeDTO>.Create(false, null, detailedMessage);
            }
        }


        private bool IsValidEmployee(EmployeeDTO employee)
        {
            if (string.IsNullOrEmpty(employee.EmployeeName))
            {
                // Nombre de empleado no puede ser nulo o vacío
                return false;
            }

            if (employee.DateOfJoining > DateTime.Now)
            {
                // La fecha de ingreso no puede estar en el futuro
                return false;
            }

            // Puedes agregar más validaciones aquí según tus requisitos
            // Por ejemplo, validar el formato del nombre, la longitud del nombre, etc.

            return true; // Todos los campos son válidos
        }



        public async Task<TResponse<EmployeeDTO>> UpdateEmployeeAsync(EmployeeDTO emp)
        {
            try
            {
                if (emp == null)
                {
                    return TResponse<EmployeeDTO>.Create(false, null, "Invalid employee data.");
                }

                var existingEmployee = await _dbContext.Employees.FindAsync(emp.EmployeeId);

                if (existingEmployee == null)
                {
                    return TResponse<EmployeeDTO>.Create(false, null, "Employee not found.");
                }

                existingEmployee.EmployeeName = emp.EmployeeName;
                existingEmployee.DepartmentId = emp.Department.DepartmentId;
                //existingEmployee.Department.DepartmentId = emp.Department.DepartmentId;
                existingEmployee.DateOfJoining = emp.DateOfJoining;
                existingEmployee.PhotoFileName = emp.PhotoFileName;
                existingEmployee.GenderId = emp.Gender.GenderId;
                //existingEmployee.Gender.GenderId = emp.Gender.GenderId;

                await _dbContext.SaveChangesAsync();

                var updatedEmployee = await GetEmployeeByIdAsync(existingEmployee.EmployeeId);

                return TResponse<EmployeeDTO>.Create(true, updatedEmployee.Data, "Employee updated successfully.");
            }
            catch (Exception ex)
            {
                string detailedMessage = $"Error: {ex.Message}\nStackTrace: {ex.StackTrace}";
                return TResponse<EmployeeDTO>.Create(false, null, detailedMessage);
            }
        }

        public async Task<TResponse<string>> DeleteEmployeeAsync(int id)
        {
            try
            {
                var employee = await _dbContext.Employees.FindAsync(id);

                if (employee == null)
                {
                    return TResponse<string>.Create(false, null, "Employee not found.");
                }

                _dbContext.Employees.Remove(employee);
                await _dbContext.SaveChangesAsync();

                return TResponse<string>.Create(true, "Deleted Successfully", "Employee deleted successfully.");
            }
            catch (Exception ex)
            {
                string detailedMessage = $"Error: {ex.Message}\nStackTrace: {ex.StackTrace}";
                return TResponse<string>.Create(false, null, detailedMessage);
            }
        }

        public async Task<TResponse<string>> SaveFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return TResponse<string>.Create(false, FILE_NAME_DEFAULT, "No file uploaded.");
                }

                string fileName = file.FileName;
                var physicalPath = Path.Combine(_env.ContentRootPath, "Photos", fileName);

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return TResponse<string>.Create(true, fileName, "File saved successfully.");
            }
            catch (Exception ex)
            {
                return TResponse<string>.Create(false, FILE_NAME_DEFAULT, $"Error: {ex.Message}");
            }
        }

        public async Task<TResponse<IEnumerable<GenderCountDTO>>> GetGenderCountAsync()
        {
            try
            {
                var genderCounts = await _dbContext.Employees
                    .GroupBy(e => e.Gender.GenderName)
                    .Select(g => new GenderCountDTO
                    {
                        GenderName = g.Key,
                        EmployeeCount = g.Count()
                    })
                    .ToListAsync();

                return TResponse<IEnumerable<GenderCountDTO>>.Create(true, genderCounts, "Gender count retrieved successfully.");
            }
            catch (Exception ex)
            {
                string detailedMessage = $"Error: {ex.Message}\nStackTrace: {ex.StackTrace}";
                return TResponse<IEnumerable<GenderCountDTO>>.Create(false, null, detailedMessage);
            }
        }

        // Método para generar el código de empleado
        private async Task<string> GenerateUniqueEmployeeCodeAsync()
        {
            Random random = new Random();
            string employeeCode;

            do
            {
                int randomSuffix = random.Next(10, 100); // Genera un número entre 10 y 99
                employeeCode = $"20017{randomSuffix}";

                // Verifica si el código ya existe en la base de datos
            } while (await _dbContext.Employees.AnyAsync(e => e.EmployeeCode == employeeCode));

            return employeeCode;
        }
    }
}
