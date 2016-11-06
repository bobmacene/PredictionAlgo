using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


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
    
        private double GetBookVsPrediction(double predictionDelta, double bookSpread)
        {
            return predictionDelta - bookSpread;
        }
        //public PredictionOutcome GetPredictionOutcome(PredictionComparison prediction, Team? teamToBack)
        //{
        //    var actualWinner = prediction.TeamToBack == teamToBack ? teamToBack : prediction.SwerveTeam;
        //    return prediction.TeamToBack == actualWinner ? PredictionOutcome.Success : PredictionOutcome.Fail;
        //}
    }
}