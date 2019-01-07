using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Doctor.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Infrastructure
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {

            var tasks = new List<Task>
            {
                AddDoctors(serviceProvider)
            };

            Task.WaitAll(tasks.ToArray());
        }

        private static async Task AddDoctors(IServiceProvider serviceProvider)
        {
            var doctors = new List<DoctorEntity>()
            {
                new DoctorEntity() { Name = "Mikael Seström" },
                new DoctorEntity() { Name = "Carina Axel" },
                new DoctorEntity() { Name = "Martin Eriksson"  }
            };

            using (var context = new DoctorDbContext(serviceProvider.GetRequiredService<DbContextOptions<DoctorDbContext>>()))
            {
                context.Database.EnsureCreated();

                if (context.Doctors.Any())
                    return;

                context.Doctors.AddRange(doctors);

                await context.SaveChangesAsync();
            }
        }
    }
}
