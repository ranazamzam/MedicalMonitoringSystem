using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patient.Infrastructure
{
    public class PatientDbContext : DbContext
    {
        public PatientDbContext(DbContextOptions<PatientDbContext> options)
         : base(options)
        {
        }


        //public DbSet<Request> Requests { get; set; }
        //public DbSet<ProductProposalParametersOnAppLayer> ProductProposalParametersOnAppLayers { get; set; }
        //public DbSet<InsuranceCompanyUserMapping> InsuranceCompanyUserMappings { get; set; }
        //public DbSet<AgentUserMapping> AgentUserMappings { get; set; }
        //public DbSet<KeyDefaultValue> KeyDefaultValues { get; set; }
        //public DbSet<KeyDefaultValueDependency> KeyDefaultValueDependencies { get; set; }
        //public DbSet<ForwardHistory> ForwardHistories { get; set; }
        //public DbSet<InsuranceCompanyRegion> InsuranceCompanyRegions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //base.OnModelCreating(builder);

            //builder.Entity<Domain.Entities.IdentityRole>().ToTable("Roles")
            //       .HasMany(m => m.UserRoles)
            //       .WithOne(o => o.Role)
            //       .HasForeignKey(f => f.RoleId)
            //       .OnDelete(DeleteBehavior.Cascade);

            //builder.Entity<IdentityUserRole>().ToTable("UserRoles");
            //builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            //builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            //builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            //builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
        }
    }
}
