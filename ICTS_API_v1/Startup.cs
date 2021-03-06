using ICTS_API_v1.Models;
using ICTS_API_v1.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ICTS_API_v1
{
    /// <summary>
    /// Services required by the app are configured.
    /// The app's request handling pipeline is defined, as a series of middleware components.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Constructor for Startup
        /// </summary>
        /// <param name="configuration">Configuration properties</param>
        /// <returns>Startup object</returns>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //Configuration properties
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ICTS_Context>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddControllers();
            services.AddTransient<MPROCProductsService>();
            services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
