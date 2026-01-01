using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ResidentialHallManagement.Core.Interfaces;
using ResidentialHallManagement.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResidentialHallManagement.Web.Controllers
{
    public class HallController : Controller
    {
        private readonly IHallService _hallService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HallController(IHallService hallService, UserManager<ApplicationUser> userManager)
        {
            _hallService = hallService;
            _userManager = userManager;
        }

        private bool IsSystemAdmin()
        {
            var role = HttpContext.Session.GetString("AdminRole");
            return role == "SystemAdmin";
        }

        private bool IsHallAdminIdentity()
        {
            return User.IsInRole("HallAdmin");
        }

        // GET: Hall
        public async Task<IActionResult> Index()
        {
            // Check session-based auth for System Admin
            if (IsSystemAdmin())
            {
                var halls = await _hallService.GetAllHallsAsync();
                return View(halls);
            }

            // Check Identity-based auth for Hall Admin
            if (IsHallAdminIdentity())
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null || user.HallId == null) return Forbid();
                var hall = await _hallService.GetHallByIdAsync(user.HallId.Value);
                if (hall == null) return NotFound();
                return View(new List<Hall> { hall });
            }

            return RedirectToAction("Login", "Account");
        }

        // GET: Hall/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // Check session-based auth for System Admin
            if (!IsSystemAdmin() && !IsHallAdminIdentity())
            {
                return RedirectToAction("Login", "Account");
            }

            var hall = await _hallService.GetHallByIdAsync(id);
            if (hall == null)
            {
                return NotFound();
            }

            if (IsHallAdminIdentity())
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null || user.HallId != id) return Forbid();
            }

            return View(hall);
        }

        // GET: Hall/Create
        public IActionResult Create()
        {
            if (!IsSystemAdmin())
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // POST: Hall/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hall hall)
        {
            if (!IsSystemAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                await _hallService.CreateHallAsync(hall);
                TempData["Success"] = "Hall created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(hall);
        }

        // GET: Hall/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsSystemAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var hall = await _hallService.GetHallByIdAsync(id);
            if (hall == null)
            {
                return NotFound();
            }
            return View(hall);
        }

        // POST: Hall/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Hall hall)
        {
            if (!IsSystemAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != hall.HallId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _hallService.UpdateHallAsync(hall);
                TempData["Success"] = "Hall updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(hall);
        }

        // GET: Hall/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsSystemAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var hall = await _hallService.GetHallByIdAsync(id);
            if (hall == null)
            {
                return NotFound();
            }
            return View(hall);
        }

        // POST: Hall/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsSystemAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            await _hallService.DeleteHallAsync(id);
            TempData["Success"] = "Hall deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Hall/Search
        public async Task<IActionResult> Search(string? name, string? type)
        {
            if (!IsSystemAdmin() && !IsHallAdminIdentity())
            {
                return RedirectToAction("Login", "Account");
            }

            if (IsHallAdminIdentity())
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null || user.HallId == null) return Forbid();
                var hall = await _hallService.GetHallByIdAsync(user.HallId.Value);
                if (hall == null) return NotFound();
                return View("Index", new List<Hall> { hall });
            }

            var halls = await _hallService.SearchHallsAsync(name, type);
            return View("Index", halls);
        }
    }
}
