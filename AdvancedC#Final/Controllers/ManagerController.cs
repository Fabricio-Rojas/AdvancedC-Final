using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AdvancedC_Final.Areas.Identity.Data;
using AdvancedC_Final.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AdvancedC_Final.Data;

namespace AdvancedC_Final.Controllers
{
    [Authorize(Roles = "Project Manager")]
    public class ManagerController : Controller
    {
        private readonly TaskManagerContext _context;
        private readonly UserManager<TaskManagerUser> _userManager;

        public ManagerController(TaskManagerContext context, UserManager<TaskManagerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Projects/AddDevProject
        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> AddDevProject(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            Project? project = await _context.Projects.FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            List<TaskManagerUser> developers = (List<TaskManagerUser>)await _userManager.GetUsersInRoleAsync("Developer");

            ViewBag.Developers = developers;

            DeveloperProject developerProject = new DeveloperProject
            {
                ProjectId = project.Id
            };

            return View("AddDevProject", developerProject);
        }

        // POST: Projects/AddDevProject

        [HttpPost, ActionName("AddDevProject")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> AddDevProject([Bind("Id, DeveloperId, ProjectId")] DeveloperProject developerProject)
        {
            developerProject.Id = default;
            TryValidateModel(developerProject);
            if (ModelState.IsValid)
            {
                Project? project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == developerProject.ProjectId);

                if (project == null)
                {
                    return NotFound();
                }

                _context.DeveloperProjects.Add(developerProject);

                developerProject.Project = project;

                project.Developers.Add(developerProject);

                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Projects", new { id = developerProject.ProjectId });
            }
            return View(developerProject);
        }

        // GET: Projects/AddDevTicket
        [Authorize(Roles = "Project Manager")]

        public async Task<IActionResult> AddDevTicket(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            Ticket? ticket = await _context.Tickets.FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            List<TaskManagerUser> developers = (List<TaskManagerUser>)await _userManager.GetUsersInRoleAsync("Developer");

            ViewBag.Developers = developers;

            DeveloperTicket developerTicket = new DeveloperTicket
            {
                TickedId = ticket.Id
            };

            return View("AddDevTicket", developerTicket);
        }

        // POST: Projects/AddDevTicket
        [HttpPost, ActionName("AddDevTicket")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> AddDevTicket([Bind("Id, UserId, TickedId")] DeveloperTicket developerTicket)
        {
            developerTicket.Id = default;
            TryValidateModel(developerTicket);
            if (ModelState.IsValid)
            {
                Ticket? ticket = await _context.Tickets
                    .FirstOrDefaultAsync(p => p.Id == developerTicket.TickedId);

                if (ticket == null)
                {
                    return NotFound();
                }

                _context.DeveloperTickets.Add(developerTicket);

                developerTicket.Ticket = ticket;

                ticket.Developers.Add(developerTicket);

                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Projects", new { id = developerTicket.TickedId });
            }
            return View(developerTicket);
        }

    }
}
