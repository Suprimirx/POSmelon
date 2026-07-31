using DrogueriaPOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrogueriaPOS.Infrastructure.Configurations;

public class AppSettingConfiguration : IEntityTypeConfiguration<AppSetting>
{
    public void Configure(EntityTypeBuilder<AppSetting> builder)
    {
        builder.ToTable("AppSettings");

        // La clave primaria es el Key, no un Id autoincremental
        builder.HasKey(s => s.Key);

        builder.Property(s => s.Key)
            .IsRequired()
            .HasMaxLength(100);
            //.ValueGeneratedNever();

        builder.Property(s => s.Value);
    }
}
