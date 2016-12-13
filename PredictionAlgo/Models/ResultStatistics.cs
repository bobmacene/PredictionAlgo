using PredictionAlgo.Models.ViewModel;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PredictionAlgo.Models
{
    public class ResultStatistics : IDisposable
    {
        [Key]
        public int StatisticReference { get; set; }

        [DisplayName("Home Team")]
        public Team? HomeTeam { get; set; }

        [DisplayName("Away Team")]
        public Team? AwayTeam { get; set; }

        [DisplayName("Stats At This Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy MM dd}")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [DisplayName("Ave Home Score Last 5 Home Games")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double AveHomeScoreLast5HomeGames { get; set; }

        [DisplayName("Ave Delta Last 5 Home Games")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double AveDeltaLast5HomeGames { get; set; }

        [DisplayName("Ave Away Score Last 5 Away Games")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double AveAwayScoreLast5AwayGames { get; set; }

        [DisplayName("Ave Delta Last 5 Away Games")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double AveDeltaLast5AwayGames { get; set; }

        [DisplayName("Ave Delta Last 2 Results Btwn Teams")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double SameFixturePreviousResult { get; set; }

        [DisplayName("Predicted Delta")]
        public double PredictedScoreDelta { get; set; }

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

        public double GetAverageHomeScoreLastFiveHomeGames =>
            _predictedResult.GetAveHomeScoreLast5HomeGames(SelectHomeTeam, SelectDate, _db);

        public double GetAverageScoreDeltaLastFiveHomeGames =>
            _predictedResult.GetAveHomeScoreLast5HomeGames(SelectHomeTeam, SelectDate, _db);

        public double GetAverageAwayScoreLastFiveAwayGames =>
            _predictedResult.GetAveAwayScoreLast5AwayGames(SelectAwayTeam, SelectDate, _db);

        public double GetAverageScoreDeltaLastFiveAwayGames =>
            _predictedResult.GetAveDeltaLast5AwayGames(SelectAwayTeam, SelectDate, _db);

        public double GetSameFixturePreviousResult =>
            _predictedResult.GetSameFixturePreviousResult(SelectHomeTeam, SelectAwayTeam, SelectDate, _db);

        public double GetPredictedScoreDelta
        {
            get
            {
                var result = _predictedResult.GetPredictedResult(SelectHomeTeam, SelectAwayTeam, SelectDate, _db);
                return result.PredictedScoreDelta;
            }
        }

        public void Dispose()
        {
            _db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
