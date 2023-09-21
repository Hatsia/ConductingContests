using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConductingContests.Data;
using ConductingContests.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System;

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

        [HttpPost]
        public async Task<IActionResult> SelectWinner(int contestId, string winnerId)
        {
            var currentContest = await _context.Contests.FindAsync(contestId);
            var winnerUser = await _userManager.FindByIdAsync(winnerId);
            if (currentContest == null || winnerUser == null)
            {
                return NotFound();
            }

            currentContest.WinnerUserName = winnerUser.UserName;
            currentContest.Status = StatusContest.End;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Contests
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            IQueryable<Contest> currentContests;

            if (User.IsInRole("Organizer"))
            {
                currentContests = _context.Contests.Include(c => c.Category)
                                                   .Include(c => c.User)
                                                   .Where(x => x.UserId == currentUser.Id);
            }
            else if (User.IsInRole("Basic"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                currentContests = _context.Contests.Include(c => c.Category)
                                                   .Include(c => c.User)
                                                   .Include(c => c.ParticipationRequest)
                                                   .Include(x => x.OfferedService);
            }
            else
            {
                currentContests = _context.Contests.Include(c => c.Category).Include(c => c.User);
            }

            return View(await currentContests.ToListAsync());
        }

        // GET: Contests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contest = await _context.Contests
                .Include(c => c.ParticipationRequest).ThenInclude(u => u.User)
                .Include(c => c.OfferedService)
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
                ViewData["CategoryId"] = new SelectList(_context.ContestCategories, "Id", "Name");
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
            ViewData["CategoryId"] = new SelectList(_context.ContestCategories, "Id", "Name");

            // Получение списка значений enum statusContest
            var statusContestValues = Enum.GetValues(typeof(StatusContest)).Cast<StatusContest>();

            // Создание SelectList из значений enum
            var statusContestList = new SelectList(statusContestValues);

            // Сохранение SelectList в ViewData
            ViewData["StatusContestList"] = statusContestList;


            return View(contest);
        }

        // POST: Contests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,StartDate,EndDate,Status,WinnerUserName,CategoryId")] Contest contest)
        {
            if (id != contest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //То что закоменчено, то было для админа(При изменении сущности менялся UserId на админа).
                    //var currentUser = await _userManager.GetUserAsync(User);
                    //contest.UserId = currentUser.Id;
                    var contestDB = _context.Contests.FirstOrDefault(x => x.Id == contest.Id);
                    contestDB.Id = contest.Id;
                    contestDB.Title = contest.Title;
                    contestDB.Description = contest.Description;
                    contestDB.StartDate = contest.StartDate;
                    contestDB.EndDate = contest.EndDate;
                    contestDB.Status = contest.Status;
                    contestDB.WinnerUserName = contest.WinnerUserName;
                    contestDB.CategoryId = contest.CategoryId;

                    //_context.Update(contest);
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

        [HttpGet]
        public async Task<IActionResult> GetDetails(int contestId, string userName) 
        {
            var contest = await _context.Contests.FirstOrDefaultAsync(x => x.Id == contestId);

            if (contest.WinnerUserName == userName) 
            {
                // return PartialView("Вы победили!");
            }
            // вы проиграли
            return PartialView();
        }

        private bool ContestExists(int id)
        {
            return _context.Contests.Any(e => e.Id == id);
        }
    }
}
