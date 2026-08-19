using MbaCrm.Api.Constants;

using Microsoft.AspNetCore.Identity;

namespace MbaCrm.Api.Data
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(
            RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames =
            {
                AppRoles.Admin,
                AppRoles.User,
                AppRoles.Customer
            };

            foreach (var roleName in roleNames)
            {
                var roleExists =
                    await roleManager.RoleExistsAsync(roleName);

                if (!roleExists)
                {
                    var result = await roleManager.CreateAsync(
                        new IdentityRole(roleName)
                    );

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(
                            ", ",
                            result.Errors.Select(error => error.Description)
                        );

                        throw new InvalidOperationException(
                            $"{roleName} rolü oluşturulamadı: {errors}"
                        );
                    }
                }
            }
        }
    }
}