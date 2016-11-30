using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PredictionAlgo.Models;
using PredictionAlgo.Models.ViewModel;
using System;
using PredictionAlgo.Models.DataModel;

namespace PredictionAlgo.Controllers
{
    public class FixturesController : Controller
    {
        private readonly PredictionAlgoContext _db = new PredictionAlgoContext();
        [ChildActionOnly]
        public ActionResult PartialPage_LastFiveFixtures(Team? team, DateTime? date)
        {
            var previousFiveResults = new FixtureRanges();
            return PartialView(previousFiveResults.GetLastFiveFixturesByTeam(_db, team, date));
        }
        public ActionResult ResultsSeason2010_2011()
        {
            var results = new ResultsSeason2010_2011();
            var orderedResults = results.GetSeasonResults(_db).ToList().OrderByDescending(x => x.FixtureDate);

            @ViewData["ResultsWithoutSpreadSuccessRate"] = new FixtureData().GetResultsWithoutSpreadsPredictionSuccessRate;

            return View(orderedResults);
        }

        public ActionResult ResultsSeason2011_2012()
        {
            var results = new ResultsSeason2011_2012();
            var orderedResults = results.GetSeasonResults(_db).ToList().OrderByDescending(x => x.FixtureDate);

            @ViewData["ResultsWithoutSpreadSuccessRate"] = new FixtureData().GetResultsWithoutSpreadsPredictionSuccessRate;

            return View(orderedResults);
        }

        public ActionResult ResultsSeason2012_2013()
        {
            var results = new ResultsSeason2012_2013();
            var orderedResults = results.GetSeasonResults(_db).ToList().OrderByDescending(x => x.FixtureDate);

            @ViewData["ResultsWithoutSpreadSuccessRate"] = new FixtureData().GetResultsWithoutSpreadsPredictionSuccessRate;

            return View(orderedResults);
        }
        public ActionResult ResultsSeason2013_2014()
        {
            var results = new ResultsSeason2013_2014();
            var orderedResults = results.GetSeasonResults(_db).ToList().OrderByDescending(x => x.FixtureDate);

            @ViewData["ResultsWithoutSpreadSuccessRate"] = new FixtureData().GetResultsWithoutSpreadsPredictionSuccessRate;

            return View(orderedResults);
        }

        public ActionResult ResultsSeason2014_2015()
        {
            var results = new ResultsSeason2014_2015 ();
            var orderedResults = results.GetSeasonResults(_db).ToList().OrderByDescending(x => x.FixtureDate);

            @ViewData["ResultsWithoutSpreadSuccessRate"] = new FixtureData().GetResultsWithoutSpreadsPredictionSuccessRate;

            return View(orderedResults);
        }
        public ActionResult ResultsSeason2015_2016()
        {
            var results = new ResultsSeason2015_2016();
            var orderedResults = results.GetSeasonResults(_db).ToList().OrderByDescending(x => x.FixtureDate);

            @ViewData["ResultsWithoutSpreadSuccessRate"] = new FixtureData().GetResultsWithoutSpreadsPredictionSuccessRate;

            return View(orderedResults);
        }

        public ActionResult ResultsSeason2016_2017(string id)
        {
            var results = new ResultsSeason2016_2017();
            var orderedResults = results.GetSeasonResults(_db).ToList().OrderByDescending(x => x.FixtureDate);

            @ViewData["ResultsWithoutSpreadSuccessRate"] = new FixtureData().GetResultsWithoutSpreadsPredictionSuccessRate;

            if (id == null) return View(orderedResults);

            @ViewData["CsvExport"] = "Fixtures has been saved to C:\\Users\\TEMP";

            var bet = new BettingData();
            bet.SaveCsv<Fixture>(orderedResults, "Fixtures");

            return View(orderedResults);
        }

        // GET: Fixtures
        public ActionResult Index()
        {
            var fixtures = new FixtureData();
            var scrapedFixtures = fixtures.GetFixturesAndResults;
            //var scrapedFixtures = fixtures.GetIcalFixturesAndResults;
            //var currentSeasonResults = new ResultsSeason2016_2017().GetSeasonResults(_db);

            return View(scrapedFixtures.ToList().OrderByDescending(x=>x.FixtureDate));
        }

        // GET: Fixtures/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fixture fixture = _db.Fixtures.Find(id);
            if (fixture == null)
            {
                return HttpNotFound();
            }
            return View(fixture);
        }

        // GET: Fixtures/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Fixtures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FixtureReference,FixtureDate,result,HomeTeam,HomeScore,AwayTeam,AwayScore,ScoreDelta,Attendance,Venue,PredictedDelta,ActualVersusPredictedDelta,PredictedResult,PredictionSuccess")] Fixture fixture)
        {
            if (ModelState.IsValid)
            {
                _db.Fixtures.Add(fixture);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fixture);
        }

        // GET: Fixtures/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fixture fixture = _db.Fixtures.Find(id);
            if (fixture == null)
            {
                return HttpNotFound();
            }
            return View(fixture);
        }

        // POST: Fixtures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FixtureReference,FixtureDate,result,HomeTeam,HomeScore,AwayTeam,AwayScore,ScoreDelta,Attendance,Venue,PredictedDelta,ActualVersusPredictedDelta,PredictedResult,PredictionSuccess")] Fixture fixture)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(fixture).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fixture);
        }

        // GET: Fixtures/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fixture fixture = _db.Fixtures.Find(id);
            if (fixture == null)
            {
                return HttpNotFound();
            }
            return View(fixture);
        }

        // POST: Fixtures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Fixture fixture = _db.Fixtures.Find(id);
            _db.Fixtures.Remove(fixture);
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
