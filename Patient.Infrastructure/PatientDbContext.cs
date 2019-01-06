using Microsoft.EntityFrameworkCore;
using Patient.Domain.Models;
using Patient.Infrastructure.EntityConfigurations;

namespace Patient.Infrastructure
{
    public class PatientDbContext : DbContext
    {
        public PatientDbContext(DbContextOptions<PatientDbContext> options)
         : base(options)
        {
        }
        public DbSet<PatientEntity> Patients { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new PatientConfigurations());
        }
    }
}
