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

        public float GetAverageHomeScoreLastFiveHomeGames(Team? team, DateTime? date, PredictionAlgoContext context)
        {
            if (team == null || date == null) return 0;

            var homeResults = GetHomeTeamResults(context);

            var numberOfAvailableResults = homeResults[team].Where(X => X.FixtureDate <= date)
            .Select(x => x.HomeScore)
            .Count();

            float averageHomeScore;
            if (numberOfAvailableResults < 5)
            {
                averageHomeScore = (float)homeResults[team].Where(X => X.FixtureDate <= date)
                .Select(x => x.HomeScore).Average();
            }
            else
            {
                averageHomeScore = (float)homeResults[team].Where(X => X.FixtureDate <= date)
                .Take(5)
                .Select(x => x.HomeScore)
                .Average();
            }

            return averageHomeScore;
        }

        public float GetAverageScoreDeltaLastFiveHomeGames(Team? team, DateTime? date, PredictionAlgoContext context)
        {
            if (team == null || date == null) return 0;

            var homeResults = GetHomeTeamResults(context);

            return (float) homeResults[team].Where(X => X.FixtureDate <= date)
                .Take(5)
                .Select(x => x.ScoreDelta)
                .Average();
        }

        public float GetAverageAwayScoreLastFiveAwayGames(Team? team, DateTime? date, PredictionAlgoContext context)
        {
            if (team == null || date == null) return 0;

            var awayResults = GetAwayTeamResults(context);

            return (float) awayResults[team].Where(X => X.FixtureDate <= date)
                .Take(5)
                .Select(x => x.AwayScore)
                .Average();
        }

        public float GetAverageScoreDeltaLastFiveAwayGames(Team? team, DateTime? date, PredictionAlgoContext context)
        {
            if (team == null || date == null) return 0;

            var awayResults = GetAwayTeamResults(context);

            return (float) awayResults[team].Where(X => X.FixtureDate <= date)
                .Take(5)
                .Select(x => x.ScoreDelta)
                .Average();
        }
        public float GetAverageScoreDeltaOfLastTwoResultsBetweenTeams(Team? homeTeam, Team? awayTeam, DateTime? date, PredictionAlgoContext context)
        {
            if (homeTeam == null || awayTeam == null || date == null) return 0;

            var homeResults = GetHomeTeamResults(context);
            var awayResults = GetAwayTeamResults(context);

            var lastHomeResultBtwnTeams = homeResults[homeTeam].Where(X => (X.AwayTeam == awayTeam
               && X.FixtureDate <= date))
               .Take(1)
               .Select(x => x.ScoreDelta);

            var lastAwayResultBtwnTeams = awayResults[awayTeam].Where(x => (x.HomeTeam == homeTeam
                && x.FixtureDate <= date))
                .Select(x => x.ScoreDelta)
                .Take(1);

            return (float)lastHomeResultBtwnTeams.ElementAt(0)
                            + lastAwayResultBtwnTeams.ElementAt(0)
                            / 2;
        }

        public ResultStatistics GetPredictedResult(Team? homeTeam, Team? awayTeam, DateTime? date, PredictionAlgoContext context)
        {
            if (homeTeam == null || awayTeam == null || date == null)
            {
                return new ResultStatistics
                {
                    HomeTeam = null,
                    AwayTeam = null,
                    Date = null,
                    AverageHomeScoreLastFiveHomeGames = 0,
                    AverageScoreDeltaLastFiveHomeGames = 0,
                    AverageAwayScoreLastFiveAwayGames = 0,
                    AverageScoreDeltaLastFiveAwayGames = 0,
                    AverageScoreDeltaOfLastTwoResultsBetweenTeams = 0,
                    PredictedScoreDelta = 0
                };
            }
            var aveHomeScoreLastFiveHomeResults = GetAverageHomeScoreLastFiveHomeGames(homeTeam, date, context);
            var aveAwayScoreLastFiveAwayResults = GetAverageAwayScoreLastFiveAwayGames(awayTeam, date, context);

            var scoreDeltalastFiveHomeResults = GetAverageAwayScoreLastFiveAwayGames(homeTeam, date, context);
            var scoreDeltalastFiveAwayResults = GetAverageScoreDeltaLastFiveAwayGames(awayTeam, date, context);

            var lastTwoResultsBtwnTeams = GetAverageScoreDeltaOfLastTwoResultsBetweenTeams(homeTeam, awayTeam, date, context);

            var predictedDelta = (aveHomeScoreLastFiveHomeResults - aveAwayScoreLastFiveAwayResults
                                                    + scoreDeltalastFiveHomeResults
                                                    + scoreDeltalastFiveAwayResults
                                                    + lastTwoResultsBtwnTeams)
                                                    / 4;
            return new ResultStatistics
            {
                HomeTeam = homeTeam,
                AwayTeam = awayTeam,
                Date = date,
                AverageHomeScoreLastFiveHomeGames = aveHomeScoreLastFiveHomeResults,
                AverageScoreDeltaLastFiveHomeGames = aveAwayScoreLastFiveAwayResults,
                AverageAwayScoreLastFiveAwayGames = scoreDeltalastFiveHomeResults,
                AverageScoreDeltaLastFiveAwayGames = scoreDeltalastFiveAwayResults,
                AverageScoreDeltaOfLastTwoResultsBetweenTeams = lastTwoResultsBtwnTeams,
                PredictedScoreDelta =  predictedDelta
                //PredictedScoreDelta = ApplySpreadChangeForDate(predictedDelta, (DateTime)date)
            };
        }

        public float ApplySpreadChangeForDate(float prediction, DateTime? date)  //weather factor
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

        #region FixtureRange
        //public List<Fixture> GetRangeOfFixtures(PredictionAlgoContext context, DateTime startDate, DateTime endDate)
        //{
        //    return context.Fixtures.Where(fixture => fixture.FixtureDate >= startDate && fixture.FixtureDate <= endDate).ToList();
        //}

        //public List<ResultStatistics> GetPredictedResultList(Team? homeTeam, Team? awayTeam, DateTime? date, PredictionAlgoContext context)
        //{
        //    var aveHomeScoreLastFiveHomeResults = GetAverageHomeScoreLastFiveHomeGames(homeTeam, date, context);
        //    var aveAwayScoreLastFiveAwayResults = GetAverageAwayScoreLastFiveAwayGames(awayTeam, date, context);

        //    var scoreDeltalastFiveHomeResults = GetAverageAwayScoreLastFiveAwayGames(homeTeam, date, context);
        //    var scoreDeltalastFiveAwayResults = GetAverageScoreDeltaLastFiveAwayGames(awayTeam, date, context);

        //    var lastTwoResultsBtwnTeams = GetAverageScoreDeltaOfLastTwoResultsBetweenTeams(homeTeam, awayTeam, date, context);

        //    var predictedDelta = (aveHomeScoreLastFiveHomeResults - aveAwayScoreLastFiveAwayResults
        //                                            + scoreDeltalastFiveHomeResults
        //                                            + scoreDeltalastFiveAwayResults
        //                                            + lastTwoResultsBtwnTeams)
        //                                            / 4;
        //    return new List<ResultStatistics>
        //    {
        //        new ResultStatistics
        //        {
        //            HomeTeam = homeTeam,
        //            AwayTeam = awayTeam,
        //            Date = date,
        //            AverageHomeScoreLastFiveHomeGames = aveHomeScoreLastFiveHomeResults,
        //            AverageScoreDeltaLastFiveHomeGames = aveAwayScoreLastFiveAwayResults,
        //            AverageAwayScoreLastFiveAwayGames = scoreDeltalastFiveHomeResults,
        //            AverageScoreDeltaLastFiveAwayGames = scoreDeltalastFiveAwayResults,
        //            AverageScoreDeltaOfLastTwoResultsBetweenTeams = lastTwoResultsBtwnTeams,
        //            PredictedScoreDelta = predictedDelta
        //        }
        //    };
        //} 
        #endregion

    }
}