using CoreExamApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Infrastructure.EntityConfigurations
{
    public class UserEntityTypeConfiguration:IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("PMSoft_UserInfo");

            //builder.HasKey(cb => cb.ID);

            builder.Property(cb => cb.ID).IsRequired();

            builder.Property(cb => cb.UserName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(cb => cb.CompanyName)
                .IsRequired(false)
                .HasMaxLength(250);
        }
    }
}
