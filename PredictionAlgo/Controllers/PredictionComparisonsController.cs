﻿using System;
using System.Linq;
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
        public ActionResult Index(string id)
        {
            var upcomingFixturesWithBettingData = _db.MatchBettingDatas
                .Where(x => x.FixtureDate >= DateTime.Today)
                .OrderBy(x => x.FixtureDate)
                .ToList();

            ViewData["UpcomingFixtureAvailability"] = upcomingFixturesWithBettingData.Any() 
                ? string.Empty
                : "Currently no fixtures are available";


            if (upcomingFixturesWithBettingData.Any())
                _predictCompare.AddPredictionComparisonsToFile(upcomingFixturesWithBettingData);

            var predictions = _db.PredictionComparisons
                .Where(x => x.FixtureDate >= DateTime.Today)
                .OrderBy(x => x.FixtureDate)
                .ToList();


            if (id == null) return View(predictions);

            @ViewData["CsvExport"] = "Fixtures have been saved to C:\\Users\\TEMP";


            var bet = new BettingData();
            bet.SaveCsv<PredictionComparison>(predictions, "PreviousBettingData");

            return View(predictions);
        }
        
        public ActionResult AllPreviousComparisons(string id)
        {
            var predict = new PredictionComparisonData();
            var allPredictions = predict.GetAllPredictionComparisons(_db).OrderByDescending(x => x.FixtureDate);

            ViewData["SuccessRate"] = _predictCompare.GetTotalPreditionSuccess;

            if (id == null) return View(allPredictions);

            @ViewData["CsvExport"] = "Fixtures have been saved to C:\\Users\\TEMP";

            var bet = new BettingData();
            bet.SaveCsv<PredictionComparison>(allPredictions, "PreviousBettingData");

            return View(allPredictions);
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
