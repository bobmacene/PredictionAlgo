using System;
using System.Collections.Generic;
using System.Linq;

namespace PredictionAlgo.Models.ViewModel
{
    public class PredictedResult
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

        public double GetAveHomeScoreLast5HomeGames(Team? team, DateTime? date, PredictionAlgoContext context)
        {
            if (team == null || date == null) return 0;

            var homeResults = GetHomeTeamResults(context);

            return homeResults[team].Where(x => x.FixtureDate <= date)
                .Take(5)
                .Select(x => x.HomeScore)
                .Average(); 
        }

        public double GetAveDeltaLast5HomeGames(Team? team, DateTime? date, PredictionAlgoContext context)
        {
            if (team == null || date == null) return 0;

            var homeResults = GetHomeTeamResults(context);

            return homeResults[team].Where(x => x.FixtureDate <= date)
                .Take(5)
                .Select(x => x.ScoreDelta)
                .Average();
        }

        public double GetAveAwayScoreLast5AwayGames(Team? team, DateTime? date, PredictionAlgoContext context)
        {
            if (team == null || date == null) return 0;

            var awayResults = GetAwayTeamResults(context);

            return awayResults[team].Where(x => x.FixtureDate <= date)
                .Take(5)
                .Select(x => x.AwayScore)
                .Average();
        }

        public double GetAveDeltaLast5AwayGames(Team? team, DateTime? date, PredictionAlgoContext context)
        {
            if (team == null || date == null) return 0;

            var awayResults = GetAwayTeamResults(context);

            return awayResults[team].Where(x => x.FixtureDate <= date)
                .Take(5)
                .Select(x => x.ScoreDelta)
                .Average();
        }


        public double GetAveDeltaLast2ResultsBtwnTeams(Team? homeTeam, Team? awayTeam, DateTime? date, PredictionAlgoContext context)
        {
            if (homeTeam == null || awayTeam == null || date == null) return 0;

            var homeResults = GetHomeTeamResults(context);
            var awayResults = GetAwayTeamResults(context);

            var lastHomeResultBtwnTeams = homeResults[homeTeam].Where(x => (x.AwayTeam == awayTeam
               && x.FixtureDate <= date))
               .Take(1)
               .Select(x => x.ScoreDelta);

            var lastAwayResultBtwnTeams = awayResults[awayTeam].Where(x => (x.HomeTeam == homeTeam
                && x.FixtureDate <= date))
                .Select(x => x.ScoreDelta)
                .Take(1);

            return lastHomeResultBtwnTeams.ElementAt(0) + lastAwayResultBtwnTeams.ElementAt(0) / 2;
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
                AveDeltaLast2ResultsBtwnTeams = GetAveDeltaLast2ResultsBtwnTeams(homeTeam, awayTeam, date, context),
                PredictedDelta =  predictedDelta
            };
        }

        public double GetPredictedDelta(Team? homeTeam, Team? awayTeam, DateTime? date, PredictionAlgoContext context)
        {
            var aveHomeScoreLast5HomeResults = GetAveHomeScoreLast5HomeGames(homeTeam, date, context);
            var aveAwayScoreLast5AwayResults = GetAveAwayScoreLast5AwayGames(awayTeam, date, context);

            var deltaLast5HomeResults = GetAveDeltaLast5HomeGames(homeTeam, date, context);
            var deltaLast5AwayResults = GetAveDeltaLast5AwayGames(awayTeam, date, context);

            var last2ResultsBtwnTeams = GetAveDeltaLast2ResultsBtwnTeams(homeTeam, awayTeam, date, context);

            double predictedDelta;

           // if (aveHomeScoreLast5HomeResults > 0 && deltaLast5HomeResults > 0)
           // {
                predictedDelta = (aveHomeScoreLast5HomeResults - aveAwayScoreLast5AwayResults
                //+ deltaLast5HomeResults - deltaLast5AwayResults
                + last2ResultsBtwnTeams)
                / 2;
           // }
                

           // var weatherResult = ApplySpreadChangeForDate(predictedDelta, date);
            return predictedDelta;
        }


        public double ApplySpreadChangeForDate(double prediction, DateTime? date)  //weather factor
        {
            switch (date?.Month)
            {
                case 9:
                case 3:
                case 4:
                    return prediction * (float)0.35;
                case 5:
                case 6:
                    return prediction * (float)0.45;
                case 2:
                case 11:
                case 10:
                    return prediction * (float)0.05;
                case 1:
                case 12:
                    return prediction * (float)0.025;
            }
            return prediction;
        }

    }
}