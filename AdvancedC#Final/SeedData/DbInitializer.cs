using AdvancedC_Final.Areas.Identity.Data;
using AdvancedC_Final.Data;
using Microsoft.AspNetCore.Identity;

namespace AdvancedC_Final.SeedData
{
    public class DbInitializer
    {
        public static async Task SeedData(TaskManagerContext context, UserManager<TaskManagerUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string username = "userAdmin@gmail.com";
            string password = "123qwe!Q";
            string[] roles = { "Developer", "Project Manager", "Administrator" };

            foreach (string roleName in roles)
            {
                try
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        IdentityRole role = new IdentityRole(roleName);
                        await roleManager.CreateAsync(role);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while seeding role {roleName}: {ex.Message}");
                }
            }

            TaskManagerUser existingUser = await userManager.FindByNameAsync(username);

            if (existingUser == null)
            {
                TaskManagerUser adminUser = new TaskManagerUser
                {
                    UserName = username,
                    Email = username,
                    LockoutEnabled = false,
                    EmailConfirmed = true
                };

                IdentityResult result = await userManager.CreateAsync(adminUser, password);

                if (!result.Succeeded)
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        Console.WriteLine(error.Description);
                    }
                    throw new Exception("Failed to create admin user.");
                }

                if (!await userManager.IsInRoleAsync(adminUser, roles[2]))
                {
                    await userManager.AddToRoleAsync(adminUser, roles[2]);
                }
            }
        }
    }
}
