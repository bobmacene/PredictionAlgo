﻿using System;
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
        [DisplayName("Fixture Date")]
        public DateTime? FixtureDate { get; set; }

        [DisplayName("Predicted Delta")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double PredictionDelta { get; set; }

        [DisplayName("Book v Predicted")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double BookVsPrediction { get; set; }

        [DisplayName("Actual Delta")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double ActualScoreDelta { get; set; }

        [DisplayName("Team To Back ")]
        public Team? TeamToBack { get; set; }

        public Team? SwerveTeam { get; set; }

        public PredictionOutcome PredictionResult { get; set; }

        public Team? HomeTeam { get; set; }

        public Team? AwayTeam { get; set; }

        public double HomeSpread { get; set; }

        public double AwaySpread { get; set; }

        public MatchBettingData BettingData { get; }

        public PredictionComparison() { } //CsvHelper serialization


        public PredictionComparison(
            MatchBettingData bettingData, Team? teamToBack, Team? swerveTeam, double predictionDelta)
        {
            FixtureDate = bettingData.FixtureDate;
            HomeTeam = bettingData.HomeTeam;
            HomeSpread = bettingData.HomeSpread;
            AwayTeam = bettingData.AwayTeam;
            AwaySpread = bettingData.AwaySpread;
            BettingData = bettingData;
            TeamToBack = teamToBack;
            SwerveTeam = swerveTeam;
            PredictionDelta = predictionDelta;
            BookVsPrediction = predictionDelta + bettingData.HomeSpread;
            TimeStamp = DateTime.Now;
        }
    
    }
}