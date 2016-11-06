using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using PredictionAlgo.Models;

namespace PredictionAlgo.Controllers
{
    public class ResultStatisticsController : Controller
    {
        private PredictionAlgoContext db = new PredictionAlgoContext();

        // GET: ResultStatistics     
        public ActionResult Index()
        {
            var resultsStats = new ResultStatistics();
            return View(resultsStats);
        }
       

        // GET: ResultStatistics/Details/5
        public ActionResult PredictResult()
        {
            return View();
        }

        // POST: ResultStatistics/Create
        [HttpPost]
        public ActionResult PredictResult(ResultStatistics predictedResultStats)
        {
            if (ModelState.IsValid)
            {
                return View(predictedResultStats);
            }

            return View(predictedResultStats);
        }
       
        // GET: ResultStatistics/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ResultStatistics resultStatistics = db.Statistics.Find(id);
            if (resultStatistics == null)
            {
                return HttpNotFound();
            }
            return View(resultStatistics);
        }

        // POST: ResultStatistics/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StatisticReference,HomeTeam,AwayTeam,Date," +
            "AverageHomeScoreLastFiveHomeGames,AverageScoreDeltaLastFiveHomeGames,AverageAwayScoreLastFiveAwayGames," +
            "AverageScoreDeltaLastFiveAwayGames,AverageScoreDeltaOfLastTwoResultsBetweenTeams,PredictedScoreDelta")]
            ResultStatistics resultStatistics)
        {
            if (ModelState.IsValid)
            {
                db.Entry(resultStatistics).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(resultStatistics);
        }

        // GET: ResultStatistics/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ResultStatistics resultStatistics = db.Statistics.Find(id);
            if (resultStatistics == null)
            {
                return HttpNotFound();
            }
            return View(resultStatistics);
        }

        // POST: ResultStatistics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ResultStatistics resultStatistics = db.Statistics.Find(id);
            db.Statistics.Remove(resultStatistics);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
