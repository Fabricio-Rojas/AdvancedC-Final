using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AdvancedC_Final.Controllers
{
    [Authorize(Roles = "Project Manager")]
    public class ManagerController : Controller
    {
        public IActionResult AddDevProject(int projectId)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDevProject(string[] devIds, int projectId)
        {
            return View();
        }

        public IActionResult AddDevTicket(int ticketId)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDevTicket(string[] devIds, int ticketId)
        {
            return View();
        }
    }
}
