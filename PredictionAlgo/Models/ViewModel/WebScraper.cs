using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace PredictionAlgo.Models.ViewModel
{
    public class WebScraper
    {
          #region XPaths
        //private const string DateXpath = "//*[@id=\"main\"]//div//div//div//span";
        //private const string MatchXpath = "//*[@id=\"main\"]//table//tr//td//div//div"; 
        #endregion

        private const string Round = "Round";
        private const string MatchXpath = "//*[@id=\"main\"]//table//tr//td";
        private const string FixtureXpath = "//*[@id=\"perspective\"]//section//section";

        public ICollection<Fixture> GetFixturesAndResults(string url)
        {
            var fixturesResultList = new List<Fixture>(1030);
            var web = new HtmlWeb();
            var page = web.Load(url);
            var nodes = page.DocumentNode.SelectNodes(FixtureXpath);
            var innerTexts = nodes.Select(x => x.InnerText).ToList();

            var fixtureDataList = innerTexts.SelectMany(s => s.Split('\n', '\t'))
                .Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

            var predictedResult = new PredictedResult();
            var context = new PredictionAlgoContext();

            for (var v = 0; v < fixtureDataList.Count; v++)
            {
                if (!fixtureDataList[v].Contains(Round)) continue;
                if (fixtureDataList[v + 1].Contains(Round)) continue;

                Team homeTeam, awayTeam;
                DateTime fixtureDate;
                int homeScore, awayScore;

                if (CheckDayOfWeek(fixtureDataList[v + 1]))
                {
                    fixtureDate = GetPro12Date(fixtureDataList[v + 2]);
                    if (fixtureDate > DateTime.Now) continue;
                    homeTeam = (Team)Enum.Parse(typeof(Team), StandardiseTeamName(fixtureDataList[v + 4]));
                    awayTeam = (Team)Enum.Parse(typeof(Team), StandardiseTeamName(fixtureDataList[v + 6]));
                    homeScore = GetScores(fixtureDataList[v + 5], 0);
                    awayScore = GetScores(fixtureDataList[v + 5], 1);
                }
                else
                {
                    fixtureDate = GetPro12Date(fixtureDataList[v + 1]);
                    if (fixtureDate > DateTime.Now) continue;
                    if (fixtureDataList[v + 2].Length <= 5)
                    {
                        homeTeam = (Team)Enum.Parse(typeof(Team), StandardiseTeamName(fixtureDataList[v + 3]));
                        awayTeam = (Team)Enum.Parse(typeof(Team), StandardiseTeamName(fixtureDataList[v + 5]));
                        homeScore = GetScores(fixtureDataList[v + 4], 0);
                        awayScore = GetScores(fixtureDataList[v + 4], 1);
                    }
                    else
                    {
                        homeTeam = (Team)Enum.Parse(typeof(Team), StandardiseTeamName(fixtureDataList[v + 2]));
                        awayTeam = (Team)Enum.Parse(typeof(Team), StandardiseTeamName(fixtureDataList[v + 4]));
                        homeScore = GetScores(fixtureDataList[v + 3], 0);
                        awayScore = GetScores(fixtureDataList[v + 3], 1);
                    }
                }
               
                var predictedDelta = predictedResult.GetPredictedResult(homeTeam, awayTeam, fixtureDate, context);
                var predictedFixtureResult = predictedDelta.PredictedScoreDelta < 0 ? Result.HomeLoss :Result.HomeWin;
                var scoreDelta = homeScore - awayScore;
                var actualResult = scoreDelta < 0 ?  Result.HomeLoss : 
                                       scoreDelta == 0 ? Result.Draw : Result.HomeWin;  
                var predictionOutcome = predictedDelta.PredictedScoreDelta - scoreDelta > 0 ?
                                        PredictionOutcome.Fail : PredictionOutcome.Success;
                var fixture = new Fixture
                {
                    FixtureDate = fixtureDate,
                    HomeTeam = homeTeam,
                    HomeScore = homeScore,
                    AwayTeam = awayTeam,
                    AwayScore = awayScore,
                    ScoreDelta = scoreDelta,
                    PredictedDelta = predictedDelta.PredictedScoreDelta,
                    ActualVersusPredictedDelta = scoreDelta - predictedDelta.PredictedScoreDelta,
                    PredictedResult = predictedFixtureResult,
                    PredictionOutcome = predictionOutcome,
                    result = actualResult,
                    Competition = Competition.Pro12,
                    FixtureReference = GetFixtureReference(homeTeam, fixtureDate)
                };
                fixturesResultList.Add(fixture);
            }
            return fixturesResultList;
        }

        public static string StandardiseTeamName(string teamName)
        {
            const string rugby = " Rugby";
            const string newport = "NewportGwent";
            if (teamName.Contains(rugby)) teamName = teamName.Replace(rugby, string.Empty);
            if (teamName.Contains(" ")) teamName = teamName.Replace(" ", string.Empty);
            if (teamName.Contains(newport)) teamName = teamName.Replace(newport, string.Empty);
            return teamName;
        }

        private static bool CheckDayOfWeek(string day)
        {
            return day == "Monday" || day == "Tuesday" || day == "Wednesday" || day ==
                   "Thursday" || day == "Friday" || day == "Saturday" || day == "Sunday";
        }
        public static string GetFixtureReference(Team? homeTeam, DateTime? date)
        {
            var shortDate = date ?? DateTime.Today;
            var dateString = shortDate.ToShortDateString();
            return homeTeam + dateString;
        }
        private static int GetScores(string str, int index)
        {
            var split =  str.Split('-');
            int score;
            try
            {
                score = Convert.ToInt16(split[index].Trim());
            }
            catch (Exception)
            { 
                return 0;
            }
            return score;
        }

        #region GetMatchDataWithDates
        public ICollection<MatchBettingData> GetMatchBettingData(IEnumerable<OddsSpreads> oddsSpreads) 
        {
            var matchBettingDataList = new List<MatchBettingData>(100);
            foreach (var oddsSpread in oddsSpreads)
            {
                if (oddsSpread.IsMatchDataAvailable)
                {
                    Team isValidHomeTeam;
                    Team isValidAwayTeam;

                    try
                    {
                        isValidHomeTeam = (Team)Enum.Parse(typeof(Team), oddsSpread.HomeTeam);
                        isValidAwayTeam = (Team)Enum.Parse(typeof(Team), oddsSpread.AwayTeam);
                    }
                    catch(Exception)
                    {
                        isValidHomeTeam = Team.None;
                        isValidAwayTeam = Team.None;
                    }
                        
                    if (isValidHomeTeam != Team.None || isValidAwayTeam != Team.None)
                    {
                        matchBettingDataList.Add(new MatchBettingData(oddsSpread));
                    }                    
                }
                else
                {
                    matchBettingDataList.Add(new MatchBettingData()
                    {
                        NoOddsAvailableText = "No match data available @: " + DateTime.Now
                    });
                }
            }
            return matchBettingDataList;
        }
        #endregion
        public ICollection<OddsSpreads> GetSpreadsAndOdds(string url)
        {
            var teamAndOddsList = new List<OddsSpreads>(100);
            var web = new HtmlWeb();
            var page = web.Load(url);
            var nodes = page.DocumentNode.SelectNodes(MatchXpath);
            var innerTexts = nodes.Select(x => x.InnerText).ToList();

            var matchDataList = innerTexts.SelectMany(s => s.Split('\n', '\t'))
                .Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

            var matchDate = DateTime.Now.AddDays(1);
            const string versus = " v ";
            var notValidDate = new DateTime(1, 1, 1, 0, 0, 0);
            var notValidDatev2 = new DateTime(2001, 1, 1, 0, 0, 0);

            const string inPlayBetting = "Play Betting on this event";

            for (var x = 0; x < matchDataList.Count; x++)
            {
                DateTime dateCheck;
                if (DateTime.TryParse(GetDate(matchDataList[x]).ToString(CultureInfo.InvariantCulture), out dateCheck))
                {
                    if (dateCheck != notValidDate && dateCheck != notValidDatev2) matchDate = dateCheck;
                }

                if (!matchDataList[x].Contains(versus)) continue;
                if(matchDataList[x + 1].Contains(inPlayBetting))
                {
                    var teams = matchDataList[x].Split(new [] { versus }, StringSplitOptions.None);
                    teamAndOddsList.Add(new OddsSpreads(matchDate,
                        teams[0], matchDataList[x + 2], matchDataList[x + 3], teams[1],
                        matchDataList[x + 8], matchDataList[x + 9]));
                }
                else
                {
                    var teams = matchDataList[x].Split(new [] { versus }, StringSplitOptions.None);
                    teamAndOddsList.Add(new OddsSpreads(matchDate,
                        teams[0], matchDataList[x + 1], matchDataList[x + 2], teams[1],
                        matchDataList[x + 7], matchDataList[x + 8]));
                }
            }

            #region TryCatch
            //try
            //{
            //    var web = new HtmlWeb();
            //    var page = web.Load(url);
            //    var nodes = page.DocumentNode.SelectNodes(MatchXpath);
            //    var innerTexts = nodes.Select(x => x.InnerText).ToList();

            //    var matchDataList = innerTexts.SelectMany(s => s.Split('\n', '\t'))
            //        .Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

            //    for (var x = 0; x < matchDataList.Count; x += 9)
            //    {
            //        teamAndOddsList.Add(new OddsSpreads(
            //        matchDataList[x], matchDataList[x + 1], matchDataList[x + 2], matchDataList[x + 6],
            //        matchDataList[x + 7], matchDataList[x + 8]));
            //    }
            //}
            //catch (Exception)
            //{
            //    var noDataAvailable = new OddsSpreads()
            //    {
            //        NoOddsAvailableText = "No match data available online @: " + DateTime.Now.ToString(),
            //        isMatchDataAvailable = false
            //    };
            //    teamAndOddsList.Add(noDataAvailable);
            //} 
            #endregion

            return teamAndOddsList;
        }

        private static DateTime GetDate(string str)
        {
            var date = new DateTime(1, 1, 1, 0, 0, 0);
            if (str.Length < 4) return date;
            var replaced = str.Substring(0, 4)
                                     .Replace("nd", "")
                                     .Replace("th", "")
                                     .Replace("rd", "")
                                     .Replace("st", "")
                                     + str.Substring(4);
            var cultureUs = new CultureInfo("en-US")
            {
                DateTimeFormat =
                {
                    AbbreviatedMonthNames = new []
                    {
                        "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sept", "Oct", "Nov", "Dec", ""
                    }
                }
            };
            DateTime.TryParseExact(replaced, "dd MMM yyyy", cultureUs, DateTimeStyles.AssumeUniversal, out date);
            if(date == new DateTime(1, 1, 1)) DateTime.TryParseExact(replaced, "d MMM yyyy", cultureUs, DateTimeStyles.AssumeUniversal, out date);
            return date;
        }

        private static DateTime GetPro12Date(string date)
        {
            if (date.Contains('/'))
            {
                var lastSlash = date.LastIndexOf('/');
                date = date.Substring(lastSlash + 1, date.Length - (lastSlash + 1));
            }

            var split = date.Split(' ');
            var day = Convert.ToInt16(split[0]);
            var month = DateTime.ParseExact(split[1], "MMM", CultureInfo.CurrentCulture).Month;
            var year = DateTime.Now.Year;
            if (month == 1 || month == 2 || month == 3 || month == 4 || month == 5 || month == 6) year = 2017;
            return new DateTime(year, month, day);
        }

        public class OddsSpreads
        {
            public DateTime Date { get; set; }
            public string HomeTeam { get; set; }
            public string HomeSpread { get; set; }
            public string HomeTeamOdds { get; set; }
            public string AwayTeam { get; set; }
            public string AwaySpread { get; set; }
            public string AwayTeamOdds { get; set; }
            public decimal Numerator { get; set; }
            public decimal Denominator { get; set; }
            public decimal HomeTeamDecimalOdds { get; set; }
            public decimal AwayTeamDecimalOdds { get; set; }
            public string NoOddsAvailableText { get; set; }
            public bool IsMatchDataAvailable = true;

            public OddsSpreads(DateTime date, string homeTeam, string homeSpread, string homeTeamOdds, string awayTeam, string awaySpread, string awayTeamOdds)
            {
                Date = date;
                HomeTeam = homeTeam.Replace(" ", string.Empty);
                HomeTeamOdds = homeTeamOdds.Trim();
                AwayTeam = awayTeam.Replace(" ", string.Empty);
                AwayTeamOdds = awayTeamOdds.Trim();

                HomeSpread = new StringBuilder(homeSpread).Replace("(", string.Empty).Replace(")", string.Empty).Replace("+", string.Empty).ToString();
                AwaySpread = new StringBuilder(awaySpread).Replace("(", string.Empty).Replace(")", string.Empty).Replace("+", string.Empty).ToString();

                var oddsHomeTeamSplit = HomeTeamOdds.Split('/');
                if (oddsHomeTeamSplit[0] == "evens")
                    HomeTeamDecimalOdds = 1;
                else
                    HomeTeamDecimalOdds = Convert.ToDecimal(oddsHomeTeamSplit[0]) / Convert.ToDecimal(oddsHomeTeamSplit[1]);

                var oddsAwayTeamSplit = awayTeamOdds.Split('/');
                if (oddsAwayTeamSplit[0] == "evens")
                    AwayTeamDecimalOdds = 1;
                else
                    AwayTeamDecimalOdds = Convert.ToDecimal(oddsAwayTeamSplit[0]) / Convert.ToDecimal(oddsAwayTeamSplit[1]);
            }
        }
    }
}

