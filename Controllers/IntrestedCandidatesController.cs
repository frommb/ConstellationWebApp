﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConstellationWebApp.Data;
using ConstellationWebApp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ConstellationWebApp.Controllers
{
    [Authorize]
    public class IntrestedCandidatesController : Controller
    {
        private readonly ConstellationWebAppContext _context;

        public IntrestedCandidatesController(ConstellationWebAppContext context)
        {
            _context = context;
        }

        #region IntrestedCandidatesGets&PostsFunctions
        // GET: IntrestedCandidates
        public async Task<IActionResult> Index()
        {
            var viewModel = new ViewModel();
            viewModel.RecruiterPicks = _context.RecruiterPicks
                .Include(i => i.Recuiter)
                .Include(i => i.Candidate)
               .Include(i => i.Posting);


            viewModel.Postings = _context.Postings
                 .Include(i => i.PostingOwner)
                  .Include(i => i.Posting_PostingTypes)
                 .ThenInclude(i => i.PostingTypes);

            viewModel.intrestedCandidates = _context.IntrestedCandidate
               .Include(i => i.User)
               .Include(i => i.Posting);

            List<IntrestedCandidate> intrestData = _context.IntrestedCandidate.ToList();
            ViewBag.intrestData = intrestData;

            return View(viewModel);


        }


        // POST: IntrestedCandidate/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int postingID)
        {
            var currentUser = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUser != null && postingID != null)
            {
                IntrestedCandidate intrestedCandidate = new IntrestedCandidate();
                intrestedCandidate.UserID = currentUser;
                intrestedCandidate.PostingID = postingID;
                _context.Add(intrestedCandidate);
                await _context.SaveChangesAsync();
            }
            var returnPath = "../Postings/Details/" + postingID.ToString();
            return Redirect(returnPath);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePick(int postingID, string recruiterID, string candidateID)
        {
            var currentUser = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            RecruiterPicks thisRP = (_context.RecruiterPicks.Where(i => i.RecuiterID == recruiterID && i.CandidateID == candidateID && i.PostingID == postingID).FirstOrDefault());

            if (thisRP == null && currentUser == recruiterID)
            {
                RecruiterPicks recruiterPicks = new RecruiterPicks();
                recruiterPicks.RecuiterID = recruiterID;
                recruiterPicks.CandidateID = candidateID;
                recruiterPicks.PostingID = postingID;

                _context.Add(recruiterPicks);
                await _context.SaveChangesAsync();
            }
            var returnPath = "../Postings/Details/" + postingID.ToString();
            return Redirect(returnPath);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateManyPicks(int[] postings, string recruiterID, string candidateID)
        {
            var currentUser = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            foreach (var postingID in postings)
            {
                RecruiterPicks thisRP = (_context.RecruiterPicks.Where(i => i.RecuiterID == recruiterID && i.CandidateID == candidateID && i.PostingID == postingID).FirstOrDefault());
                if (thisRP == null && currentUser == recruiterID)
                {
                    RecruiterPicks recruiterPicks = new RecruiterPicks();
                    recruiterPicks.RecuiterID = recruiterID;
                    recruiterPicks.CandidateID = candidateID;
                    recruiterPicks.PostingID = postingID;

                    _context.Add(recruiterPicks);
                    await _context.SaveChangesAsync();
                }
            }
            var returnPath = "../Users/Details/" + candidateID.ToString();
            return Redirect(returnPath);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePick(int recuiterPicksID)
        {
            RecruiterPicks thisRP = (_context.RecruiterPicks.Where(i => i.RecuiterPicksID == recuiterPicksID).FirstOrDefault());

            var currentUser = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var returnPath = "../IntrestedCandidates";

            if (thisRP.RecuiterID == currentUser)
            {
                _context.RecruiterPicks.Remove(thisRP);
                await _context.SaveChangesAsync();
            }
            return Redirect(returnPath);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteManyPick(int[] recuiterPicks)
        {
            var returnPath = "";

            foreach (var recuiterPicksID in recuiterPicks)
            {
                RecruiterPicks thisRP = (_context.RecruiterPicks.Where(i => i.RecuiterPicksID == recuiterPicksID).FirstOrDefault());

                var currentUser = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (thisRP.RecuiterID == currentUser)
                {
                    _context.RecruiterPicks.Remove(thisRP);
                    await _context.SaveChangesAsync();
                }
                 returnPath = "../Users/Details/" + thisRP.CandidateID.ToString();
            }

            return Redirect(returnPath);
        }

        // POST: IntrestedCandidates/Delete/5
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int postingID, string returnUrl)
        {
            var currentUser = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            IntrestedCandidate thisIC = ((_context.IntrestedCandidate.Where(i => (i.PostingID == postingID) && (i.UserID == currentUser)).FirstOrDefault()));
            string detailReturnID = postingID.ToString();
            if (currentUser == thisIC.UserID)
            {
                _context.IntrestedCandidate.Remove(thisIC);
                await _context.SaveChangesAsync();
            }
            var returnPath = "../Postings/Details/" + postingID.ToString();
            return Redirect(returnPath);
        }

        private bool IntrestedCandidateExists(int id)
        {
            return _context.IntrestedCandidate.Any(e => e.IntrestedCandidateID == id);
        }
        #endregion
    }
}
