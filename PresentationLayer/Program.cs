using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresentationLayer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Init logger
            Log.Logger = new LoggerConfiguration()
                    //Save in file (sink)
                    .WriteTo.File(
                        //File path
                        path: "D:\\CSharp\\Projeto - Social Media Platform\\SocialMediaPlatform\\Logs\\log-.log",
                        //Timestamp; level of event; message body; new line; exception (optional)
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                        //create a new log file every day
                        rollingInterval: RollingInterval.Day,
                        //only events of level "information" and higher (warning, error, ...) will be logged
                        restrictedToMinimumLevel: LogEventLevel.Information
                    ).CreateLogger();

            try
            {
                //write log info
                Log.Information("Application is starting...");

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                //write fatal log with exception
                Log.Fatal(ex, "Application failed to start");
            }

            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
