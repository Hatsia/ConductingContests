using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConductingContests.Data;
using ConductingContests.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ConductingContests.Controllers
{
    public class ContestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ContestsController(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Contests
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Contests.Include(c => c.Category).Include(c => c.User);
            if (User.IsInRole("Organizer"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                applicationDbContext = _context.Contests.Where(x => x.User == currentUser).Include(c => c.Category).Include(c => c.User);
            }

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Contests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contest = await _context.Contests
                .Include(c => c.Category)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contest == null)
            {
                return NotFound();
            }

            return View(contest);
        }

        // GET: Contests/Create
        //[Authorize(Roles = "Admin, Organizer")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.ContestCategories, "Id", "Name");
            return View();
        }

        // POST: Contests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,StartDate,EndDate,CategoryId")] Contest contest)
        {
            var result = contest.EndDate.CompareTo(contest.StartDate);

            if (result == 0 || result < 0)
            {
                ViewData["StartDate"] = "Choose the correct date";
                ViewData["EndDate"] = "Change the date";
                ViewData["CategoryId"] = new SelectList(_context.ContestCategories, "Id", "Id", contest.CategoryId);
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", contest.UserId);
                
                return View(contest);
            }

            if (ModelState.IsValid)
            {
                contest.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                await _context.Contests.AddAsync(contest);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.ContestCategories, "Id", "Id", contest.CategoryId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", contest.UserId);

            return View(contest);
        }

        // GET: Contests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contest = await _context.Contests.FindAsync(id);
            if (contest == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.ContestCategories, "Id", "Id", contest.CategoryId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", contest.UserId);
            return View(contest);
        }

        // POST: Contests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,StartDate,EndDate,Status,WinnerUserName,UserId,CategoryId")] Contest contest)
        {
            if (id != contest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContestExists(contest.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.ContestCategories, "Id", "Id", contest.CategoryId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", contest.UserId);
            return View(contest);
        }

        // GET: Contests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contest = await _context.Contests
                .Include(c => c.Category)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contest == null)
            {
                return NotFound();
            }

            return View(contest);
        }

        // POST: Contests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contest = await _context.Contests.FindAsync(id);
            _context.Contests.Remove(contest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContestExists(int id)
        {
            return _context.Contests.Any(e => e.Id == id);
        }
    }
}
