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
                AppRoles.User
            };

            foreach (var roleName in roleNames)
            {
                var roleExists =
                    await roleManager.RoleExistsAsync(roleName);

                if (!roleExists)
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(roleName)
                    );
                }
            }
        }
    }
}