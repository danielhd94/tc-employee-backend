using System.Reflection;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebUser.SRV.ModelsDTO;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Especifica el esquema leído de la configuración
         
            modelBuilder.HasDefaultSchema(_schema);

            // Configuraciones adicionales del modelo si es necesario

            base.OnModelCreating(modelBuilder);
        }
    }

}
