using Ecommerce.Core.Entites.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Data.Configurations.IdentityConfig
{
    internal class UserConfigurations : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(x => x.UserName)
                   .IsRequired()
                   .HasMaxLength(30);

            builder.Property(x => x.DisplayName)
                   .IsRequired()
                   .HasMaxLength(80);

            builder.HasMany(x => x.RefreshTokens)
                   .WithOne(r => r.User)
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
