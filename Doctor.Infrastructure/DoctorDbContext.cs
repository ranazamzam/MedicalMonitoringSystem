using Microsoft.EntityFrameworkCore;
using Doctor.Domain.Models;
using Doctor.Infrastructure.EntityConfigurations;

namespace Doctor.Infrastructure
{
    public class DoctorDbContext : DbContext
    {
        public DoctorDbContext(DbContextOptions<DoctorDbContext> options)
         : base(options)
        {
        }
        public DbSet<DoctorEntity> Doctors { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new DoctorConfigurations());
        }
    }
}
