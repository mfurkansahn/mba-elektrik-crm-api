using MbaCrm.Api.Constants;
using MbaCrm.Api.Entities;

using Microsoft.AspNetCore.Identity;

namespace MbaCrm.Api.Data
{
    public static class AdminUserSeeder
    {
        public static async Task SeedAdminAsync(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            var fullName =
                configuration["AdminUser:FullName"]
                ?? throw new InvalidOperationException(
                    "AdminUser:FullName yapılandırması bulunamadı."
                );

            var email =
                configuration["AdminUser:Email"]
                ?? throw new InvalidOperationException(
                    "AdminUser:Email yapılandırması bulunamadı."
                );

            var password =
                configuration["AdminUser:Password"]
                ?? throw new InvalidOperationException(
                    "AdminUser:Password yapılandırması bulunamadı."
                );

            var adminUser =
                await userManager.FindByEmailAsync(email);

            if (adminUser is null)
            {
                adminUser = new ApplicationUser
                {
                    FullName = fullName.Trim(),
                    Email = email.Trim(),
                    UserName = email.Trim()
                };

                var createResult =
                    await userManager.CreateAsync(
                        adminUser,
                        password
                    );

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        createResult.Errors.Select(
                            error => error.Description
                        )
                    );

                    throw new InvalidOperationException(
                        $"Admin kullanıcısı oluşturulamadı: {errors}"
                    );
                }
            }

            var isAdmin =
                await userManager.IsInRoleAsync(
                    adminUser,
                    AppRoles.Admin
                );

            if (!isAdmin)
            {
                var roleResult =
                    await userManager.AddToRoleAsync(
                        adminUser,
                        AppRoles.Admin
                    );

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        roleResult.Errors.Select(
                            error => error.Description
                        )
                    );

                    throw new InvalidOperationException(
                        $"Admin rolü atanamadı: {errors}"
                    );
                }
            }
        }
    }
}