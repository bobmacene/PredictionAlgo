using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PredictionAlgo.Models
{
    public class Fixture
    {
        [Key]
        public string FixtureReference { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy MM dd}")]
        [DataType(DataType.Date)]
        [DisplayName("Fixture Date ")]
        public DateTime? FixtureDate { get; set; }
        [DisplayName("Result ")]
        public Result? Result { get; set; }
        [DisplayName("Home Team ")]
        public Team? HomeTeam { get; set; }
        [DisplayName("Home Score ")]
        public int HomeScore { get; set; }
        [DisplayName("Away Team ")]
        public Team? AwayTeam { get; set; }
        [DisplayName("Away Score ")]
        public int AwayScore { get; set; }
        [DisplayName("Score Delta ")]
        public int ScoreDelta { get; set; }
        [DisplayName("Predicted Delta ")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double PredictedDelta { get; set; }
        [DisplayName("Predicted Result ")]
        public Result? PredictedResult { get; set; }
        [DisplayName("Prediction Outcome ")]
        public PredictionOutcome? PredictionOutcome { get; set; }
        public Competition? Competition { get; set; }
        [DisplayName("Home Team")]
        public Team? SelectHomeTeam { get; set; }
        [DisplayName("Away Team")]
        public Team? SelectAwayTeam { get; set; }
        [DisplayName("Stats At This Date")]
        [DataType(DataType.Date)]
        public DateTime? SelectDate { get; set; }
        public string CommonReference { get; set; }
    }

    public enum Result { HomeWin = 1, Draw = 0, HomeLoss = -1, NoPrectiction = -2 }
    public enum PredictionOutcome { Success = 0, Fail = 1 }
    public enum Competition { Pro12,  Mixed }

    public enum Team
    {
        BenettonTreviso, CardiffBlues, Connacht, Edinburgh,
        GlasgowWarriors, Leinster, Munster, Dragons,
        Ospreys, Scarlets, Ulster, Zebre, Aironi, None
    }

    public static class Teams
    {
        public static IEnumerable<Team> Pro12Teams => new List<Team>
        {
            Team.BenettonTreviso, Team.CardiffBlues, Team.Connacht, Team.Edinburgh,
            Team.GlasgowWarriors, Team.Leinster, Team.Munster, Team.Dragons,
            Team.Ospreys, Team.Scarlets, Team.Ulster, Team.Zebre, Team.Aironi
        };   
    }
 }


#region Top14TeamData

//Team.Toulon, Team.LaRochelle, Team.BordeauxBegles, Team.Grenoble, Team.Montpellier,
//Team.Toulouse, Team.StadeFrancais, Team.ClermontAuvergne, Team.Agen, Team.Oyonnax,
//Team.Pau, Team.Racing92, Team.Brive, Team.Castres, Team.LyonAU, Team.Bayonne
//Toulon, LaRochelle, BordeauxBegles, Grenoble, Montpellier, Toulouse, StadeFrancais, ClermontAuvergne,
        //Agen, Oyonnax, Pau, Racing92, Brive, Castres, LyonAU, Bayonne, None

#endregion