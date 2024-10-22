using System;
using WebUser.SRV.Models;

namespace WebUser.SRV.ModelsDTO
{
	public class EmployeeTimeDTO
	{
        public int EmployeeTimeId { get; set; }  // Identificador único del registro de horas del empleado

        public int EmployeeId { get; set; }  // Identificador del empleado

        public DateTime Date { get; set; }  // Fecha en la que se aplica el registro

        public DateTime? EntryTime { get; set; }  // Hora de entrada

        public DateTime? ExitTime { get; set; }  // Hora de salida

        public int? OvertimeHours { get; set; }  // Horas extra trabajadas

        public int? SickLeaveHours { get; set; }  // Horas de enfermedad

        public int? VacationHours { get; set; }  // Horas de vacaciones

        public int? HolidayHours { get; set; }  // Horas de días festivos

        public int? OtherHours { get; set; }  // Otras horas

        public DateTime CreatedAt { get; set; }  // Fecha y hora de creación del registro

        public DateTime UpdatedAt { get; set; }  // Fecha y hora de la última actualización

        public string  EmployeeName { get; set; }

        public string EmployeeCode { get; set; }
    }
}
