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
                GetTeamToBack(betData.HomeTeam, betData.AwayTeam, predictedScoreDelta, betData.HomeSpread);

                var prediction = new PredictionComparison(betData, _teamToBack, _swerveTeam, predictedScoreDelta);
                prediction.ActualScoreDelta = GetActualScoreDelta(prediction.BettingData.FixtureReference, fixtures);
                prediction.PredictionResult = GetPredictionOutcome(betData,  predictedScoreDelta, prediction.ActualScoreDelta);
                prediction.PredictionComparisonReference = WebScraper.GetFixtureReference(prediction.HomeTeam, prediction.BettingData.FixtureDate);
                prediction.BettingData.MatchDataReference = prediction.PredictionComparisonReference;
                PredictedComparisonDataList.Add(prediction);
            }
            var predictionsWithoutDupes = PredictedComparisonDataList.GroupBy(x => x.PredictionComparisonReference)
                        .Select(x => x.First());

            foreach (var prediction in predictionsWithoutDupes)
            {

                if(_db.PredictionComparisons.Find(prediction.PredictionComparisonReference) != null)
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
        private PredictionOutcome GetPredictionOutcome( MatchBettingData bettingData, double predictedScoreDelta, double actualScoreDelta)
        {
            var checkTeamToBackOutcome = CheckTeamToBackOutcome(bettingData, predictedScoreDelta);
            var backedTeamOutcome = CheckTheResult_ActualDeltaVsPredictedDelta(actualScoreDelta, predictedScoreDelta);
            return checkTeamToBackOutcome == PredictionOutcome.Success && backedTeamOutcome == PredictionOutcome.Success
                ? PredictionOutcome.Success
                : PredictionOutcome.Fail;
        }
        private PredictionOutcome CheckTeamToBackOutcome( MatchBettingData bettingData, double predictedScoreDelta)
        {
            var teamToBackBookDelta = _teamToBack == bettingData.AwayTeam ? bettingData.AwaySpread : bettingData.HomeSpread;

            if(teamToBackBookDelta > 0)
            {
                return teamToBackBookDelta < predictedScoreDelta ? PredictionOutcome.Success : PredictionOutcome.Fail;
            }
            return teamToBackBookDelta > predictedScoreDelta ? PredictionOutcome.Success : PredictionOutcome.Fail;
        }
        private static PredictionOutcome CheckTheResult_ActualDeltaVsPredictedDelta(double actualScoreDelta, double predictedScoreDelta)
        {
            if(actualScoreDelta > 0)
            {
                return actualScoreDelta > predictedScoreDelta ? PredictionOutcome.Success : PredictionOutcome.Fail;
            }
            return actualScoreDelta < predictedScoreDelta  ? PredictionOutcome.Success : PredictionOutcome.Fail;
        }
        private double GetPredictedScoreSpread(Team? homeTeam, Team? awayTeam, DateTime? date, PredictionAlgoContext context)
        {
            var aveHomeScoreLastFiveHomeResults = _predictedResult.GetAverageHomeScoreLastFiveHomeGames(homeTeam, date, context);
            var aveAwayScoreLastFiveAwayResults = _predictedResult.GetAverageAwayScoreLastFiveAwayGames(awayTeam, date, context);

            var scoreDeltalastFiveHomeResults = _predictedResult.GetAverageAwayScoreLastFiveAwayGames(homeTeam, date, context);
            var scoreDeltalastFiveAwayResults = _predictedResult.GetAverageScoreDeltaLastFiveAwayGames(awayTeam, date, context);

            var lastTwoResultsBtwnTeams = _predictedResult.GetAverageScoreDeltaOfLastTwoResultsBetweenTeams(homeTeam, awayTeam, date, context);

            var result = (aveHomeScoreLastFiveHomeResults - aveAwayScoreLastFiveAwayResults
                                                    + scoreDeltalastFiveHomeResults
                                                    + scoreDeltalastFiveAwayResults
                                                    + lastTwoResultsBtwnTeams)
                                                    / 4;

            return _predictedResult.ApplySpreadChangeForDate(result, date);
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

        private void GetTeamToBack(Team? homeTeam, Team? awayTeam, double predictedScoreSpread, double bookScoreSpread)
        {
            var predictedVsBookScoreSpread = predictedScoreSpread - bookScoreSpread;
            _teamToBack = predictedVsBookScoreSpread < 0 ? homeTeam : awayTeam;
            _swerveTeam = _teamToBack == homeTeam ? awayTeam : homeTeam;
        }
        public double GetTotalPreditionSuccess
        {
            get
            {
                var numberOfSuccessPredictions = _db.PredictionComparisons.Count(x => x.PredictionResult == PredictionOutcome.Success);
                numberOfSuccessPredictions = numberOfSuccessPredictions == 0 ? -1 : numberOfSuccessPredictions;
                var totalPredictionCount = _db.PredictionComparisons.Count();
                totalPredictionCount = totalPredictionCount == 0 ? 1 : totalPredictionCount;
                return numberOfSuccessPredictions / totalPredictionCount;
            }
        }
        private void UpdateMatchBettingDataReferences() // used initially
        {
            foreach (var bettingData in _db.MatchBettingDatas.ToList())
            {
                bettingData.FixtureReference = bettingData.HomeTeam + bettingData.FixtureDate?.ToShortDateString(); 
            }
            _db.SaveChanges();
        }
    }
}