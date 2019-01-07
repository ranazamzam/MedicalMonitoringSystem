using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Doctor.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Infrastructure.EntityConfigurations
{
    public class DoctorConfigurations : IEntityTypeConfiguration<DoctorEntity>
    {
        public void Configure(EntityTypeBuilder<DoctorEntity> builder)
        {
            //builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                   .HasMaxLength(500)
                   .IsUnicode(false);
        }
    }
}
