using System;
using System.Collections.Generic;
using System.Linq;
using PredictionAlgo.Models.ViewModel;

namespace PredictionAlgo.Models.DataModel
{
    public class FixtureData
    {

        private const string Pro12Fixtures = "http://www.pro12rugby.com/fixtures/";
        private readonly string _uriPro12Fixtures = new Uri(Pro12Fixtures).AbsoluteUri;
        private readonly PredictionAlgoContext _db = new PredictionAlgoContext();


        public IEnumerable<Fixture> GetFixturesAndResults
        {
            get
            {
                var existingFixtureReferences = _db.Fixtures.Select(x => x.FixtureReference);


                var webScraper = new WebScraper();
                var scrapedFixturesAndResultsList = webScraper.GetFixturesAndResults(_uriPro12Fixtures)
                        .GroupBy(x => x.FixtureReference)
                        .Select(x => x.First());


                foreach (var fixture in scrapedFixturesAndResultsList)
                {
                    if (existingFixtureReferences.Contains(fixture.FixtureReference))
                    {
                        var fixtureToEdit = _db.Fixtures.FirstOrDefault(x => x.FixtureReference == fixture.FixtureReference);
                        _db.Fixtures.Remove(fixtureToEdit);
                        _db.Fixtures.Add(fixture);
                    }
                    else
                    {
                        _db.Fixtures.Add(fixture);
                    }
                }

                _db.SaveChanges();

                return scrapedFixturesAndResultsList;
            }
        }
        
        public double GetResultsWithoutSpreadsPredictionSuccessRate
        {
            get
            {
                double numberOfSuccessPredictions = 
                    _db.Fixtures.Count(x => x.PredictionOutcome == PredictionOutcome.Success);


                double totalPredictionCount = _db.Fixtures
                    .Where(x => x.ScoreDelta != 0)
                    .GroupBy(x => x.FixtureReference)
                    .Count();


                return Math.Round(numberOfSuccessPredictions / totalPredictionCount * 100, 1);
            }
        }

    }
}