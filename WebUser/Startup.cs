using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WebUser.Mapper;
using WebUser.SRV;
using WebUser.SRV.Interfaces;
using WebUser.SRV.Services;

namespace WebUser
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private string BuildConnectionString()
        { 
            return $"Data Source={Environment.GetEnvironmentVariable("DB_SERVER")};" +
                   $"Initial Catalog={Environment.GetEnvironmentVariable("DB_NAME")};" +
                   $"User Id={Environment.GetEnvironmentVariable("DB_USER")};" +
                   $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
                   $"Encrypt={Environment.GetEnvironmentVariable("ENCRRYPT")};" +
                   $"TrustServerCertificate={Environment.GetEnvironmentVariable("TRUSTCERTIFICATE")};" +
                   $"Connection Timeout={Environment.GetEnvironmentVariable("CONTIMEOUT")};";

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Auto Mapper Configurations
            services.AddSingleton(provider => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperConfiguration());
            }).CreateMapper());

            // Imprimir toda la configuración
            Console.WriteLine("-------- Configuración completa --------");
            foreach (var c in Configuration.AsEnumerable())
            {
                Console.WriteLine($"{c.Key} = {c.Value}");
            }
            Console.WriteLine("----------------------------------------");


            // Configuración del DbContext
            services.AddDbContext<MyDbContext>(options =>
            {
                // Configurar el DbContext con la cadena de conexión
                var connectionString = BuildConnectionString(); // Configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new ArgumentNullException("connectionString", "La cadena de conexión no puede ser nula o vacía.");
                }

                options.UseSqlServer(connectionString, sqlOptions =>
                    sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
            });

            // Configuración de servicios
            services.AddTransient<IDepartmentService, DepartmentService>();
            services.AddTransient<IEmployeeService, EmployeeService>();

            // Enable CORS
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            // Configuración de controladores
            services.AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

            // Add Swagger documentation
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API REST",
                    Version = "v1",
                    Description = "Employee's documentation services"
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
                else
                {
                    Console.WriteLine($"Warning: XML file not found at {xmlPath}");
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Habilitar Swagger
            app.UseSwagger();

            // Indica la ruta para generar la configuración de Swagger
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API REST v1");
            });

            // Enable CORS
            app.UseCors("AllowOrigin");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Configuración de archivos estáticos
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Photos")),
                RequestPath = "/Photos"
            });
        }
    }
}
