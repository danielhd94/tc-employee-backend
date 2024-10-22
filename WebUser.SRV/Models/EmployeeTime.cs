using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebUser.SRV.Models
{
	public class EmployeeTime
	{
        [Key]
        public int EmployeeTimeId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public DateTime? EntryTime { get; set; }

        public DateTime? ExitTime { get; set; }

        public int? OvertimeHours { get; set; }

        public int? SickLeaveHours { get; set; }

        public int? VacationHours { get; set; }

        public int? HolidayHours { get; set; }

        public int? OtherHours { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navegación a Employee si tienes una relación configurada
        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }
    }
}

