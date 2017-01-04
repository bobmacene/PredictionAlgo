using System;

namespace PredictionAlgo.Models
{
    public abstract class CommonFunctions
    {
        //Used in PredictionAlgoDataUpdater to changed historical db data
        public double GetPredictedDelta(double aveHomeScoreLast5HomeResults, double aveAwayScoreLast5AwayResults,
          double deltaLast5HomeResults, double deltaLast5AwayResults, double sameFixturePreviousResult)
        {
            return (aveHomeScoreLast5HomeResults + deltaLast5HomeResults
                    - aveAwayScoreLast5AwayResults + deltaLast5AwayResults
                    + sameFixturePreviousResult) / 2;
        }


        public string GetFixtureReference(Team? homeTeam, DateTime? date)
        {
            var shortDate = date ?? DateTime.Today;
            var dateString = shortDate.ToShortDateString();
            return homeTeam + dateString;
        }

        //Used in PredictionAlgoDataUpdater to changed historical db data
        public double GetPredictedDeltaRandom(double aveHomeScoreLast5HomeResults, double aveAwayScoreLast5AwayResults,
         double deltaLast5HomeResults, double deltaLast5AwayResults, double sameFixturePreviousResult , double random)
        {
            return (aveHomeScoreLast5HomeResults + deltaLast5HomeResults
                    - aveAwayScoreLast5AwayResults + deltaLast5AwayResults
                    + sameFixturePreviousResult) / random;
        }
    }
}

#region WeatherFactor

//Weather factor not proving to increase algo accuracy

//public double GetWeatherPredictedDelta(double aveHomeScoreLast5HomeResults, double aveAwayScoreLast5AwayResults,
//   double deltaLast5HomeResults, double deltaLast5AwayResults, double last2ResultsBtwnTeams,
//   DateTime? date)
//{
//    var predictedDelta = aveHomeScoreLast5HomeResults - aveAwayScoreLast5AwayResults
//        + GetAveDelta(deltaLast5HomeResults, deltaLast5AwayResults)
//        + last2ResultsBtwnTeams
//        / 4;

//    return ApplySpreadChangeForDate(predictedDelta, date);
//}

//public double ApplySpreadChangeForDate(double prediction, DateTime? date)  //weather factor
//{
//    switch (date?.Month)
//    {
//        case 9:
//        case 3:
//        case 4:
//            return prediction * 0.35;
//        case 5:
//        case 6:
//            return prediction * 0.45;
//        case 2:
//        case 11:
//        case 10:
//            return prediction * 0.05;
//        case 1:
//        case 12:
//            return prediction * 0.025;
//    }
//    return prediction;
//}

//public double GetAveDelta(double homeDelta, double awayDelta)
//{
//    double delta = 0;

//    if (homeDelta > 0 && awayDelta > 0)
//        delta = homeDelta - awayDelta;
//    if (homeDelta > 0 && awayDelta < 0)
//        delta = homeDelta + awayDelta;
//    if (homeDelta < 0 && awayDelta < 0)
//        delta = homeDelta - awayDelta;
//    if (homeDelta < 0 && awayDelta > 0)
//        delta = homeDelta + awayDelta;

//    return delta;
//}


#endregion