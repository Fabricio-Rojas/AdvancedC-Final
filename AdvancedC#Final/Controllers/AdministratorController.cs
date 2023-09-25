﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AdvancedC_Final.Data;
using Microsoft.AspNetCore.Identity;
using AdvancedC_Final.Areas.Identity.Data;

namespace AdvancedC_Final.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministratorController : Controller
    {
        private readonly TaskManagerContext _context;
        private readonly UserManager<TaskManagerUser> _userManager;

        public AdministratorController(
            TaskManagerContext context,
            UserManager<TaskManagerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult SetUserRole()
        {
            List<TaskManagerUser> usersWithoutRoles = _userManager.Users
                .ToList()
                .Where(u => !_userManager.GetRolesAsync(u).Result.Any())
                .ToList();

            return View(usersWithoutRoles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetUserRole(string userId, string roleName)
        {
            string mgmtRole = "Project Manager";
            string devRole = "Developer";

            TaskManagerUser user = await _userManager.FindByIdAsync(userId);

            if (roleName == "manager")
            {
                if (!await _userManager.IsInRoleAsync(user, mgmtRole))
                {
                    await _userManager.AddToRoleAsync(user, mgmtRole);
                }
            }
            else if (roleName == "developer")
            {
                if (!await _userManager.IsInRoleAsync(user, devRole))
                {
                    await _userManager.AddToRoleAsync(user, devRole);
                }
            }
            return RedirectToAction(nameof(SetUserRole));
        }
    }
}
