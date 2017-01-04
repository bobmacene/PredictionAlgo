using PredictionAlgo.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PredictionAlgo.Models.DataModel
{
    public sealed class PredictionComparisonData : CommonFunctions, IDisposable
    {
        public ICollection<PredictionComparison> PredictedComparisonDataList { get; set; } = new List<PredictionComparison>();
        private readonly PredictionAlgoContext _db = new PredictionAlgoContext();
        private readonly PredictedResult _predictedResult = new PredictedResult();
        private Team? _swerveTeam;
        private Team? _teamToBack;


        public IEnumerable<PredictionComparison> GetPredictionComparisons(IEnumerable<MatchBettingData> bettingData)
        {
            var fixtures = _db.Fixtures;

            foreach (var betData in bettingData)
            {
                var predictedScoreDelta = _predictedResult.GetPredictedDelta(betData.HomeTeam, betData.AwayTeam, betData.FixtureDate, _db);

                GetTeamToBack(betData, predictedScoreDelta);

                var prediction = new PredictionComparison(betData, _teamToBack, _swerveTeam, predictedScoreDelta);
                prediction.ActualScoreDelta = GetActualScoreDelta(prediction.BettingData.FixtureReference, fixtures);

                prediction.PredictionComparisonReference = GetFixtureReference(prediction.HomeTeam,
                    prediction.BettingData.FixtureDate);

                prediction.BettingData.MatchDataReference = prediction.PredictionComparisonReference;

                PredictedComparisonDataList.Add(prediction);
            }

            return PredictedComparisonDataList;
        }


        public void UpdateDatabase()
        {
            var predictionsWithoutDupes = PredictedComparisonDataList
                .GroupBy(x => x.PredictionComparisonReference).Select(x => x.First());

            foreach (var prediction in predictionsWithoutDupes)
            {
                var predictionToEdit = _db.PredictionComparisons.Find(prediction.PredictionComparisonReference);

                if (predictionToEdit != null)
                {    
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

        public IEnumerable<PredictionComparison> UpdatePredictions()
        {
            var fixturesToUpdate = _db.PredictionComparisons
                .Where(x => x.ActualScoreDelta == -1000).ToList();

            foreach (var fixture in fixturesToUpdate)
            {
                var predictionToEdit = _db.Fixtures.Find(fixture.PredictionComparisonReference);

                if (predictionToEdit == null) continue;

                fixture.PredictionDelta = predictionToEdit.PredictedDelta;
                fixture.ActualScoreDelta = predictionToEdit.ScoreDelta;
            }

            _db.SaveChanges();

            return _db.PredictionComparisons.OrderByDescending(x=>x.FixtureDate);
        }

        public IEnumerable<FixtureAndBettingData> GetFixtureBettingDatas(PredictionAlgoContext context)
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


        public PredictionOutcome GetPredictionOutcome(
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


        private static double GetActualScoreDelta(string fixtureReference, IEnumerable<Fixture> fixtures)
        {
            var firstOrDefault = fixtures.FirstOrDefault(x => x.FixtureReference == fixtureReference);
            if (firstOrDefault != null)
            {
                return firstOrDefault.ScoreDelta;
            }
            return -1000;
        }


        public BackedTeamData GetTeamToBack(MatchBettingData betData, double predictedScoreSpread)
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


            return new BackedTeamData(_teamToBack, predictedScoreSpread, betData.HomeSpread);
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

        public void Dispose()
        {
            _db.Dispose();
            GC.SuppressFinalize(this);
        }

    }

    public struct FixtureAndBettingData
    {
        public Fixture Fixture;
        public MatchBettingData BettingData;

        public FixtureAndBettingData(Fixture fixture, MatchBettingData betting)
        {
            Fixture = fixture;
            BettingData = betting;
        }
    }

    public struct BackedTeamData
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
