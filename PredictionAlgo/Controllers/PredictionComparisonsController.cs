﻿using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PredictionAlgo.Models;
using PredictionAlgo.Models.DataModel;

namespace PredictionAlgo.Controllers
{
    public class PredictionComparisonsController : Controller
    {
        private readonly PredictionAlgoContext _db = new PredictionAlgoContext();
        private readonly PredictionComparisonData _predictCompare = new PredictionComparisonData();

        // GET: PredictionComparisons
        public ActionResult Index()
        {
            _predictCompare.AddPredictionComparisonsToFile(_db.MatchBettingDatas.ToList());

            ViewData["SuccessRate"] = _predictCompare.GetTotalPreditionSuccess;

           // ViewData["DataAvailable"] = 
            return View(_db.PredictionComparisons.ToList().OrderByDescending(x=>x.FixtureDate));
        }

        public ActionResult AllPreviousComparisons()
        {
            var predict = new PredictionComparisonData();

            ViewData["SuccessRate"] = _predictCompare.GetTotalPreditionSuccess;

            return View(predict.GetAllPredictionComparisons(_db).OrderByDescending(x => x.FixtureDate));
        }
        // GET: PredictionComparisons/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var predictionComparison = _db.PredictionComparisons.Find(id);
            if (predictionComparison == null)
            {
                return HttpNotFound();
            }
            return View(predictionComparison);
        }

        // GET: PredictionComparisons/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PredictionComparisons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PredictionComparisonReference,AlgoScoreSpreadPrediction,BookScoreSpreadPrediction,ActualScoreSpreadPrediction,TeamToBack,SwerveTeam,PredictionResult")] PredictionComparison predictionComparison)
        {
            if (ModelState.IsValid)
            {
                _db.PredictionComparisons.Add(predictionComparison);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(predictionComparison);
        }

        // GET: PredictionComparisons/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PredictionComparison predictionComparison = _db.PredictionComparisons.Find(id);
            if (predictionComparison == null)
            {
                return HttpNotFound();
            }
            return View(predictionComparison);
        }

        // POST: PredictionComparisons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PredictionComparisonReference,AlgoScoreSpreadPrediction,BookScoreSpreadPrediction,ActualScoreSpreadPrediction,TeamToBack,SwerveTeam,PredictionResult")] PredictionComparison predictionComparison)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(predictionComparison).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(predictionComparison);
        }

        // GET: PredictionComparisons/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PredictionComparison predictionComparison = _db.PredictionComparisons.Find(id);
            if (predictionComparison == null)
            {
                return HttpNotFound();
            }
            return View(predictionComparison);
        }

        // POST: PredictionComparisons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PredictionComparison predictionComparison = _db.PredictionComparisons.Find(id);
            _db.PredictionComparisons.Remove(predictionComparison);
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
