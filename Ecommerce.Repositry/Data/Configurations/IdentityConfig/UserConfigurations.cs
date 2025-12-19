
using Ecommerce.Core;

namespace Ecommerce.Infrastructure;

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

        builder.HasData(new ApplicationUser
        {
            Id = DefaultUsers.AdminId,
            DisplayName = "Ecommerce Admin",
            UserName = DefaultUsers.AdminEmail,
            NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
            Email = DefaultUsers.AdminEmail,
            NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
            SecurityStamp = DefaultUsers.AdminSecurityStamp,
            ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
            EmailConfirmed = true,
            PasswordHash = "AQAAAAIAAYagAAAAEOH6D/vkD9oFT+v/vYCeVxgFl3vfVYP9jcJXBCGU6Wzy2pOoVMQk4MGaGg+vJf7vAg=="

        });

    }


}

