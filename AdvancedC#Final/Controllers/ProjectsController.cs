﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AdvancedC_Final.Data;
using AdvancedC_Final.Models;
using Microsoft.AspNetCore.Identity;
using AdvancedC_Final.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;

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

        public async Task<IActionResult> Index()
        {
            var taskManagerContext = _context.Projects.Include(p => p.ProjectManager);
            return View(await taskManagerContext.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            Project? project = await _context.Projects
                .Include(p => p.ProjectManager)
                .Include(p => p.Tickets)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create

        // [Authorize(Roles = "Project Manager")]
        public IActionResult Create()
        {
            TaskManagerUser? loggedIn = _context.Users.FirstOrDefault(u => User.Identity.Name == u.UserName);
            string currentUserId = loggedIn.Id;

            Project model = new Project()
            {
                ProjectManagerId = currentUserId
            };


            ViewData["ProjectManagerId"] = new SelectList(_context.Users, "Id", "Id");
            return View(model);
        }

        // POST: Projects/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> Create([Bind("Id,Title,ProjectManagerId")] Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectManagerId"] = new SelectList(_context.Users, "Id", "Id", project.ProjectManagerId);
            return View(project);
        }

        // GET: Projects/Edit

        // [Authorize(Roles = "Project Manager")]
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
            ViewData["ProjectManagerId"] = new SelectList(_context.Users, "Id", "Id", project.ProjectManagerId);
            return View(project);
        }

        // POST: Projects/Edit

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Project Manager")]
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
            ViewData["ProjectManagerId"] = new SelectList(_context.Users, "Id", "Id", project.ProjectManagerId);
            return View(project);
        }

        // GET: Projects/Delete

        // [Authorize(Roles = "Project Manager")]
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
        // [Authorize(Roles = "Project Manager")]
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

        // [Authorize(Roles = "Project Manager")]
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
        // [Authorize(Roles = "Project Manager")]
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

        // GET: Project/DetailTicket
        public async Task<IActionResult> DetailTicket(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            Ticket? ticket = await _context.Tickets
                .Include(p => p.Developers)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        private bool ProjectExists(int id)
        {
          return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}