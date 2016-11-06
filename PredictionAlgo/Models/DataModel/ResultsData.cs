using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PredictionAlgo.Models.DataModel
{
    public class ResultsData
    {
        private const string ResultsPagePro12 = @"http://www.pro12rugby.com/fixtures/";
        private string uriResultPro12 = new Uri(ResultsPagePro12).AbsoluteUri;
        public List<Fixture> ResultDataList { get; set; }
        private PredictionAlgoContext db = new PredictionAlgoContext();
        //public List<Fixture> GetResultData
        //{
        //    get
        //    {
        //        var webScraper = new WebScraper();
        //        ResultDataList = webScraper.GetMatchBettingData(webScraper.GetSpreadsAndOdds(uriResultPro12));
        //        var increment = 1;
        //        foreach (var resultData in ResultsDataList)
        //        {
        //            var time = DateTime.Now.GetHashCode() + ++increment;
        //            resultData.FixtureReference = time.ToString();
        //            time++;
        //            resultData.MatchDataReference = time.ToString();
        //            resultData.TimeStamp = DateTime.Now;
        //            db.Fixtures.Add(resultData);
        //        }
        //        db.SaveChanges();
        //        return ResultsDataList;
        //    }
        //}
    }
}

#region FilePaths
//private const string Pro12Url = @"http://www.paddypower.com/bet/rugby-union/guinness-pro-12?ev_oc_grp_ids=30998";
// private const string Top14Url = @"http://www.paddypower.com/bet/rugby-union/french-top-14?ev_oc_grp_ids=31127";
//private const string Top14Url1 = @"http://www.paddypower.com/bet/rugby-union/french-top-14?ev_oc_grp_ids=31127#link_11132866";
// private const string Handicaps = "http://www.paddypower.com/bet/rugby-union/rugby-hcap-coupon";
// private const string TestTop14 = "file:///C:/Users/rip/Documents/ITT/Project/BettingHtml&Excel/2016.08.16_French Top 14 Betting Odds - Paddy Power.html";
//2016.08.16_French%20Top%2014%20Betting.html";      2016.08.28_Pro12%20-%20Paddy%20Power.html
//private string localPathTop14 = new Uri(TestTop14).LocalPath;
//"file:///C:/Users/rip/Documents/ITT/Project/BettingHtml&Excel/2016.08.16_French%20Top%2014%20Betting%20" 
#endregion