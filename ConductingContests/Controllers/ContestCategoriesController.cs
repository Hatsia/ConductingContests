using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConductingContests.Data;
using ConductingContests.Models.Entities;

namespace ConductingContests.Controllers
{
    public class ContestCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContestCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ContestCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.ContestCategories.ToListAsync());
        }

        // GET: ContestCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contestCategory = await _context.ContestCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contestCategory == null)
            {
                return NotFound();
            }

            return View(contestCategory);
        }

        // GET: ContestCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ContestCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] ContestCategory contestCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contestCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contestCategory);
        }

        // GET: ContestCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contestCategory = await _context.ContestCategories.FindAsync(id);
            if (contestCategory == null)
            {
                return NotFound();
            }
            return View(contestCategory);
        }

        // POST: ContestCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] ContestCategory contestCategory)
        {
            if (id != contestCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contestCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContestCategoryExists(contestCategory.Id))
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
            return View(contestCategory);
        }

        // GET: ContestCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contestCategory = await _context.ContestCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contestCategory == null)
            {
                return NotFound();
            }

            return View(contestCategory);
        }

        // POST: ContestCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contestCategory = await _context.ContestCategories.FindAsync(id);
            _context.ContestCategories.Remove(contestCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContestCategoryExists(int id)
        {
            return _context.ContestCategories.Any(e => e.Id == id);
        }
    }
}