#region GetMatchDate

//public class MatchDateData
//{
//    public DateTime Date { get; set; }
//    public string HomeTeam { get; set; }
//    public string AwayTeam { get; set; }
//    public string NoOddsAvailableText { get; set; }
//    public bool IsMatchDataAvailable = true;
//}

//    private const string DateXpath = "//*[@id=\"main\"]//table//tr//td//strong";

//public MatchDateData() { } // req when no betting data found online
//public MatchDateData(string teams, string date)
//{
//    var dateSplit = date.Split(' ');
//    var day = dateSplit[1].Substring(0, dateSplit[1].Length - 2);
//    var year = dateSplit[4].Substring(0, dateSplit[4].Length - 1);
//    int monthInDigit = DateTime.ParseExact(dateSplit[3], "MMMM", CultureInfo.InvariantCulture).Month;

//    var timeSplit = dateSplit[5].Split(':');

//    Date = new DateTime(Convert.ToInt16(year), monthInDigit, Convert.ToInt16(day), Convert.ToInt16(timeSplit[0]),
//        Convert.ToInt16(timeSplit[1]), 0);

//    var teamSplit = teams.Split(new string[] { " v " }, StringSplitOptions.None);

//    HomeTeam = teamSplit[0].Replace("&nbsp", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(";", string.Empty).Trim();
//    AwayTeam = teamSplit[1].Replace("&nbsp", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(";", string.Empty).Trim();
//} 

