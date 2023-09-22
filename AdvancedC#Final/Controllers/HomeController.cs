using AdvancedC_Final.Areas.Identity.Data;
using AdvancedC_Final.Data;
using AdvancedC_Final.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AdvancedC_Final.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TaskManagerContext _context;

        private readonly UserManager<TaskManagerUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(
            ILogger<HomeController> logger, 
            TaskManagerContext context, 
            UserManager<TaskManagerUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            await SeedData();

            return View();
        }

        private async Task SeedData()
        {
            string username = "userAdmin@gmail.com";
            string password = "123qwe!Q";
            string[] roles = { "Developer", "Project Manager", "Administrator" };

            foreach (string roleName in roles)
            {
                try
                {
                    if (!await _roleManager.RoleExistsAsync(roleName))
                    {
                        IdentityRole role = new IdentityRole(roleName);
                        await _roleManager.CreateAsync(role);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while seeding role {roleName}: {ex.Message}");
                }
            }

            TaskManagerUser existingUser = await _userManager.FindByEmailAsync(username);

            if (existingUser == null)
            {
                TaskManagerUser adminUser = new TaskManagerUser
                {
                    UserName = username,
                    Email = username
                };

                IdentityResult result = await _userManager.CreateAsync(adminUser, password);

                if (!result.Succeeded)
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        Console.WriteLine(error.Description);
                    }
                    throw new Exception("Failed to create admin user.");
                }

                if (!await _userManager.IsInRoleAsync(adminUser, roles[2]))
                {
                    await _userManager.AddToRoleAsync(adminUser, roles[2]);
                }

                adminUser.LockoutEnabled = false;
                adminUser.EmailConfirmed = true;
                await _userManager.UpdateAsync(adminUser);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}