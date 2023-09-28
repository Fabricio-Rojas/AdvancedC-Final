using AdvancedC_Final.Areas.Identity.Data;
using AdvancedC_Final.Data;
using AdvancedC_Final.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AdvancedC_Final.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly TaskManagerContext _context;
        private readonly UserManager<TaskManagerUser> _userManager;

        public ProjectsController(TaskManagerContext context, UserManager<TaskManagerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Projects
        [Authorize(Roles = "Project Manager,Developer")]
        public async Task<IActionResult> Index(int? page)
        {
            TaskManagerUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            List<Project> data = new List<Project>();

            if (await _userManager.IsInRoleAsync(user, "Developer"))
            {
                data = _context.Projects.Include(p => p.ProjectManager).Where(p => p.Developers.Where(d => d.DeveloperId == user.Id).Any()).ToList();
            }

            if (await _userManager.IsInRoleAsync(user, "Project Manager"))
            {
                data = _context.Projects.Include(p => p.ProjectManager).Where(p => p.ProjectManagerId == user.Id).ToList();
            }

            int pageNumber = page ?? 1;
            IPagedList<Project> onePage = data.ToPagedList(pageNumber, 10);

            ViewBag.onePage = onePage;
            return View(onePage);
        }

        // GET: Projects/Details/5
        [Authorize(Roles = "Project Manager,Developer")]
        public async Task<IActionResult> Details(int? id, int? page, string sortOrder = "Title")
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            Project? project = await _context.Projects
                .Include(p => p.ProjectManager)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.Developers)
                .Include(p => p.Developers)
                .ThenInclude(d => d.Developer)
                .FirstOrDefaultAsync(m => m.Id == id);


            if (project == null)
            {
                return NotFound();
            }

            TaskManagerUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            HashSet<Ticket> tickets;

            if (await _userManager.IsInRoleAsync(user, "Developer"))
            {
                tickets = project.Tickets.Where(t => t.Developers.Any(dt => dt.UserId == user.Id)).ToHashSet();
            }
            else
            {
                tickets = project.Tickets;
            }

            ViewBag.TitleSortParm = sortOrder == "Title" ? "title_desc" : "Title";
            ViewBag.PrioritySortParm = sortOrder == "Priority" ? "priority_desc" : "Priority";
            ViewBag.HoursSortParm = sortOrder == "Hours" ? "hours_desc" : "Hours";
            ViewBag.CompletionFilter = sortOrder == "False" ? "" : "False";
            ViewBag.CurrentSort = sortOrder;

            switch (sortOrder)
            {
                case "Title":
                    tickets = tickets.OrderBy(t => t.Title).ToHashSet();
                    break;
                case "title_desc":
                    tickets = tickets.OrderByDescending(t => t.Title).ToHashSet();
                    break;
                case "Priority":
                    tickets = tickets.OrderBy(t => t.Priority).ToHashSet();
                    break;
                case "priority_desc":
                    tickets = tickets.OrderByDescending(t => t.Priority).ToHashSet();
                    break;
                case "Hours":
                    tickets = tickets.OrderBy(t => t.RequiredHours).ToHashSet();
                    break;
                case "hours_desc":
                    tickets = tickets.OrderByDescending(t => t.RequiredHours).ToHashSet();
                    break;
                case "False":
                    tickets = tickets.Where(t => t.IsCompleted == false).ToHashSet();
                    break;
                default:
                    tickets = tickets.OrderBy(t => t.Title).ToHashSet();
                    break;
            }
            project.Tickets = tickets;

            int pageNumber = page ?? 1;
            IPagedList<Ticket> onePage = tickets.ToPagedList(pageNumber, 10);

            ViewBag.CurrentPage = pageNumber;

            TicketPageVM viewModel = new TicketPageVM
            {
                Project = project,
                Tickets = onePage
            };

            return View(viewModel);
        }

        // GET: Projects/Create

        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> Create()
        {
            TaskManagerUser? loggedIn = _context.Users.FirstOrDefault(u => User.Identity.Name == u.UserName);
            string currentUserId = loggedIn.Id;

            List<TaskManagerUser> developers = (List<TaskManagerUser>)await _userManager.GetUsersInRoleAsync("Developer");

            ViewBag.Developers = developers;

            Project model = new Project()
            {
                ProjectManagerId = currentUserId
            };
            
            return View(model);
        }

        // POST: Projects/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> Create([Bind("Id,Title,ProjectManagerId")] Project project, List<string> DeveloperIds)
        {
            if (ModelState.IsValid)
            {
                foreach (string devId in DeveloperIds)
                {
                    TaskManagerUser? user = _context.Users.FirstOrDefault(u =>  u.Id == devId);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    DeveloperProject developerProject = new DeveloperProject
                    {
                        Developer = user,
                        DeveloperId = devId,
                        Project = project,
                        ProjectId = project.Id
                    };

                    project.Developers.Add(developerProject);
                }

                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Edit

        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            Project? project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ProjectManagerId")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Delete

        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            Project? project = await _context.Projects
                .Include(p => p.ProjectManager)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Projects == null)
            {
                return Problem("Entity set 'TaskManagerContext.Projects'  is null.");
            }
            Project? project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Projects/AddTicket

        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> AddTicket(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            Project? project = await _context.Projects
                .Include(p => p.ProjectManager)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            Ticket newTicket = new Ticket
            {
                ProjectId = project.Id
            };

            return View(newTicket);
        }

        // POST: Projects/AddTicket

        [HttpPost, ActionName("AddTicket")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> AddTicket([Bind("Id, Title, Priority, RequiredHours, ProjectId, IsCompleted")] Ticket ticket)
        {
            ticket.Id = default;
            TryValidateModel(ticket);
            if (ModelState.IsValid)
            {
                Project? project = await _context.Projects
                    .Include(p => p.Tickets)
                    .FirstOrDefaultAsync(p => p.Id == ticket.ProjectId);

                if (project == null)
                {
                    return NotFound();
                }

                _context.Tickets.Add(ticket);

                ticket.Project = project;

                project.Tickets.Add(ticket);

                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Projects", new { id = ticket.ProjectId });
            }
            return View(ticket);
        }

        // GET: Projects/DetailTicket
        [Authorize(Roles = "Project Manager,Developer")]
        public async Task<IActionResult> DetailTicket(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            Ticket? ticket = await _context.Tickets
                .Include(p => p.Developers)
                .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        [Authorize(Roles = "Project Manager")]
        public IActionResult EditTicket(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            Ticket? ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Project Manager")]
        public IActionResult EditTicket(int id, [Bind("Id, Title, Priority, RequiredHours, ProjectId, IsCompleted")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(ticket);
                _context.SaveChanges();

                return RedirectToAction(nameof(DetailTicket), new { id = id });
            }
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateTicketHours(int id, int RequiredHours)
        {
            Ticket? ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);

            if (ticket != null && RequiredHours <= 999 && RequiredHours >= 0)
            {
                ticket.RequiredHours = RequiredHours;
                _context.Update(ticket);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(DetailTicket), new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateTicketIsCompleted(int id, string? prevView, string? sortOrder, int? page)
        {
            Ticket? ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);

            if (ticket != null)
            {
                ticket.IsCompleted = !ticket.IsCompleted;
                _context.Update(ticket);
                _context.SaveChanges();
            }

            if (prevView == "details")
            {
                return RedirectToAction(nameof(Details), new { id = ticket.ProjectId, sortOrder = sortOrder, page = page });
            }

            return RedirectToAction(nameof(DetailTicket), new { id = id });
        }

        private bool ProjectExists(int id)
        {
            return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
