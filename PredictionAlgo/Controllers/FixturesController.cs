using System.Linq;
using System.Web.Mvc;
using PredictionAlgo.Models;
using PredictionAlgo.Models.ViewModel;
using PredictionAlgo.Models.DataModel;

namespace PredictionAlgo.Controllers
{
    public class FixturesController : Controller
    {
        private readonly PredictionAlgoContext _db = new PredictionAlgoContext();
       

        public ActionResult ResultsSeason2010_2011(string id)
        {
            var results = new ResultsSeason2010_2011();
            var orderedResults = results.GetSeasonResults(_db).ToList().OrderByDescending(x => x.FixtureDate);

            _db.SaveChanges();

            ViewData["ResultsWithoutSpreadSuccessRate"] = new FixtureData().GetResultsWithoutSpreadsPredictionSuccessRate;

            if (id == null) return View(orderedResults);

            ViewData["CsvExport"] = "Fixtures have been saved to C:\\Users\\TEMP";

            var bet = new BettingData();
            bet.SaveCsv<Fixture>(orderedResults, "Fixtures");

            return View(orderedResults);
        }

        public ActionResult ResultsSeason2011_2012(string id)
        {
            var results = new ResultsSeason2011_2012();
            var orderedResults = results.GetSeasonResults(_db).ToList().OrderByDescending(x => x.FixtureDate);

            _db.SaveChanges();

            ViewData["ResultsWithoutSpreadSuccessRate"] = new FixtureData().GetResultsWithoutSpreadsPredictionSuccessRate;

            if (id == null) return View(orderedResults);

            ViewData["CsvExport"] = "Fixtures have been saved to C:\\Users\\TEMP";

            var bet = new BettingData();
            bet.SaveCsv<Fixture>(orderedResults, "Fixtures");

            return View(orderedResults);
        }

        public ActionResult ResultsSeason2012_2013(string id)
        {
            var results = new ResultsSeason2012_2013();
            var orderedResults = results.GetSeasonResults(_db).ToList().OrderByDescending(x => x.FixtureDate);

            _db.SaveChanges();

            ViewData["ResultsWithoutSpreadSuccessRate"] = new FixtureData().GetResultsWithoutSpreadsPredictionSuccessRate;

            if (id == null) return View(orderedResults);

            ViewData["CsvExport"] = "Fixtures have been saved to C:\\Users\\TEMP";

            var bet = new BettingData();
            bet.SaveCsv<Fixture>(orderedResults, "Fixtures");

            return View(orderedResults);
        }
        public ActionResult ResultsSeason2013_2014(string id)
        {
            var results = new ResultsSeason2013_2014();
            var orderedResults = results.GetSeasonResults(_db).ToList().OrderByDescending(x => x.FixtureDate);

            _db.SaveChanges();

            ViewData["ResultsWithoutSpreadSuccessRate"] = new FixtureData().GetResultsWithoutSpreadsPredictionSuccessRate;

            if (id == null) return View(orderedResults);

            ViewData["CsvExport"] = "Fixtures have been saved to C:\\Users\\TEMP";

            var bet = new BettingData();
            bet.SaveCsv<Fixture>(orderedResults, "Fixtures");

            return View(orderedResults);
        }

        public ActionResult ResultsSeason2014_2015(string id)
        {
            var results = new ResultsSeason2014_2015 ();
            var orderedResults = results.GetSeasonResults(_db).ToList().OrderByDescending(x => x.FixtureDate);

            _db.SaveChanges();

            ViewData["ResultsWithoutSpreadSuccessRate"] = new FixtureData().GetResultsWithoutSpreadsPredictionSuccessRate;

            if (id == null) return View(orderedResults);

            ViewData["CsvExport"] = "Fixtures have been saved to C:\\Users\\TEMP";

            var bet = new BettingData();
            bet.SaveCsv<Fixture>(orderedResults, "Fixtures");

            return View(orderedResults);
        }
        public ActionResult ResultsSeason2015_2016(string id)
        {
            var results = new ResultsSeason2015_2016();
            var orderedResults = results.GetSeasonResults(_db).ToList().OrderByDescending(x => x.FixtureDate);

            _db.SaveChanges();

            ViewData["ResultsWithoutSpreadSuccessRate"] = new FixtureData().GetResultsWithoutSpreadsPredictionSuccessRate;

            if (id == null) return View(orderedResults);

            ViewData["CsvExport"] = "Fixtures have been saved to C:\\Users\\TEMP";

            var bet = new BettingData();
            bet.SaveCsv<Fixture>(orderedResults, "Fixtures");

            return View(orderedResults);
        }

        public ActionResult ResultsSeason2016_2017(string id)
        {
            var fixtureData = new FixtureData();

            if (id == "updateFixtures") fixtureData.UpdateFixtureDataSet();

            if (id == "getRecentResults")
            {
                var scrapedFixtures = fixtureData.GetFixturesAndResults;

                ViewData["ResultsWithoutSpreadSuccessRate"] = new FixtureData().GetResultsWithoutSpreadsPredictionSuccessRate;

                return View(scrapedFixtures.ToList().OrderByDescending(x => x.FixtureDate));
            }

            var results = new ResultsSeason2016_2017();
            var orderedResults = results.GetSeasonResults(_db).ToList().OrderByDescending(x => x.FixtureDate);

            ViewData["ResultsWithoutSpreadSuccessRate"] = new FixtureData().GetResultsWithoutSpreadsPredictionSuccessRate;

            if (id == "id")
            {
                ViewData["CsvExport"] = "Fixtures have been saved to C:\\Users\\TEMP";

                var bet = new BettingData();
                bet.SaveCsv<Fixture>(orderedResults, "Fixtures");
            }

            return View(orderedResults);
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
