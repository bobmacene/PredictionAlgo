using System;
using System.Linq;
using PredictionAlgo.Models;
using PredictionAlgo.Models.DataModel;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

namespace PredictionAlgo.Controllers
{
    public class PredictionComparisonsController : Controller
    {
        private readonly PredictionAlgoContext _db = new PredictionAlgoContext();
        private readonly PredictionComparisonData _predictCompare = new PredictionComparisonData();

        // GET: PredictionComparisons
        public ActionResult Index()
        {
            var upcomingFixturesWithBettingData = _db.MatchBettingDatas
                .Where(x => x.FixtureDate >= DateTime.Today)
                .OrderBy(x => x.FixtureDate)
                .ToList();


            ViewData["UpcomingFixtureAvailability"] = upcomingFixturesWithBettingData.Any() 
                ? string.Empty
                : "Currently no fixtures are available online";


            if (upcomingFixturesWithBettingData.Any())
            {
                _predictCompare.GetPredictionComparisons(upcomingFixturesWithBettingData);
                _predictCompare.UpdateDatabase();
            }


            var predictions = _db.PredictionComparisons
                .Where(x => x.FixtureDate >= DateTime.Today)
                .OrderBy(x => x.FixtureDate)
                .ToList();

            return View(predictions);
        }


        public ActionResult SampleDataComparisons()
        {
            var upcomingFixturesWithBettingData = _db.MatchBettingDatas
                .Where(x => x.FixtureDate > new DateTime(2016,10,27) && x.FixtureDate < new DateTime(2016, 10, 30))
                .OrderBy(x => x.FixtureDate)
                .ToList();

            var predictions =  _predictCompare.GetPredictionComparisons(upcomingFixturesWithBettingData);

            return View(predictions);
        }

        public ActionResult AllPreviousComparisons()
        {
            using (var predict = new PredictionComparisonData())
            {
                var allPredictions = _db.PredictionComparisons
                    .DistinctBy(x => x.PredictionComparisonReference)
                    .OrderByDescending(x => x.FixtureDate)
                    .ToList();

                ViewData["SuccessRate"] = _predictCompare.GetTotalPreditionSuccess;

                return View(allPredictions);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
            _db.Dispose();
        }
    }
}
