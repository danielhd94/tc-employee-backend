using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace WebUser.SRV.ModelsDTO
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }

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
    }


}

