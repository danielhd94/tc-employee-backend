using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using WebUser.Managers;

namespace WebUser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Condicionar el uso de URLs basado en el entorno
                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                    {
                        webBuilder.UseUrls("http://localhost:5002"); // URL para desarrollo
                    }
                    else
                    {
                        webBuilder.UseUrls("http://*:5003"); // URL para producción
                    }

                    webBuilder.ConfigureAppConfiguration((context, builder) =>
                    {
                        var env = context.HostingEnvironment;
                        var logger = context.Configuration.GetSection("Logging");

                        Console.WriteLine("ddddddddddd----> "+env.EnvironmentName);

                        builder.SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables()
                            .AddCommandLine(args);
                    });

                    

                    webBuilder.UseStartup<Startup>();

                });
    }
}

