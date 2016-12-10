//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace PredictionAlgo.Models
//{
//    public class Utility
//    {

//        public void UpdateDatabaseCollection(IEnumerable<object> context ,IEnumerable<object> updateList )
//        {

//            foreach (var prediction in updateList)
//            {
//                if (_db.PredictionComparisons.Any(x => x.PredictionComparisonReference == prediction.PredictionComparisonReference))
//                {
//                    var recordToEdit = _db.PredictionComparisons.FirstOrDefault(
//                            x => x.PredictionComparisonReference == prediction.PredictionComparisonReference);

//                    _db.PredictionComparisons.Remove(recordToEdit);
//                    _db.PredictionComparisons.Add(prediction);
//                }
//                else
//                {
//                    _db.PredictionComparisons.Add(prediction);
//                }
//            }

//            _db.SaveChanges();
//        }
//    }
//}