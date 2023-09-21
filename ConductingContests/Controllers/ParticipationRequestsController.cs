using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConductingContests.Data;
using ConductingContests.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace ConductingContests.Controllers
{
    public class ParticipationRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ParticipationRequestsController(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //// GET: api/ParticipationRequests
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<ParticipationRequest>>> GetParticipationRequests()
        //{
        //    return await _context.ParticipationRequests.ToListAsync();
        //}

        //// GET: api/ParticipationRequests/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<ParticipationRequest>> GetParticipationRequest(int id)
        //{
        //    var participationRequest = await _context.ParticipationRequests.FindAsync(id);

        //    if (participationRequest == null)
        //    {
        //        return NotFound();
        //    }

        //    return participationRequest;
        //}

        [HttpPost]
        public async Task<IActionResult> Apply(int id)
        {
            var contest = await _context.Contests.FindAsync(id);
            if (contest == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var currentUser = await _userManager.GetUserAsync(User);
            ParticipationRequest request = null;

            if (contest.ParticipationRequest == null || contest.ParticipationRequest.FirstOrDefault(r => r.UserId == userId) == null)
            {
                request = new ParticipationRequest
                {
                    SubmissionDate = DateTime.Now,
                    Status = StatusRequest.Pending,
                    //UserId = userId,
                    User = currentUser,
                    //ContestId = contest.Id,
                    Contest = contest
                };
                _context.ParticipationRequests.Add(request);
            }
            else if (request.Status == StatusRequest.Pending)
            {
                request.Status = null; // отменяем заявку
            }
            else
            {
                // заявка уже принята или отклонена
                return RedirectToAction("Index", "Contests");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Contests");
        }

        [HttpPost]
        public async Task<IActionResult> ApproveRequest(int id, int modelId)
        {
            var currentRequest = await _context.ParticipationRequests.FindAsync(id);
            if(currentRequest == null)
            {
                return NotFound();
            }

            currentRequest.SubmissionDate = DateTime.Now;
            currentRequest.Status = StatusRequest.Accepted;

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Contests", new { id = modelId });
        }

        [HttpPost]
        public async Task<IActionResult> RejectRequest(int id, int modelId)
        {
            var currentRequest = await _context.ParticipationRequests.FindAsync(id);
            if (currentRequest == null)
            {
                return NotFound();
            }

            currentRequest.SubmissionDate = DateTime.Now;
            currentRequest.Status = StatusRequest.Rejected;

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Contests", new { id = modelId });
        }


        //// PUT: api/ParticipationRequests/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutParticipationRequest(int id, ParticipationRequest participationRequest)
        //{
        //    if (id != participationRequest.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(participationRequest).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ParticipationRequestExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/ParticipationRequests
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<ParticipationRequest>> PostParticipationRequest(ParticipationRequest participationRequest)
        //{
        //    _context.ParticipationRequests.Add(participationRequest);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetParticipationRequest", new { id = participationRequest.Id }, participationRequest);
        //}

        //// DELETE: api/ParticipationRequests/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteParticipationRequest(int id)
        //{
        //    var participationRequest = await _context.ParticipationRequests.FindAsync(id);
        //    if (participationRequest == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.ParticipationRequests.Remove(participationRequest);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool ParticipationRequestExists(int id)
        //{
        //    return _context.ParticipationRequests.Any(e => e.Id == id);
        //}
    }
}
