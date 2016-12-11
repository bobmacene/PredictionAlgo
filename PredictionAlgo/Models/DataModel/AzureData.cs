using System.Collections.Generic;
using System.Linq;

namespace PredictionAlgo.Models.DataModel
{
    //Use this class to update Azure tables with missing data
    public class AzureData
    {
        private readonly PredictionAlgoContext _db = new PredictionAlgoContext();

        public void UpdateAzure()  //Referenced in MatchBettingDataController
        {
            IEnumerable<MatchBettingData> betDataUpdates = new[]
            {
                new MatchBettingData("BenettonTreviso9/23/2016", 0, 1, 0.91, 7, -1, 0.91, "2016-09-23 01:00:00"),
                new MatchBettingData("Connacht10/7/2016", 2, 5, 0.91, 10, -5, 0.91, "2016-10-07 01:00:00"),
                new MatchBettingData("Edinburgh10/7/2016", 3, -14, 0.91, 0, 14, 0.91, "2016-10-07 01:00:00"),
                new MatchBettingData("GlasgowWarriors9/23/2016", 4, -3, 0.91, 10, 3, 0.91, "2016-09-23 01:00:00"),
                new MatchBettingData("Leinster10/8/2016", 5, -4, 0.91, 6, 4, 0.91, "2016-10-08 01:00:00"),
                new MatchBettingData("Leinster9/23/2016", 5, -6, 0.91, 8, 6, 0.91, "2016-09-23 01:00:00"),
                new MatchBettingData("Munster9/24/2016", 6, -10, 0.91, 3, 10, 0.91, "2016-09-24 01:00:00"),
                new MatchBettingData("Ospreys10/7/2016", 8, -7, 0.91, 1, 7, 0.91, "2016-10-07 01:00:00"),
                new MatchBettingData("Scarlets10/8/2016", 9, -12, 0.91, 7, 12, 0.91, "2016-10-08 01:00:00"),
                new MatchBettingData("Zebre10/8/2016", 11, 21, 0.91, 4, -21, 0.91, "2016-10-08 01:00:00")
            };

            var references = _db.MatchBettingDatas.Select(x => x.FixtureReference);

            foreach (var bet in betDataUpdates)
            {
                if (references.Contains(bet.FixtureReference)) continue;

                _db.MatchBettingDatas.Add(bet);
            }

            _db.SaveChanges();
        }

    }
}