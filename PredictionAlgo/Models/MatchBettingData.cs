using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PredictionAlgo.Models.ViewModel;

namespace PredictionAlgo.Models
{
    public class MatchBettingData
    { 
        [Key]
        public string MatchDataReference { get; set; }
        [DisplayName("Home Team")]
        public Team HomeTeam { get; set; }
        [DisplayName("Home Spread")]
        public double HomeSpread { get; set; }
        [DisplayName("Home Team Odds")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal HomeTeamOdds { get; set; }
        [DisplayName("Away Team")]
        public Team? AwayTeam { get; set; }
        [DisplayName("Away Spread")]
        public double AwaySpread { get; set; }
        [DisplayName("Away Team Odds")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal AwayTeamOdds { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy MM dd}")]
        [DataType(DataType.Date)]
        [DisplayName("Fixture Date")]
        public DateTime? FixtureDate { get; set; }
        public DateTime TimeStamp { get; set; }      
        public string FixtureReference { get; set; } //common reference
        public string NoOddsAvailableText { get; set; }

        public MatchBettingData(WebScraper.OddsSpreads oddsSpreads)
        {
            HomeTeam = (Team)Enum.Parse(typeof(Team), oddsSpreads.HomeTeam);
            HomeSpread = Convert.ToDouble(oddsSpreads.HomeSpread);
            HomeTeamOdds = Convert.ToDecimal(oddsSpreads.HomeTeamDecimalOdds);
            AwayTeam = (Team)Enum.Parse(typeof(Team), oddsSpreads.AwayTeam);
            AwaySpread = Convert.ToDouble(oddsSpreads.AwaySpread);
            AwayTeamOdds = Convert.ToDecimal(oddsSpreads.AwayTeamDecimalOdds);
            FixtureDate = oddsSpreads.Date;
            TimeStamp = DateTime.Now;
        }
        public MatchBettingData()
        {
            HomeTeam = Team.None;
            HomeSpread = 0;
            HomeTeamOdds = 0;
            AwayTeam = Team.None;
            AwaySpread = 0;
            AwayTeamOdds = 0;
            FixtureDate = null;
            TimeStamp = DateTime.Now;
        }
    }
}
#region Top14

//public List<MatchBettingData> GetMatchBettingDataTop14Test
//{
//    get
//    {
//        return matchBettingDataList = webScraper.GetMatchBettingData(webScraper.GetSpreadsAndOdds(localPathHCapCoupon),
//            webScraper.GetMatchDates(localPathHCapCoupon, Competition.Mixed));
//    }
//}

//public List<MatchBettingData> GetMatchBettingDataTop14
//{
//    get
//    {
//        return matchBettingDataList = webScraper.GetMatchBettingData(webScraper.GetSpreadsAndOdds(Top14Url), 
//            webScraper.GetMatchDates(Top14Url, Competition.Top14));
//    }
//}
//public List<MatchBettingData> GetMatchBettingDataTop14_1
//{
//    get
//    {
//        return matchBettingDataList = webScraper.GetMatchBettingData(webScraper.GetSpreadsAndOdds(Top14Url),
//            webScraper.GetMatchDates(Top14Url, Competition.Top14));
//    }
//}

//public List<MatchBettingData> GetMatchBettingDataPro12
//{
//    get
//    {
//        return matchBettingDataList = webScraper.GetMatchBettingData(webScraper.GetSpreadsAndOdds(Pro12Url),
//            webScraper.GetMatchDates(Pro12Url, Competition.Pro12));
//    }
//} 
#endregion