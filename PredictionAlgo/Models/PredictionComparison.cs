using PredictionAlgo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PredictionAlgo.Models
{
    public class PredictionComparison
    {
        [Key]
        public string PredictionComparisonReference { get; set; }
        public DateTime TimeStamp { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy MM dd}")]
        [DataType(DataType.Date)]
        [DisplayName("Fixture Date ")]
        public DateTime? FixtureDate { get; set; }
        [DisplayName("Predicted Score Delta ")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double AlgoScoreSpreadPrediction { get; set; }
        [DisplayName("Book v Predicted")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double BookVsPrediction { get; set; }
        [DisplayName("Actual Score Delta")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double ActualScoreDelta { get; set; }
        [DisplayName("Backed Team ")]
        public Team? TeamToBack { get; set; }
        public Team? SwerveTeam { get; set; }
        public PredictionOutcome PredictionResult { get; set; }
        public Team? HomeTeam { get; set; }
        public Team? AwayTeam { get; set; }
        public double HomeSpread { get; set; }
        public double AwaySpread { get; set; }

        public MatchBettingData BettingData { get; }

        public PredictionComparison() { }  // required for PredictionComparison view ToList call
        public PredictionComparison(MatchBettingData bettingData, Team? teamToBack, Team? swerveTeam, double algoScoreSpreadPrediction)
        {
            FixtureDate = bettingData.FixtureDate;
            HomeTeam = bettingData.HomeTeam;
            HomeSpread = bettingData.HomeSpread;
            AwayTeam = bettingData.AwayTeam;
            AwaySpread = bettingData.AwaySpread;
            BettingData = bettingData;
            TeamToBack = teamToBack;
            SwerveTeam = swerveTeam;
            AlgoScoreSpreadPrediction = algoScoreSpreadPrediction;
            BookVsPrediction = GetBookVsPrediction(algoScoreSpreadPrediction, bettingData.HomeSpread);
            TimeStamp = DateTime.Now;
        }
    
        public PredictionComparison(Fixture fixture, MatchBettingData betting)
        {
            FixtureDate = fixture.FixtureDate;
            HomeTeam = fixture.HomeTeam;
            HomeSpread = betting.HomeSpread;
            AwayTeam = fixture.AwayTeam;
            AwaySpread = betting.AwaySpread;
            BettingData = betting;
            TeamToBack = GetTeamToBack(fixture.HomeTeam, fixture.AwayTeam, fixture.PredictedDelta, betting.HomeSpread);
            AlgoScoreSpreadPrediction = fixture.PredictedDelta;
            BookVsPrediction = GetBookVsPrediction(fixture.PredictedDelta, betting.HomeSpread);
            TimeStamp = DateTime.Now;
        }

        private double GetBookVsPrediction(double predictionDelta, double bookSpread)
        {
            return predictionDelta - bookSpread;
        }

        public PredictionComparison GetAllPredictionComparisons(PredictionAlgoContext context)
        {
            var predictionComparisons = new List<PredictionComparison>();
            var predictionComp = new PredictionComparison();

            var fixtures = context.Fixtures.Where(x => x.FixtureDate > new DateTime(2016, 07, 30));
            var bettingData = context.MatchBettingDatas;

            foreach (var fixture in fixtures)
            {
                var prediction = new PredictionComparison
                {
                    PredictionComparisonReference = ViewModel.WebScraper.GetFixtureReference(fixture.HomeTeam, fixture.FixtureDate),

                }
            }

            var predictedScoreDelta = GetPredictedScoreSpread(betData.HomeTeam, betData.AwayTeam, betData.FixtureDate, _db);
            GetTeamToBack(betData.HomeTeam, betData.AwayTeam, predictedScoreDelta, betData.HomeSpread);

            var prediction = new PredictionComparison(betData, _teamToBack, _swerveTeam, predictedScoreDelta);
            prediction.ActualScoreDelta = GetActualScoreDelta(prediction.BettingData.FixtureReference, fixtures);
            prediction.PredictionResult = GetPredictionOutcome(betData, predictedScoreDelta, prediction.ActualScoreDelta);
            prediction.PredictionComparisonReference = WebScraper.GetFixtureReference(prediction.HomeTeam, prediction.BettingData.FixtureDate);
            prediction.BettingData.MatchDataReference = prediction.PredictionComparisonReference;
            PredictedComparisonDataList.Add(prediction);

            return
        }

        private Team? GetTeamToBack(Team? homeTeam, Team? awayTeam, double predictedScoreSpread, double bookScoreSpread)
        {
            var predictedVsBookScoreSpread = predictedScoreSpread - bookScoreSpread;
            return  predictedVsBookScoreSpread < 0 ? homeTeam : awayTeam;
        }

     
        //public PredictionOutcome GetPredictionOutcome(PredictionComparison prediction, Team? teamToBack)
        //{
        //    var actualWinner = prediction.TeamToBack == teamToBack ? teamToBack : prediction.SwerveTeam;
        //    return prediction.TeamToBack == actualWinner ? PredictionOutcome.Success : PredictionOutcome.Fail;
        //}
    }

    internal class FixtureAndBettingData
    {
        private Fixture _fixture;
        private MatchBettingData _bettingData;

        public FixtureAndBettingData(Fixture fixture, MatchBettingData betting)
        {
            _fixture = fixture;
            _bettingData = betting;
        }
    }

}