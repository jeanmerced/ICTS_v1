using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ICTS_API_v1.Models;
using Microsoft.AspNetCore.Hosting;


namespace ICTS_API_v1.Services
{
    public class MPROCProductsService
    {
        public MPROCProductsService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }

        public string JsonFileName
        {
            get { return Path.Combine(WebHostEnvironment.WebRootPath, "Data", "MPROCProducts.json"); }
        }

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