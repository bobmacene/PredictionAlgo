using System;
using System.Linq;
using PredictionAlgo.Models;
using PredictionAlgo.Models.DataModel;
using System.Web.Mvc;

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


            if (id == null) return View(predictions);


            ViewData["CsvExport"] = "Fixtures have been saved to C:\\Users\\TEMP";


            var bet = new BettingData();
            bet.SaveCsv<PredictionComparison>(predictions, "PreviousBettingData");

            return View(predictions);
        }


        public ActionResult TestDataComparisons(string id)
        {
            var upcomingFixturesWithBettingData = _db.MatchBettingDatas
                .Where(x => x.FixtureDate > new DateTime(2016,10,27) && x.FixtureDate < new DateTime(2016, 10, 30))
                .OrderBy(x => x.FixtureDate)
                .ToList();

            var predictions =  _predictCompare.GetPredictionComparisons(upcomingFixturesWithBettingData);

            if (id == null) return View(predictions);

            ViewData["CsvExport"] = "Fixtures have been saved to C:\\Users\\TEMP";

            var bet = new BettingData();
            bet.SaveCsv<PredictionComparison>(predictions, "PreviousBettingData");

            return View(predictions);
        }

        public ActionResult AllPreviousComparisons(string id)
        {
            using (var predict = new PredictionComparisonData())
            {

                #region RequiredWhenMatchBettingDatasUpdatedWithHistoricalData

                //    _predictCompare.GetPredictionComparisons(_db.MatchBettingDatas);
                //    _predictCompare.UpdateDatabase();
                
                #endregion

                var allPredictions = predict.GetAllPredictionComparisons(_db).OrderByDescending(x => x.FixtureDate);

                ViewData["SuccessRate"] = _predictCompare.GetTotalPreditionSuccess;

                if (id == null) return View(allPredictions);

                ViewData["CsvExport"] = "Fixtures have been saved to C:\\Users\\TEMP";

                var bet = new BettingData();
                bet.SaveCsv<PredictionComparison>(allPredictions, "PreviousBettingData");

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
