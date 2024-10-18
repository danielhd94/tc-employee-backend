using System.Reflection;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using WebUser.SRV.ModelsDTO;

namespace WebUser.SRV
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        // Define las propiedades DbSet para las entidades que deseas mapear a la base de datos
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Gender> Genders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("danapp"); // Especifica el esquema predeterminado

            // Configuraciones adicionales del modelo si es necesario

            base.OnModelCreating(modelBuilder);
        }
    }

}
