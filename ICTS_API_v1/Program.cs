using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ICTS_API_v1
{
    /// <summary>
    /// Main class of the project
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point of the program
        /// </summary>
        /// <param name="args">Command line args</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates a new instance of IHostBuilder with pre-configured defaults
        /// </summary>
        /// <param name="args">Command line args</param>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseWebRoot("Content");
                });
    }
}
