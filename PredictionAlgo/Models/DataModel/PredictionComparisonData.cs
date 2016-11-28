using PredictionAlgo.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PredictionAlgo.Models.DataModel
{
    public class PredictionComparisonData
    {
        public List<PredictionComparison> PredictedComparisonDataList { get; set; } = new List<PredictionComparison>();
        private readonly PredictionAlgoContext _db = new PredictionAlgoContext();
        private readonly PredictedResult _predictedResult = new PredictedResult();
        private Team? _swerveTeam;
        private Team? _teamToBack;

        public void AddPredictionComparisonsToFile(IEnumerable<MatchBettingData> bettingData)
        {
            //UpdateMatchBettingDataReferences(); // used initially to set references correctly
            var fixtures = _db.Fixtures;

            foreach (var betData in bettingData)
            {
                var predictedScoreDelta = GetPredictedScoreSpread(betData.HomeTeam, betData.AwayTeam, betData.FixtureDate, _db);
                GetTeamToBack(betData, predictedScoreDelta);

                var prediction = new PredictionComparison(betData, _teamToBack, _swerveTeam, predictedScoreDelta);
                prediction.ActualScoreDelta = GetActualScoreDelta(prediction.BettingData.FixtureReference, fixtures);

                prediction.PredictionComparisonReference = WebScraper.GetFixtureReference(prediction.HomeTeam, prediction.BettingData.FixtureDate);
                prediction.BettingData.MatchDataReference = prediction.PredictionComparisonReference;
                PredictedComparisonDataList.Add(prediction);
            }

            var predictionsWithoutDupes = PredictedComparisonDataList.GroupBy(x => x.PredictionComparisonReference)
                        .Select(x => x.First());

            foreach (var prediction in predictionsWithoutDupes)
            {
                if (_db.PredictionComparisons.Find(prediction.PredictionComparisonReference) != null)
                {
                    var predictionToEdit = _db.PredictionComparisons.Find(prediction.PredictionComparisonReference);
                    _db.PredictionComparisons.Remove(predictionToEdit);
                    _db.PredictionComparisons.Add(prediction);
                }
                else
                {
                    _db.PredictionComparisons.Add(prediction);
                }
            }

            _db.SaveChanges();
        }

        public IEnumerable<PredictionComparison> GetAllPredictionComparisons(PredictionAlgoContext context)
        {
            var fixturesWithBettingData = GetFixtureAndBettingDatas(context);

            var predictionComparisons = fixturesWithBettingData.Select(
                fixtureWithBetting => new PredictionComparison(
                fixtureWithBetting.Fixture, fixtureWithBetting.BettingData)).ToList();

            var distinctList = predictionComparisons.OrderByDescending(x => x.TimeStamp)
                        .GroupBy(x => x.PredictionComparisonReference)
                        .Select(g => g.First())
                        .ToList();

            foreach (var prediction in distinctList)
            {
                var backedTeamData = GetTeamToBack(prediction.BettingData, prediction.AlgoScoreSpreadPrediction);
                prediction.TeamToBack = backedTeamData.BackedTeam;

                prediction.SwerveTeam = prediction.TeamToBack == prediction.HomeTeam ? prediction.AwayTeam : prediction.HomeTeam;

                prediction.PredictionResult =
                    GetPredictionOutcome(prediction.BettingData, backedTeamData, prediction.ActualScoreDelta);
            }

            foreach (var prediction in distinctList)
            {
                if (_db.PredictionComparisons.Any(
                        x => x.PredictionComparisonReference == prediction.PredictionComparisonReference))
                {
                    var recordToEdit = _db.PredictionComparisons.FirstOrDefault(
                            x => x.PredictionComparisonReference == prediction.PredictionComparisonReference);

                    _db.PredictionComparisons.Remove(recordToEdit);
                    _db.PredictionComparisons.Add(prediction);
                }
                else
                {
                    _db.PredictionComparisons.Add(prediction);
                }
            }

            _db.SaveChanges();

            return distinctList;
        }
        private static IEnumerable<FixtureAndBettingData> GetFixtureAndBettingDatas(PredictionAlgoContext context)
        {
            var fixturesWithBettingData = new List<FixtureAndBettingData>();

            var fixtures = context.Fixtures.Where(x => x.FixtureDate > new DateTime(2016, 07, 30));
            var bettingData = context.MatchBettingDatas;

            foreach (var fixture in fixtures)
            {
                foreach (var betData in bettingData)
                {
                    if (fixture.FixtureReference == betData.FixtureReference)
                    {
                        fixturesWithBettingData.Add(new FixtureAndBettingData(fixture, betData));
                    }
                }
            }

            return fixturesWithBettingData;
        }

        private PredictionOutcome GetPredictionOutcome(
            MatchBettingData bettingData, BackedTeamData backedTeamData, double actualDelta)
        {
            var isHomeTeamTheBackedTeam = backedTeamData.BackedTeam == bettingData.HomeTeam;
            var predictionOutcome = PredictionOutcome.Fail;

            if (isHomeTeamTheBackedTeam)
            {
                if (bettingData.HomeSpread < 0 && actualDelta < 0)
                    predictionOutcome = PredictionOutcome.Fail;
                else if (bettingData.HomeSpread < 0 && actualDelta > 0)
                    predictionOutcome = bettingData.HomeSpread + actualDelta > 0
                        ? PredictionOutcome.Success
                        : PredictionOutcome.Fail;
                else if (bettingData.HomeSpread > 0 && actualDelta < 0)
                    predictionOutcome = bettingData.HomeSpread + actualDelta > 0
                        ? PredictionOutcome.Success
                        : PredictionOutcome.Fail;
                else if (bettingData.HomeSpread > 0 && actualDelta > 0)
                    predictionOutcome = PredictionOutcome.Success;
            }
            else
            {
                if (bettingData.AwaySpread < 0 && actualDelta < 0)
                    predictionOutcome = actualDelta - bettingData.AwaySpread < 0
                        ? PredictionOutcome.Success
                        : PredictionOutcome.Fail;
                else if (bettingData.AwaySpread < 0 && actualDelta > 0)
                    predictionOutcome = bettingData.AwaySpread + actualDelta > 0
                        ? PredictionOutcome.Success
                        : PredictionOutcome.Fail;
                else if (bettingData.AwaySpread > 0 && actualDelta < 0)
                    predictionOutcome = PredictionOutcome.Success;
                else if (bettingData.AwaySpread > 0 && actualDelta > 0)
                    predictionOutcome = actualDelta - bettingData.AwaySpread < 0
                        ? PredictionOutcome.Success
                        : PredictionOutcome.Fail;
            }

            return predictionOutcome;
        }

        //private PredictionOutcome CheckTeamToBackOutcome(MatchBettingData bettingData, double predictedScoreDelta)
        //{
        //    var teamToBackBookDelta = _teamToBack == bettingData.AwayTeam ? bettingData.AwaySpread : bettingData.HomeSpread;

        //    if(teamToBackBookDelta > 0)
        //    {
        //        return teamToBackBookDelta < predictedScoreDelta ? PredictionOutcome.Success : PredictionOutcome.Fail;
        //    }
        //    return teamToBackBookDelta > predictedScoreDelta ? PredictionOutcome.Success : PredictionOutcome.Fail;
        //}

        //private static PredictionOutcome CheckResultAgainstActualDeltaVsPredictedDelta(double actualScoreDelta, double predictedScoreDelta)
        //{
        //    if(actualScoreDelta > 0)
        //    {
        //        return actualScoreDelta > predictedScoreDelta ? PredictionOutcome.Success : PredictionOutcome.Fail;
        //    }
        //    return actualScoreDelta < predictedScoreDelta  ? PredictionOutcome.Success : PredictionOutcome.Fail;
        //}

        private double GetPredictedScoreSpread(Team? homeTeam, Team? awayTeam, DateTime? date, PredictionAlgoContext context)
        {
            var aveHomeScoreLastFiveHomeResults = _predictedResult.GetAverageHomeScoreLastFiveHomeGames(homeTeam, date, context);
            var aveAwayScoreLastFiveAwayResults = _predictedResult.GetAverageAwayScoreLastFiveAwayGames(awayTeam, date, context);

            var scoreDeltalastFiveHomeResults = _predictedResult.GetAverageAwayScoreLastFiveAwayGames(homeTeam, date, context);
            var scoreDeltalastFiveAwayResults = _predictedResult.GetAverageScoreDeltaLastFiveAwayGames(awayTeam, date, context);

            var lastTwoResultsBtwnTeams = _predictedResult.GetAverageScoreDeltaOfLastTwoResultsBetweenTeams(homeTeam, awayTeam, date, context);

            var result = (aveHomeScoreLastFiveHomeResults - aveAwayScoreLastFiveAwayResults
                                                    + scoreDeltalastFiveHomeResults
                                                    + scoreDeltalastFiveAwayResults)
                                                    //+ lastTwoResultsBtwnTeams)
                                                    / 4;
            return result;
            //return _predictedResult.ApplySpreadChangeForDate(result, date);
        }

        private static double GetActualScoreDelta(string fixtureReference, IEnumerable<Fixture> fixtures)
        {
            var firstOrDefault = fixtures.FirstOrDefault(x => x.FixtureReference == fixtureReference);
            if (firstOrDefault != null)
            {
                return firstOrDefault.ScoreDelta;
            }
            return -1000;
        }

        private BackedTeamData GetTeamToBack(MatchBettingData betData, double predictedScoreSpread)
        {
            double predictedVsBookScoreSpread = 0;

            if (predictedScoreSpread > 0 && betData.HomeSpread > 0)
                predictedVsBookScoreSpread = predictedScoreSpread - betData.HomeSpread;
            else if (predictedScoreSpread > 0 && betData.HomeSpread < 0)
                predictedVsBookScoreSpread = predictedScoreSpread + betData.HomeSpread;
            else if (predictedScoreSpread < 0 && betData.HomeSpread > 0)
                predictedVsBookScoreSpread = predictedScoreSpread + betData.HomeSpread;
            else if (predictedScoreSpread < 0 && betData.HomeSpread < 0)
                predictedVsBookScoreSpread = predictedScoreSpread - betData.HomeSpread;

            _teamToBack = predictedVsBookScoreSpread > 0 ? betData.HomeTeam : betData.AwayTeam;
            _swerveTeam = _teamToBack == betData.HomeTeam ? betData.AwayTeam : betData.HomeTeam;

            //ReversePrediction(_teamToBack, _swerveTeam);

            return new BackedTeamData(_teamToBack, predictedScoreSpread, betData.HomeSpread);
        }

        private void ReversePrediction(Team? backedTeam, Team? swerveTeam)
        {
            _swerveTeam = backedTeam;
            _teamToBack = swerveTeam;
        }
        public double GetTotalPreditionSuccess
        {
            get
            {
                double numberOfSuccessPredictions = _db.PredictionComparisons.Count(
                    x => x.PredictionResult == PredictionOutcome.Success);

                double totalPredictionCount = _db.PredictionComparisons
                    .Where(x => x.ActualScoreDelta != 0)
                    .GroupBy(x => x.PredictionComparisonReference)
                    .Count();

                return Math.Round(numberOfSuccessPredictions / totalPredictionCount * 100, 1);
            }
        }

        private void UpdateMatchBettingDataReferences() // used initially to update references
        {
            foreach (var bettingData in _db.MatchBettingDatas.ToList())
            {
                bettingData.FixtureReference = bettingData.HomeTeam + bettingData.FixtureDate?.ToShortDateString();
            }
            _db.SaveChanges();
        }
    }

    internal struct FixtureAndBettingData
    {
        public Fixture Fixture;
        public MatchBettingData BettingData;

        public FixtureAndBettingData(Fixture fixture, MatchBettingData betting)
        {
            Fixture = fixture;
            BettingData = betting;
        }
    }

    internal struct BackedTeamData
    {
        public Team? BackedTeam;
        public double PredictedDelta;
        public double HomeSpread;

        public BackedTeamData(Team? backedTeam, double predictedDelta, double homeSpread)
        {
            BackedTeam = backedTeam;
            PredictedDelta = predictedDelta;
            HomeSpread = homeSpread;
        }
    }
}