//public List<MatchDateData> GetMatchDates(string url, Competition comp)
//{
//    var html = new HtmlWeb();
//    var page = html.Load(url);
//    var matchDateData = new List<MatchDateData>(100);
//    var nodes = page.DocumentNode.SelectNodes(DateXpath);
//    var innerTexts = nodes.Select(x => x.InnerText).ToList();

//    var matchDateDataList = innerTexts.SelectMany(s => s.Split('\n', '\t'))
//        .Where(s => !string.IsNullOrWhiteSpace(s))
//     .Where(s => s != "&nbsp;").ToList();

//    #region DateParse
//    //if (comp == Competition.Pro12)
//    //{
//    //    for (var x = 1; x < matchDateDataList.Count(); x += 5)
//    //    {
//    //        matchDateData.Add(new MatchDateData(innerTexts[x], innerTexts[x + 1]));
//    //    }
//    //}
//    //else
//    //{
//    //for (var x = 2; x < matchDateDataList.Count(); x += 5)
//    //{
//    //    matchDateData.Add(new MatchDateData(innerTexts[x], innerTexts[x + 1]));
//    //}
//    //} 
//    #endregion

//    #region TryCatch
//    //try
//    //{
//    //    var nodes = page.DocumentNode.SelectNodes(DateXpath);
//    //    var innerTexts = nodes.Select(x => x.InnerText).ToList();

//    //    var matchDateDataList = innerTexts.SelectMany(s => s.Split('\n', '\t'))
//    //        .Where(s => !string.IsNullOrWhiteSpace(s))
//    //     .Where(s => s != "&nbsp;").ToList();

//    //    if (comp == Competition.Top14)
//    //    {
//    //        for (var x = 1; x < matchDateDataList.Count(); x += 5)
//    //        {
//    //            matchDateData.Add(new MatchDateData(innerTexts[x], innerTexts[x + 1]));
//    //        }
//    //    }
//    //    else
//    //    {
//    //        for (var x = 2; x < matchDateDataList.Count(); x += 5)
//    //        {
//    //            matchDateData.Add(new MatchDateData(innerTexts[x], innerTexts[x + 1]));
//    //        }
//    //    }
//    //}
//    //catch(Exception)
//    //{
//    //    var noDataAvailable = new MatchDateData()
//    //    {
//    //        NoOddsAvailableText = "No match data available online @: " + DateTime.Now.ToString(),
//    //        isMatchDataAvailable = false
//    //    };
//    //    matchDateData.Add(noDataAvailable);
//    //} 
//    #endregion
//    return matchDateData;
//}


#endregion


