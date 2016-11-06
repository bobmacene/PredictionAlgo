using System;
using System.Collections.Generic;
using System.Linq;

namespace PredictionAlgo.Models.ViewModel
{
    public class ResultsSeason2010_2011
    {
        readonly DateTime _seasonStartDate = new DateTime(2010, 06, 01);
        readonly DateTime _seasonEndDate = new DateTime(2011, 07, 01);
        public IEnumerable<Fixture> GetSeasonResults(PredictionAlgoContext context)
        {
            return context.Fixtures.Where(x => x.FixtureDate > _seasonStartDate)
                             .Where(x => x.FixtureDate < _seasonEndDate)
                             .OrderBy(x => x.FixtureDate);
        }
    }

    public class ResultsSeason2011_2012
    {
        DateTime seasonStartDate = new DateTime(2011, 06, 01);
        DateTime seasonEndDate = new DateTime(2012, 07, 01);
        public IEnumerable<Fixture> GetSeasonResults(PredictionAlgoContext context)
        {
            return context.Fixtures.Where(x => x.FixtureDate > seasonStartDate)
                             .Where(x => x.FixtureDate < seasonEndDate)
                             .OrderBy(x => x.FixtureDate);
        }
    }

    public class ResultsSeason2012_2013
    {
        DateTime seasonStartDate = new DateTime(2012, 06, 01);
        DateTime seasonEndDate = new DateTime(2013, 07, 01);
        public IEnumerable<Fixture> GetSeasonResults(PredictionAlgoContext context)
        {
            return context.Fixtures.Where(x => x.FixtureDate > seasonStartDate)
                             .Where(x => x.FixtureDate < seasonEndDate)
                             .OrderBy(x => x.FixtureDate);
        }
    }

    public class ResultsSeason2013_2014
    {
        DateTime seasonStartDate = new DateTime(2013, 06, 01);
        DateTime seasonEndDate = new DateTime(2014, 07, 01);
        public IEnumerable<Fixture> GetSeasonResults(PredictionAlgoContext context)
        {
            return context.Fixtures.Where(x => x.FixtureDate > seasonStartDate)
                             .Where(x => x.FixtureDate < seasonEndDate)
                             .OrderBy(x => x.FixtureDate);
        }
    }
    public class ResultsSeason2014_2015
    {
        DateTime seasonStartDate = new DateTime(2014, 06, 01);
        DateTime seasonEndDate = new DateTime(2015, 07, 01);
        public IEnumerable<Fixture> GetSeasonResults(PredictionAlgoContext context)
        { 
            return context.Fixtures.Where(x => x.FixtureDate > seasonStartDate)
                             .Where(x => x.FixtureDate < seasonEndDate)
                             .OrderBy(x => x.FixtureDate);
        }
    }
    public class ResultsSeason2015_2016
    {
        readonly DateTime _seasonStartDate = new DateTime(2015, 06, 01);
        readonly DateTime _seasonEndDate = new DateTime(2016, 07, 01);
        public IEnumerable<Fixture> GetSeasonResults(PredictionAlgoContext context)
        {
            return context.Fixtures.Where(x => x.FixtureDate > _seasonStartDate)
                           .Where(x => x.FixtureDate < _seasonEndDate)
                           .OrderBy(x => x.FixtureDate);
        }
    }
    public class ResultsSeason2016_2017
    {
        readonly DateTime _seasonStartDate = new DateTime(2016, 06, 01);
        readonly DateTime _seasonEndDate = new DateTime(2017, 07, 01);
        public IEnumerable<Fixture> GetSeasonResults(PredictionAlgoContext context)
        {
            return context.Fixtures.Where(x => x.FixtureDate > _seasonStartDate)
                             .Where(x => x.FixtureDate < _seasonEndDate)
                             .OrderBy(x => x.FixtureDate);
        }
    }

}