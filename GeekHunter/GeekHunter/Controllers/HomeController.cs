using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GeekHunter.Models;
using GeekHunter.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeekHunter.Controllers
{
    public class HomeController : Controller
    {
        private readonly GeekHuntersDbContext _db;

        public HomeController(GeekHuntersDbContext db)
        {
            _db = db;
        }

        public string FirstNameSort { get; set; }
        public string LastNameSort { get; set; }
        public string SkillSort { get; set; }

        // GET: Candidate/
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchSkill)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.FirstNameSort = String.IsNullOrEmpty(sortOrder) ? "firstName_desc" : "";
            ViewBag.LastNameSort = String.IsNullOrEmpty(sortOrder) ? "lastName_desc" : "";

            if (searchSkill == null)
                searchSkill = currentFilter;

            ViewBag.CurrentFilter = searchSkill;

            var query = await _db.Candidates
                .Include(p => p.Skills)
                .ThenInclude(s => s.Skill)
                .ToListAsync();

            if (!String.IsNullOrEmpty(searchSkill))
            {
                query = query.Where(p => p.Skills.Any(t => CultureInfo.CurrentCulture.CompareInfo.IndexOf(t.Skill.Name, searchSkill, CompareOptions.IgnoreCase) >= 0)).ToList();
            }

            switch (sortOrder)
            {
                case "firstName_desc":
                    query = query.OrderByDescending(c => c.FirstName).ToList();
                    break;
                case "lastName_desc":
                    query = query.OrderByDescending(c => c.LastName).ToList();
                    break;
                default:
                    query = query.OrderBy(c => c.FirstName).ToList();
                    break;
            }

            return View(query);
        }

        // GET: Candidate/Register
        public async Task<IActionResult> Register()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var skills = await _db.Skills.ToListAsync();
                    ViewBag.skills = skills;
                }
            }
            catch (DataException dex)
            {
                return Content(dex.Message);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(CandidateSkillViewModel candidateSkill)
        {
            try
            {
                // Repopulate ViewBag to avoid NullException
                var skills = await _db.Skills.ToListAsync();
                ViewBag.skills = skills;

                if (candidateSkill == null)
                    NotFound();

                if (ModelState.IsValid)
                {
                    await _db.AddAsync(candidateSkill);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    return View();
                }
            }
            catch (DataException dex)
            {
                return Content(dex.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Candidate/Delete?candidateId=5
        public async Task<IActionResult> Delete(int? candidateId)
        {
            if (candidateId == null)
                return NotFound();

            var candidate =  await _db.Candidates.SingleOrDefaultAsync(c => c.Id == candidateId);
            if (candidate == null)
                return NotFound();

            return View(candidate);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var candidate = await _db.Candidates.SingleOrDefaultAsync(c => c.Id == id);
                    //var candidateSkill = _db.CandidateSkills.Where(c => c.CandidateId == id).Select(c => c.CandidateId);

                    if (candidate == null)
                        return NotFound();

                    _db.Remove(candidate);
                    //_db.Remove(candidateSkill);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    return View();
                }
            }
            catch (DataException dex)
            {
                return Content(dex.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _db.Dispose();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
