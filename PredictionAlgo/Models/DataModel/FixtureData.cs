using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PredictionAlgo.Models.ViewModel;

namespace PredictionAlgo.Models.DataModel
{
    public class FixtureData
    {
        private const string Pro12Fixtures = "http://www.pro12rugby.com/fixtures/";
        private readonly string _uriPro12Fixtures = new Uri(Pro12Fixtures).AbsoluteUri;
        private readonly PredictionAlgoContext _db = new PredictionAlgoContext();
        public IEnumerable<Fixture> GetFixturesAndResults
        {
            get
            {
                var existingFixtureReferences = _db.Fixtures.Select(x => x.FixtureReference);
                var webScraper = new WebScraper();
                var scrapedFixturesAndResultsList = webScraper.GetFixturesAndResults(_uriPro12Fixtures)
                        .GroupBy(x => x.FixtureReference)
                        .Select(x => x.First());

                foreach (var fixture in scrapedFixturesAndResultsList)
                {
                    if (existingFixtureReferences.Contains(fixture.FixtureReference))
                    {
                        var fixtureToEdit = _db.Fixtures.FirstOrDefault(x => x.FixtureReference == fixture.FixtureReference);
                        _db.Fixtures.Remove(fixtureToEdit);
                        _db.Fixtures.Add(fixture);
                    }
                    else
                    {
                        _db.Fixtures.Add(fixture);
                    }
                }
                _db.SaveChanges();
                return scrapedFixturesAndResultsList;
            }
        }
        public IEnumerable<Fixture> GetIcalFixturesAndResults
        {
            get
            {
                var ical = new FixtureIcalParser();
                var fixtureRawData = ical.GetFixtureData(ical.GetFixturesIcalToFile()).ToArray();
                var predictedResult = new PredictedResult();
                var context = new PredictionAlgoContext();

                ICollection<Fixture> resultsList = new List<Fixture>(1000);
                foreach (var fixture in fixtureRawData)
                {
                    var result = fixture.Contains(" - ") 
                        ? GetResult(predictedResult, context, fixture) 
                        : GetFutureFixture(predictedResult, context, fixture);
                    
                    resultsList.Add(result);

                    if (_db.Fixtures.Any(x => x.FixtureReference == result.FixtureReference))
                    {
                        var fixtureToEdit = _db.Fixtures.FirstOrDefault(x => x.FixtureDate == result.FixtureDate);
                        _db.Fixtures.Remove(fixtureToEdit);
                        _db.Fixtures.Add(result);
                    }
                    else
                    {
                        _db.Fixtures.Add(result);
                    }
                }
                _db.SaveChanges();
                return resultsList;
            }
        }

        private static Fixture GetFutureFixture(PredictedResult predictedResult, PredictionAlgoContext context, string fixture)
        {
            var regex = new Regex("UID:(?<date>[0-9]+)T\\d+Z(?<homeTeam>[a-zA-Z ]+) v  (?<awayTeam>[a-zA-Z ]+)");
            var match = regex.Match(fixture);
            var date = GetDate(match.Groups["date"].Value);
            var homeTeam = (Team)Enum.Parse(typeof(Team), WebScraper.StandardiseTeamName(match.Groups["homeTeam"].Value));
            var awayTeam = (Team)Enum.Parse(typeof(Team), WebScraper.StandardiseTeamName(match.Groups["homeTeam"].Value));
            var predictedData = predictedResult.GetPredictedResult(homeTeam, awayTeam, date, context);

            return new Fixture
            {
                FixtureReference = WebScraper.GetFixtureReference(homeTeam, date),
                FixtureDate = date,
                HomeTeam = homeTeam,
                HomeScore = 0,
                AwayTeam = awayTeam,
                AwayScore = 0,
                result = null,
                ScoreDelta = 0,
                PredictedDelta = predictedResult.ApplySpreadChangeForDate(predictedData.PredictedScoreDelta, DateTime.Now),
                ActualVersusPredictedDelta = 0 - predictedData.PredictedScoreDelta,
                PredictedResult = predictedData.PredictedScoreDelta > 0 ? Result.HomeWin : Result.HomeLoss,
                PredictionOutcome = null,
                Competition = Competition.Pro12
            };
        }

        private static Fixture GetResult(PredictedResult predictedResult, PredictionAlgoContext context, string fixture)
        {
            var regex = new Regex("UID:(?<date>[0-9]+)T\\d+Z(?<homeTeam>[a-zA-Z ]+) (?<homeScore>[0-9]+) - (?<awayScore>[0-9]+) (?<awayTeam>[a-zA-Z ]+)");
            var match = regex.Match(fixture);
            var date = GetDate(match.Groups["date"].Value);
            var homeTeam = (Team)Enum.Parse(typeof(Team), WebScraper.StandardiseTeamName(match.Groups["homeTeam"].Value));
            var homeScore = Convert.ToInt16(match.Groups["homeScore"].Value);
            var awayTeam = (Team)Enum.Parse(typeof(Team), WebScraper.StandardiseTeamName(match.Groups["homeTeam"].Value));
            var awayScore = Convert.ToInt16(match.Groups["awayScore"].Value);
            var delta = homeScore - awayScore;
            var actualResult = delta < 0 ? Result.HomeLoss : delta == 0 ? Result.Draw : Result.HomeWin;
            var predictedData = predictedResult.GetPredictedResult(homeTeam, awayTeam, date, context);
            var resultPrediction= delta > 0 ? Result.HomeWin : Result.HomeLoss;

            return new Fixture
            {
                FixtureReference = WebScraper.GetFixtureReference(homeTeam, date),
                FixtureDate = date,
                HomeTeam = homeTeam,
                HomeScore = homeScore,
                AwayTeam = awayTeam,
                AwayScore = awayScore,
                result = actualResult,
                ScoreDelta = delta,
                PredictedDelta = predictedResult.ApplySpreadChangeForDate(predictedData.PredictedScoreDelta, DateTime.Now),
                ActualVersusPredictedDelta = delta - predictedData.PredictedScoreDelta,
                PredictedResult = resultPrediction,
                PredictionOutcome = actualResult == resultPrediction ? PredictionOutcome.Success : PredictionOutcome.Fail,
                Competition = Competition.Pro12
            };
        }
        
        private static DateTime GetDate(string date)
        {
            var regex = new Regex("(?<year>\\d{4})(?<month>\\d{2})(?<day>\\d{2})");
            var match = regex.Match(date);
            return new DateTime(
                Convert.ToInt16(match.Groups["year"].Value),
                Convert.ToInt16(match.Groups["month"].Value),
                Convert.ToInt16(match.Groups["day"].Value));
        }

        public double GetResultsWithoutSpreadsPredictionSuccessRate
        {
            get
            {
                double numberOfSuccessPredictions = _db.Fixtures.Count(
                    x => x.PredictionOutcome == PredictionOutcome.Success);

                double totalPredictionCount = _db.Fixtures
                    .Where(x => x.ScoreDelta != 0)
                    .GroupBy(x => x.FixtureReference)
                    .Count();

                return Math.Round(numberOfSuccessPredictions / totalPredictionCount * 100, 1);
            }
        }
    }
}