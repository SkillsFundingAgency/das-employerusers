using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Data.Users;

public class UserProfilesEntityConfiguration : IEntityTypeConfiguration<UserProfileEntity>
{
    public void Configure(EntityTypeBuilder<UserProfileEntity> builder)
    {
        builder.ToTable("User");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id).HasColumnName("Id").HasColumnType("varchar").HasMaxLength(50).IsRequired();
        builder.Property(x => x.FirstName).HasColumnName("FirstName").HasColumnType("varchar").HasMaxLength(50).IsRequired(false);
        builder.Property(x => x.LastName).HasColumnName("LastName").HasColumnType("varchar").HasMaxLength(50).IsRequired(false);
        builder.Property(x => x.Email).HasColumnName("Email").HasColumnType("varchar").HasMaxLength(255).IsRequired();
        builder.Property(x => x.GovUkIdentifier).HasColumnName("GovUkIdentifier").HasColumnType("varchar").HasMaxLength(150).IsRequired(false);
        builder.Property(x => x.IsSuspended).HasColumnName("IsSuspended").HasColumnType("bit");
        
        builder.HasIndex(x => x.GovUkIdentifier).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();
    }
}