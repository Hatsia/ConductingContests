﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConductingContests.Data;
using ConductingContests.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ConductingContests.Controllers
{
    public class OfferedServicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public OfferedServicesController(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: OfferedServices
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.OfferedServices.Include(o => o.Contest).Include(o => o.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: OfferedServices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offeredService = await _context.OfferedServices
                .Include(o => o.Contest)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (offeredService == null)
            {
                return NotFound();
            }

            return View(offeredService);
        }

        // GET: OfferedServices/Create
        public IActionResult Create(int id)
        {
            ViewData["ContestId"] = id;
            ViewData["UserId"] = _userManager.GetUserId(User);
            return View();
        }

        // POST: OfferedServices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OfferedService offeredService, [FromForm] IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0 && file.Length < 10 * 1024 * 1024)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/userFiles", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    offeredService.FilePath = "/userFiles/" + fileName;
                    offeredService.Contest = _context.Contests.Find(offeredService.ContestId);
                    offeredService.User = _context.Users.Find(offeredService.UserId);
                    offeredService.Id = 0;
                    _context.Add(offeredService);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Handle invalid file size here
                    ViewData["FileSize"] = "Choose the correct file, filesize should be lower than 10 Mb";
                    return View();
                }
            }

            ViewData["ContestId"] = new SelectList(_context.Contests, "Id", "Id", offeredService.ContestId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", offeredService.UserId);

            return View(offeredService);
        }

        // GET: OfferedServices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offeredService = await _context.OfferedServices.FindAsync(id);
            if (offeredService == null)
            {
                return NotFound();
            }
            ViewData["ContestId"] = new SelectList(_context.Contests, "Id", "Id", offeredService.ContestId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", offeredService.UserId);
            return View(offeredService);
        }

        

        // POST: OfferedServices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,FilePath,Description,UserId,ContestId")] OfferedService offeredService)
        {
            if (id != offeredService.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(offeredService);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OfferedServiceExists(offeredService.Id))
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
            ViewData["ContestId"] = new SelectList(_context.Contests, "Id", "Id", offeredService.ContestId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", offeredService.UserId);
            return View(offeredService);
        }

        // GET: OfferedServices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offeredService = await _context.OfferedServices
                .Include(o => o.Contest)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (offeredService == null)
            {
                return NotFound();
            }

            return View(offeredService);
        }

        // POST: OfferedServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var offeredService = await _context.OfferedServices.FindAsync(id);
            _context.OfferedServices.Remove(offeredService);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OfferedServiceExists(int id)
        {
            return _context.OfferedServices.Any(e => e.Id == id);
        }
    }
}
