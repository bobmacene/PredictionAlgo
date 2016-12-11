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
        public ActionResult Index(string id)
        {
            //new AzureData().UpdateAzure();   only required if updating Azure tables from C#_App

            var bettingData = _db.MatchBettingDatas
                .DistinctBy(x => x.FixtureReference)
                .OrderByDescending(x => x.FixtureDate);

            if (id == null) return View(bettingData);

            ViewData["CsvExport"] = "Fixtures have been saved to C:\\Users\\TEMP";

            var bet = new BettingData();
            bet.SaveCsv<MatchBettingData>(bettingData, "PreviousBettingData");

            return View(bettingData);
        }


        public ActionResult GetBettingData(string id)
        {
            var bettingData = new BettingData();
            var pro12Data = bettingData.GetCurrentBettingData;

            if (!pro12Data.Any()) ViewData["NoData"] = "Currently no betting odds available online";

            if (id == null) return View(pro12Data.ToList());

            ViewData["CsvExport"] = "Fixtures have been saved to C:\\Users\\TEMP";

            var bet = new BettingData();
            bet.SaveCsv<MatchBettingData>(pro12Data, "FutureBettingData");

            return View(pro12Data.ToList());
        }


        public ActionResult TestDataBetData(string id)
        {
            var pro12Data = _db.MatchBettingDatas
                 .Where(x => x.FixtureDate > new DateTime(2016, 10, 27) && x.FixtureDate < new DateTime(2016, 10, 30))
                 .OrderBy(x => x.FixtureDate)
                 .ToList();

            if (id == null) return View(pro12Data.ToList());

            @ViewData["CsvExport"] = "Fixtures have been saved to C:\\Users\\TEMP";

            var bet = new BettingData();
            bet.SaveCsv<MatchBettingData>(pro12Data, "FutureBettingData");

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
