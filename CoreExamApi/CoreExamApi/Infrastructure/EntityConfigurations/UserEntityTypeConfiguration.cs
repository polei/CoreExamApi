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
            builder.Property(cb => cb.Password)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(cb => cb.Sex)
                .IsRequired();
            builder.Property(cb => cb.CompanyName)
                .IsRequired(false)
                .HasMaxLength(250);
            builder.Property(cb => cb.Address)
                .IsRequired(false)
                .HasMaxLength(200);
            builder.Property(cb => cb.CertificateNumber)
                .IsRequired(false)
                .HasMaxLength(50);

            builder.Property(cb => cb.Province)
                .IsRequired(true);

            builder.Property(cb => cb.City)
                .IsRequired(true);

            builder.Property(cb => cb.IsEngineer)
                .IsRequired(true);

            builder.Property(cb => cb.IsChildCompany)
                .IsRequired(true);

            builder.Property(cb => cb.ChildCity)
                .IsRequired(false);

            builder.Property(cb => cb.BirthYear)
                .IsRequired(false);

            builder.Property(cb => cb.BirthMonth)
                .IsRequired(false);
        }
    }
}
