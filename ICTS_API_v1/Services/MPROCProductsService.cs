using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ICTS_API_v1.Models;
using Microsoft.AspNetCore.Hosting;


namespace ICTS_API_v1.Services
{
    /// <summary>
    /// Service for reading MPROC products data
    /// </summary>
    public class MPROCProductsService
    {
        /// <summary>
        /// Constructor for MPROCProductsService
        /// </summary>
        /// <param name="webHostEnvironment">Web hosting environment</param>
        /// <returns>MPROCProductsService object</returns>
        public MPROCProductsService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        //Web hosting environment
        public IWebHostEnvironment WebHostEnvironment { get; }

        //Path for json file containing MPROC products data
        public string JsonFileName
        {
            get { return Path.Combine(WebHostEnvironment.WebRootPath, "Data", "MPROCProducts.json"); }
        }

        /// <summary>
        /// Reads json file containing MPROC products data
        /// </summary>
        /// <returns>List of MPROC products</returns>
        public IEnumerable<MPROCProduct> GetMPROCProducts()
        {
            using (var jsonFileReader = File.OpenText(JsonFileName))
            {
                return JsonSerializer.Deserialize<MPROCProduct[]>(jsonFileReader.ReadToEnd(),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            }
        }
    }
}
