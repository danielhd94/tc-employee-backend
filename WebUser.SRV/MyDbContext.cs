using System.Reflection;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebUser.SRV.Models;

namespace WebUser.SRV
{
    public class MyDbContext : DbContext
    {
        private readonly string _schema;

        public MyDbContext(DbContextOptions<MyDbContext> options, IConfiguration configuration) : base(options)
        {
            _schema = configuration["DatabaseSettings:Schema"];
        }

        // Define las propiedades DbSet para las entidades que deseas mapear a la base de datos
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<EmployeeTime> EmployeeTimes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmployeeTime>(entity =>
            {
                entity.Property(e => e.HolidayHours)
                      .HasColumnType("decimal(18, 2)") // Especifica tipo de columna
                      .IsRequired(false); // Cambia según tus necesidades

                entity.Property(e => e.OtherHours)
                      .HasColumnType("decimal(18, 2)") // Especifica tipo de columna
                      .IsRequired(false); // Cambia según tus necesidades

                entity.Property(e => e.OvertimeHours)
                      .HasColumnType("decimal(18, 2)") // Especifica tipo de columna
                      .IsRequired(false); // Cambia según tus necesidades

                entity.Property(e => e.SickLeaveHours)
                      .HasColumnType("decimal(18, 2)") // Especifica tipo de columna
                      .IsRequired(false); // Cambia según tus necesidades

                entity.Property(e => e.VacationHours)
                      .HasColumnType("decimal(18, 2)") // Especifica tipo de columna
                      .IsRequired(false); // Cambia según tus necesidades
            });

            // Especifica el esquema leído de la configuración

            modelBuilder.HasDefaultSchema(_schema);

            // Configuraciones adicionales del modelo si es necesario

            base.OnModelCreating(modelBuilder);
        }
    }

}
