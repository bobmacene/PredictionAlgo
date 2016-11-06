 using PredictionAlgo.Models.ViewModel;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PredictionAlgo.Models
{
    public class ResultStatistics
    {     
        [Key] 
        public int StatisticReference { get; set; }
        [DisplayName("Home Team")]
        public Team? HomeTeam { get;set; }
        [DisplayName("Away Team")]
        public Team? AwayTeam { get; set; }
        [DisplayName("Stats At This Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy MM dd}")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
        [DisplayName("Average Home Score Last Five Home Games")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public float AverageHomeScoreLastFiveHomeGames { get; set; }
        [DisplayName("Average Delta Last Five Home Games")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public float AverageScoreDeltaLastFiveHomeGames { get; set; }
        [DisplayName("Average Away Score Last Five Away Games")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public float AverageAwayScoreLastFiveAwayGames { get; set; }
        [DisplayName("Average Delta Last Five Away Games")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public float AverageScoreDeltaLastFiveAwayGames { get; set; }
        [DisplayName("Average Delta Of Last Two Results Btwn Teams")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public float AverageScoreDeltaOfLastTwoResultsBetweenTeams { get; set; }
        [DisplayName("Predicted Delta")]
        public float PredictedScoreDelta { get; set; }
        [DisplayName("Home Team")]
        [Required(ErrorMessage = "Required Field")]
        public Team? SelectHomeTeam { get; set; }
        [DisplayName("Away Team")]
        [Required(ErrorMessage = "Required Field")]
        public Team? SelectAwayTeam { get; set; }
        [Required(ErrorMessage = "Required Field")]
        [DisplayName("Stats At This Date")]
        [DataType(DataType.Date)]
        public DateTime? SelectDate { get; set; }
        public string FixtureReference { get; set; } //common reference

        public ResultStatistics FixturePredictedResult { get; set; }

        private readonly PredictionAlgoContext _db = new PredictionAlgoContext();
        private readonly PredictedResult _predictedResult = new PredictedResult();

        public float GetAverageHomeScoreLastFiveHomeGames =>
            _predictedResult.GetAverageHomeScoreLastFiveHomeGames(SelectHomeTeam, SelectDate, _db);

        public float GetAverageScoreDeltaLastFiveHomeGames => 
            _predictedResult.GetAverageScoreDeltaLastFiveHomeGames(SelectHomeTeam, SelectDate, _db);

        public float GetAverageAwayScoreLastFiveAwayGames => 
            _predictedResult.GetAverageAwayScoreLastFiveAwayGames(SelectAwayTeam, SelectDate, _db);

        public float GetAverageScoreDeltaLastFiveAwayGames => 
            _predictedResult.GetAverageScoreDeltaLastFiveAwayGames(SelectAwayTeam, SelectDate, _db);

        public float GetAverageScoreDeltaOfLastTwoResultsBetweenTeams => 
            _predictedResult.GetAverageScoreDeltaOfLastTwoResultsBetweenTeams(SelectHomeTeam, SelectAwayTeam, SelectDate, _db);

        public float GetPredictedScoreDelta
        {
            get
            {
                var result = _predictedResult.GetPredictedResult(SelectHomeTeam, SelectAwayTeam, SelectDate, _db);
                return result.PredictedScoreDelta;
            }
        }
        public string GetFixtureReference => WebScraper.GetFixtureReference(SelectHomeTeam, SelectDate);
    }
}
