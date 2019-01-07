using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Doctor.Domain.Interfaces;
using Doctor.Infrastructure.Repositories;
using Doctor.Services.Interfaces;
using Doctor.Services.Services;
using Doctor.Domain.Models;
using Doctor.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MedicalBookingSystem.Doctor.Config;
using Doctor.Infrastructure.UnitOfWorks;
using Microsoft.AspNetCore.Http;

namespace MedicalBookingSystem.Doctor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddOptions();
            services.AddTransient<IDoctorService, DoctorService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddTransient<IRepository<PatientEntity>, InMemoryRepository>();

            var connectionString = DoctorServiceConfiguration.DefaultConnectiontring;
            services.AddDbContext<DoctorDbContext>(options => options.UseSqlServer(connectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");
            app.UseMvc();

            // Initialize the DB if not already initialized
            DbInitializer.Initialize(app.ApplicationServices);
        }
    }
}
