
using System.Web.Mvc;
using NUnit.Framework;
using PredictionAlgo.Controllers;
using PredictionAlgo.Models;

namespace PredictionAlgo.Tests.Controllers
{
    [TestFixture]
    public class ResultStatisticsControllerTest
    {
        [Test]
        public void ResultStatisticsController_IndexNotNull_True()
        {
            using (var controller = new ResultStatisticsController())
            {
                var result = controller.Index() as ViewResult;

                Assert.IsNotNull(result);
            }
        }

        [Test]
        public void ResultStatisticsController_ResultNotNull_True()
        {
            using (var controller = new ResultStatisticsController())
            {
                var result = controller.PredictResult() as ViewResult;

                Assert.IsNotNull(result);
            }
        }

        [Test]
        public void ResultStatisticsController_PostNotNull_True()
        {
            using (var controller = new ResultStatisticsController())
            {
                using (var resultStatistics = new ResultStatistics())
                {
                    var result = controller.PredictResult(resultStatistics) as ViewResult;

                    Assert.IsNotNull(result);
                }
            }
        }
    }
}
