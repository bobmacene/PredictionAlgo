using System;
using System.Web.Mvc;
using PredictionAlgo.Models;

namespace PredictionAlgo.Controllers
{
    public class ResultStatisticsController : Controller, IDisposable
    {
        private readonly PredictionAlgoContext _db = new PredictionAlgoContext();

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
            return View(predictedResultStats);
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
