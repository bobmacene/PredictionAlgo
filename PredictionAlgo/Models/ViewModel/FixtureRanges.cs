using System;
using System.Collections.Generic;
using System.Linq;

namespace PredictionAlgo.Models.ViewModel
{
    public class FixtureRanges
    {     
        public List<Fixture> GetLastFiveFixturesByTeam(PredictionAlgoContext context, Team? team, DateTime? date)
        {
            if (team == null || date == null)
            {
                return new List<Fixture>
                    {
                        new Fixture
                            {
                                FixtureDate = null,
                                result = null,
                                HomeTeam = null,
                                HomeScore = 0,
                                AwayTeam = null,
                                AwayScore = 0,
                                ScoreDelta = 0,
                                PredictedDelta = 0,
                                ActualVersusPredictedDelta= 0,
                                PredictedResult = null,
                                PredictionOutcome = null
                            }
                    };
            }

            return context.Fixtures.Where(x => x.HomeTeam == team || x.AwayTeam == team && x.FixtureDate <= date)
            .OrderBy(x => x.FixtureDate)
            .Take(5)
            .ToList();
        }

        public List<Fixture> GetLastTwoFixturesBtwnTeams(PredictionAlgoContext context, Team? homeTeam, Team? awayTeam, DateTime? date)
        {
            if (homeTeam == null || awayTeam == null || date == null)
            {
                return new List<Fixture>
                    {
                        new Fixture
                            {
                                FixtureDate = null,
                                result = null,
                                HomeTeam = null,
                                HomeScore = 0,
                                AwayTeam = null,
                                AwayScore = 0,
                                ScoreDelta = 0,
                                PredictedDelta = 0,
                                ActualVersusPredictedDelta= 0,
                                PredictedResult = null,
                                PredictionOutcome = null
                            }
                    };
            }
            return context.Fixtures.Where(x => x.HomeTeam == homeTeam && x.AwayTeam == awayTeam
                || x.HomeTeam == awayTeam && x.AwayTeam == homeTeam && x.FixtureDate <= date)
            .OrderBy(x => x.FixtureDate)
            .Take(2)
            .ToList();
        }
    }
}

