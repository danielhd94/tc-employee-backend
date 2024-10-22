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

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configurar AutoMapper
            services.AddAutoMapper(typeof(AutoMapperConfiguration));

            // Imprimir configuración
            PrintConfiguration();

            // Configurar DbContext
            ConfigureDbContext(services);

            // Configurar servicios
            ConfigureMyServices(services);

            // Configurar CORS
            ConfigureCors(services);

            // Configurar controladores
            services.AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

            // Configurar Swagger
            ConfigureSwagger(services);
        }

        private void PrintConfiguration()
        {
            Console.WriteLine("-------- Configuración completa --------");
            foreach (var c in Configuration.AsEnumerable())
            {
                Console.WriteLine($"{c.Key} = {c.Value}");
            }
            Console.WriteLine("----------------------------------------");
        }

        private void ConfigureDbContext(IServiceCollection services)
        {
            var connectionString = BuildConnectionString();
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString", "La cadena de conexión no puede ser nula o vacía.");
            }

            services.AddDbContext<MyDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                    sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
            });
        }

        private void ConfigureMyServices(IServiceCollection services)
        {
            services.AddTransient<IDepartmentService, DepartmentService>();
            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<IEmployeeTimeService, EmployeeTimeService>();
        }

        private void ConfigureCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API REST",
                    Version = "v1",
                    Description = "Documentación de servicios de empleados"
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

        private string BuildConnectionString()
        {
            return $"Data Source={Configuration["DB_SERVER"]};" +
                   $"Initial Catalog={Configuration["DB_NAME"]};" +
                   $"User Id={Configuration["DB_USER"]};" +
                   $"Password={Configuration["DB_PASSWORD"]};" +
                   $"Encrypt={Configuration["ENCRRYPT"]};" +
                   $"TrustServerCertificate={Configuration["TRUSTCERTIFICATE"]};" +
                   $"Connection Timeout={Configuration["CONTIMEOUT"]};";
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API REST v1");
            });

            app.UseCors("AllowFrontend");

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
