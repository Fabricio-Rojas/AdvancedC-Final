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

            Project? project = await _context.Projects.Include(p => p.Developers).FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            List<TaskManagerUser> developers = (List<TaskManagerUser>)await _userManager.GetUsersInRoleAsync("Developer");

            if (project.Developers.Count > 0)
            {
                foreach(DeveloperProject dp in project.Developers)
                {
                    developers.Remove(developers.FirstOrDefault(d => d.Id == dp.DeveloperId));
                }
            }

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
                TaskManagerUser? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == developerProject.DeveloperId);

                if (project == null || user == null)
                {
                    return NotFound();
                }

                _context.DeveloperProjects.Add(developerProject);

                developerProject.Project = project;
                developerProject.Developer = user;

                project.Developers.Add(developerProject);

                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Projects", new { id = developerProject.ProjectId });
            }
            return View(developerProject);
        }

        // GET: Projects/AddDevTicket
        [Authorize(Roles = "Project Manager")]

        public async Task<IActionResult> AddDevTicket(int? ticketId, int? projectId)
        {
            if (ticketId == null || _context.Tickets == null || projectId == null || _context.Projects == null)
            {
                return NotFound();
            }

            Ticket? ticket = await _context.Tickets.Include(t => t.Developers).ThenInclude(dt => dt.User).FirstOrDefaultAsync(m => m.Id == ticketId);
            Project? project = await _context.Projects.Include(p => p.Developers).ThenInclude(dp => dp.Developer).FirstOrDefaultAsync(m => m.Id == projectId);

            if (ticket == null || project == null)
            {
                return NotFound();
            }

            List<DeveloperProject> developers = project.Developers.ToList();
            List<DeveloperProject> correctedDevelopers = new List<DeveloperProject>();

            foreach (DeveloperProject dp in developers)
            {
                if (!ticket.Developers.Any(d => d.UserId == dp.DeveloperId))
                {
                    correctedDevelopers.Add(dp);
                }
            }

            ViewBag.Developers = correctedDevelopers;

            DeveloperTicket developerTicket = new DeveloperTicket
            {
                TicketId = ticket.Id
            };

            return View("AddDevTicket", developerTicket);
        }

        // POST: Projects/AddDevTicket
        [HttpPost, ActionName("AddDevTicket")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> AddDevTicket([Bind("Id, UserId, TicketId")] DeveloperTicket developerTicket)
        {
            if (developerTicket.Id != default)
            {
                developerTicket.TicketId = developerTicket.Id;
                developerTicket.Id = default;
            }
            TryValidateModel(developerTicket);
            if (ModelState.IsValid)
            {
                Ticket? ticket = await _context.Tickets
                    .FirstOrDefaultAsync(p => p.Id == developerTicket.TicketId);

                if (ticket == null)
                {
                    return NotFound();
                }

                _context.DeveloperTickets.Add(developerTicket);

                developerTicket.Ticket = ticket;

                ticket.Developers.Add(developerTicket);

                await _context.SaveChangesAsync();

                return RedirectToAction("DetailTicket", "Projects", new { id = developerTicket.TicketId });
            }
            return View(developerTicket);
        }

    }
}
