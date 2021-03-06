﻿using System;
using System.Collections.Generic;
using PredictionAlgo.Models.ViewModel;


namespace PredictionAlgo.Models.DataModel
{
    public class BettingData : CommonFunctions
    {
        #region filepaths
        //private const string HCapCouponPath = "file://C:/Users/bob/Documents/" + "ITT Project/BettingHtml/2016.10.25_HCapCoupon - Paddy Power.html";
        //private const string HCapCouponPath = @"file://C:/Users/rip/Documents/ITT/Project/2016.10.07_Rugby H'Cap Betting from Paddy Power.html";
        #endregion

        private const string HCapCouponPath = @"http://www.paddypower.com/bet/rugby-union/rugby-hcap-coupon";
        private readonly string _uriHCapCoupon = new Uri(HCapCouponPath).AbsoluteUri;


        public IEnumerable<MatchBettingData> MatchBettingDataList { get; set; }
        private readonly PredictionAlgoContext _db = new PredictionAlgoContext();


        public IEnumerable<MatchBettingData> GetCurrentBettingData
        {
            get
            {
                var webScraper = new WebScraper();
                MatchBettingDataList = webScraper.GetMatchBettingData(webScraper.GetSpreadsAndOdds(_uriHCapCoupon));

                foreach (var bettingData in MatchBettingDataList)
                {
                    bettingData.FixtureReference = GetFixtureReference(bettingData.HomeTeam, bettingData.FixtureDate);
                    bettingData.MatchDataReference = GetFixtureReference(bettingData.HomeTeam, bettingData.FixtureDate);
                    bettingData.TimeStamp = DateTime.Now;
                    bettingData.NoOddsAvailableText = string.Empty;

                    if(_db.MatchBettingDatas.Find(bettingData.FixtureReference) != null)
                    {
                        var bettingDataToEdit = _db.MatchBettingDatas.Find(bettingData.FixtureReference);
                        _db.MatchBettingDatas.Remove(bettingDataToEdit);
                        _db.MatchBettingDatas.Add(bettingData);
                    }
                    else
                    {
                        _db.MatchBettingDatas.Add(bettingData);
                    } 
                }
                _db.SaveChanges();
                return MatchBettingDataList;
            }
        }

    }
}


#region FilePaths
//private const string Pro12Url = @"http://www.paddypower.com/bet/rugby-union/guinness-pro-12?ev_oc_grp_ids=30998";
// private const string Top14Url = @"http://www.paddypower.com/bet/rugby-union/french-top-14?ev_oc_grp_ids=31127";
//private const string Top14Url1 = @"http://www.paddypower.com/bet/rugby-union/french-top-14?ev_oc_grp_ids=31127#link_11132866";
// private const string Handicaps = "http://www.paddypower.com/bet/rugby-union/rugby-hcap-coupon";
// private const string TestTop14 = "file:///C:/Users/rip/Documents/ITT/Project/BettingHtml&Excel/2016.08.16_French Top 14 Betting Odds - Paddy Power.html";
//2016.08.16_French Top 14 Betting.html";      2016.08.28_Pro12 - Paddy Power.html
//private string localPathTop14 = new Uri(TestTop14).LocalPath;
//"file:///C:/Users/rip/Documents/ITT/Project/BettingHtml&Excel/2016.08.16_French Top 14 Betting " 
#endregion
