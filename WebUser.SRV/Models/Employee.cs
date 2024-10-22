using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using WebUser.SRV.ModelsDTO;

namespace WebUser.SRV.Models
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "EmployeeCode is required.")]
        [MaxLength(50)]
        public string EmployeeCode { get; set; }

        [Required(ErrorMessage = "EmployeeName is required.")]
        public string EmployeeName { get; set; }

        [Required(ErrorMessage = "DateOfJoining is required.")]
        public DateTime DateOfJoining { get; set; }

        public string PhotoFileName { get; set; }

        // Clave foránea para el departamento
        [ForeignKey("DepartmentId")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        // Clave foránea para el género
        [ForeignKey("GenderId")]
        public int GenderId { get; set; }
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Rate is required.")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Rate { get; set; }

        [Required(ErrorMessage = "Overtime rate is required.")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal OvertimeRate { get; set; }
    }


}

