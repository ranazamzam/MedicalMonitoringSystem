using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Patient.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patient.Infrastructure
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {

            var tasks = new List<Task>
            {
                AddPatients(serviceProvider)
            };

            Task.WaitAll(tasks.ToArray());
        }

        private static async Task AddPatients(IServiceProvider serviceProvider)
        {
            var patients = new List<PatientEntity>()
            {
                new PatientEntity() { Name = "Henrik Karlsson" },
                new PatientEntity() { Name = "Erik Henriksson" },
                new PatientEntity() { Name = "Cecilia Eliasson"  }
            };

            using (var context = new PatientDbContext(serviceProvider.GetRequiredService<DbContextOptions<PatientDbContext>>()))
            {
                context.Database.EnsureCreated();

                if (context.Patients.Any())
                    return;

                context.Patients.AddRange(patients);

                await context.SaveChangesAsync();
            }
        }
    }
}
