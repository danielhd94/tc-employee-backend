using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebUser.SRV.Models
{
    public class Gender
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GenderId { get; set; }

        [Required(ErrorMessage = "GenderName is required.")]
        public string GenderName { get; set; }

        // Colección de empleados con este género
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }

}

