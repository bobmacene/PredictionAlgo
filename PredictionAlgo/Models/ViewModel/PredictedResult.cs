using System;
using System.Collections.Generic;
using System.Linq;

namespace PredictionAlgo.Models.ViewModel
{
    public class PredictedResult : CommonFunctions
    {

        private static IDictionary<Team?, ICollection<Fixture>> GetHomeTeamResults(PredictionAlgoContext context)
        {
            var teamResults = new Dictionary<Team?, ICollection<Fixture>>(context.Fixtures.Count() / Teams.Pro12Teams.Count());

            foreach (var fixture in context.Fixtures)
            {
                if (fixture.HomeTeam != null && teamResults.ContainsKey(fixture.HomeTeam))
                {
                    teamResults[fixture.HomeTeam].Add(fixture);
                }
                else
                {
                    if (fixture.HomeTeam != null) teamResults.Add(fixture.HomeTeam, new List<Fixture> { fixture });
                }
            }
            return teamResults;
        }

        private static IDictionary<Team?, ICollection<Fixture>> GetAwayTeamResults(PredictionAlgoContext context)
        {
            var teamResults = new Dictionary<Team?, ICollection<Fixture>>(context.Fixtures.Count() / Teams.Pro12Teams.Count());

            foreach (var fixture in context.Fixtures)
            {
                if (fixture == null) continue;
                if (fixture.AwayTeam != null && teamResults.ContainsKey(fixture.AwayTeam))
                {
                    teamResults[fixture.AwayTeam].Add(fixture);
                }
                else
                {
                    if (fixture.AwayTeam != null) teamResults.Add(fixture.AwayTeam, new List<Fixture> { fixture });
                }
            }
            return teamResults;
        }

        private static readonly PredictionAlgoContext Context = new PredictionAlgoContext();
        private static readonly IDictionary<Team?, ICollection<Fixture>> AllFixturesByHomeTeam = GetHomeTeamResults(Context);
        private static readonly IDictionary<Team?, ICollection<Fixture>> AllFixturesByAwayTeam = GetAwayTeamResults(Context);

        public double GetAveHomeScoreLast5HomeGames(Team? team, DateTime? date, PredictionAlgoContext context)
        {
            if (team == null || date == null) return 0;

            return AllFixturesByHomeTeam[team].Where(x => x.FixtureDate <= date)
                .Take(5)
                .Select(x => x.HomeScore)
                .Average(); 
        }

        public double GetAveDeltaLast5HomeGames(Team? team, DateTime? date, PredictionAlgoContext context)
        {
            if (team == null || date == null) return 0;

            return AllFixturesByHomeTeam[team].Where(x => x.FixtureDate <= date)
                .Take(5)
                .Select(x => x.ScoreDelta)
                .Average();
        }

        public double GetAveAwayScoreLast5AwayGames(Team? team, DateTime? date, PredictionAlgoContext context)
        {
            if (team == null || date == null) return 0;

            return AllFixturesByAwayTeam[team].Where(x => x.FixtureDate <= date)
                .Take(5)
                .Select(x => x.AwayScore)
                .Average();
        }

        public double GetAveDeltaLast5AwayGames(Team? team, DateTime? date, PredictionAlgoContext context)
        {
            if (team == null || date == null) return 0;
            
            return AllFixturesByAwayTeam[team].Where(x => x.FixtureDate <= date)
                .Take(5)
                .Select(x => x.ScoreDelta)
                .Average();
        }

        public double GetSameFixturePreviousResult(Team? homeTeam, Team? awayTeam, DateTime? date, PredictionAlgoContext context)
        {
            if (homeTeam == null || awayTeam == null || date == null) return 0;

            var lastHomeResultBtwnTeams = AllFixturesByHomeTeam[homeTeam].Where(x => (x.AwayTeam == awayTeam
               && x.FixtureDate <= date))
               .Take(1)
               .Select(x => x.ScoreDelta);

            return lastHomeResultBtwnTeams.ElementAt(0);
        }


        public ResultStatistics GetPredictedResult(Team? homeTeam, Team? awayTeam, DateTime? date, PredictionAlgoContext context)
        {
            var predictedDelta = GetPredictedDelta(homeTeam, awayTeam, date, context);

            return new ResultStatistics
            {
                HomeTeam = homeTeam,
                AwayTeam = awayTeam,
                Date = date,
                AveHomeScoreLast5HomeGames = GetAveHomeScoreLast5HomeGames(homeTeam, date, context),
                AveDeltaLast5HomeGames = GetAveDeltaLast5HomeGames(homeTeam, date, context),
                AveAwayScoreLast5AwayGames = GetAveAwayScoreLast5AwayGames(awayTeam, date, context),
                AveDeltaLast5AwayGames = GetAveDeltaLast5AwayGames(awayTeam, date, context),
                SameFixturePreviousResult = GetSameFixturePreviousResult(homeTeam, awayTeam, date, context),
                PredictedScoreDelta =  predictedDelta
            };
        }

        public double GetPredictedDelta(Team? homeTeam, Team? awayTeam, DateTime? date, PredictionAlgoContext context)
        {
            var aveHomeScoreLast5HomeResults = GetAveHomeScoreLast5HomeGames(homeTeam, date, context);
            var aveAwayScoreLast5AwayResults = GetAveAwayScoreLast5AwayGames(awayTeam, date, context);

            var deltaLast5HomeResults = GetAveDeltaLast5HomeGames(homeTeam, date, context);
            var deltaLast5AwayResults = GetAveDeltaLast5AwayGames(awayTeam, date, context);

            var sameFixturePreviousResult = GetSameFixturePreviousResult(homeTeam, awayTeam, date, context);

            return (aveHomeScoreLast5HomeResults + deltaLast5HomeResults
                     - aveAwayScoreLast5AwayResults + deltaLast5AwayResults
                     + sameFixturePreviousResult) / .5;
        }

    }
}