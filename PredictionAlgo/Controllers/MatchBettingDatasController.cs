using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using PredictionAlgo.Models;
using PredictionAlgo.Models.DataModel;


namespace PredictionAlgo.Controllers
{
    public class MatchBettingDatasController : Controller
    {
        private readonly PredictionAlgoContext _db = new PredictionAlgoContext();

        // GET: MatchBettingDatas
        public ActionResult Index()
        {
            var bettingData = _db.MatchBettingDatas
                .DistinctBy(x => x.FixtureReference)
                .OrderByDescending(x => x.FixtureDate);

            return View(bettingData);
        }


        public ActionResult GetBettingData()
        {
            var bettingData = new BettingData();
            var pro12Data = bettingData.GetCurrentBettingData;

            if (!pro12Data.Any()) ViewData["NoData"] = "Currently no betting odds available online";


            return View(pro12Data.ToList());
        }


        public ActionResult SampleBetData()
        {
            var pro12Data = _db.MatchBettingDatas
                 .Where(x => x.FixtureDate > new DateTime(2016, 10, 27) && x.FixtureDate < new DateTime(2016, 10, 30))
                 .OrderBy(x => x.FixtureDate)
                 .ToList();

            return View(pro12Data.ToList());
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
