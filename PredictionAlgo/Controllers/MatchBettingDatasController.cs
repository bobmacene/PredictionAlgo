﻿using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PredictionAlgo.Models;
using PredictionAlgo.Models.DataModel;
using PredictionAlgo.Models.ViewModel;

namespace PredictionAlgo.Controllers
{
    public class MatchBettingDatasController : Controller
    {
        private readonly PredictionAlgoContext _db = new PredictionAlgoContext();

        // GET: MatchBettingDatas
        public ActionResult Index()
        {
            foreach (var bet in _db.MatchBettingDatas)
            {
                var recordToEdit = _db.MatchBettingDatas.Find(bet.MatchDataReference);
                _db.MatchBettingDatas.Remove(recordToEdit);

                bet.FixtureReference =  bet.MatchDataReference = WebScraper.GetFixtureReference(bet.HomeTeam, bet.FixtureDate);

                if (!_db.MatchBettingDatas.Any(x => x.FixtureReference == bet.FixtureReference))
                {
                    _db.MatchBettingDatas.Add(bet);
                }
            }
            _db.SaveChanges();

            var bettingData = new BettingData();
            var pro12Data = bettingData.GetMatchBettingData;

            if (pro12Data.Count == 0) ViewData["NoData"] = "No currently betting odds available online";
            return View(pro12Data.ToList());
        }

        // GET: MatchBettingDatas/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MatchBettingData matchBettingData = _db.MatchBettingDatas.Find(id);
            if (matchBettingData == null)
            {
                return HttpNotFound();
            }
            return View(matchBettingData);
        }

        // GET: MatchBettingDatas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MatchBettingDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MatchDataReference,HomeTeam,HomeSpread,HomeTeamOdds,AwayTeam,AwaySpread,AwayTeamOdds,FixtureDate,TimeStamp,FixtureReference,NoOddsAvailableText")] MatchBettingData matchBettingData)
        {
            if (ModelState.IsValid)
            {
                _db.MatchBettingDatas.Add(matchBettingData);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(matchBettingData);
        }

        // GET: MatchBettingDatas/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MatchBettingData matchBettingData = _db.MatchBettingDatas.Find(id);
            if (matchBettingData == null)
            {
                return HttpNotFound();
            }
            return View(matchBettingData);
        }

        // POST: MatchBettingDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MatchDataReference,HomeTeam,HomeSpread,HomeTeamOdds,AwayTeam,AwaySpread,AwayTeamOdds,FixtureDate,TimeStamp,FixtureReference,NoOddsAvailableText")] MatchBettingData matchBettingData)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(matchBettingData).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(matchBettingData);
        }

        // GET: MatchBettingDatas/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MatchBettingData matchBettingData = _db.MatchBettingDatas.Find(id);
            if (matchBettingData == null)
            {
                return HttpNotFound();
            }
            return View(matchBettingData);
        }

        // POST: MatchBettingDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            MatchBettingData matchBettingData = _db.MatchBettingDatas.Find(id);
            _db.MatchBettingDatas.Remove(matchBettingData);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
