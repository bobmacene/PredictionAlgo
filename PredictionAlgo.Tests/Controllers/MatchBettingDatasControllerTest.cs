
using System.Web.Mvc;
using NUnit.Framework;
using PredictionAlgo.Controllers;

namespace PredictionAlgo.Tests.Controllers
{
    [TestFixture]
    public class MatchBettingDatasControllerTest
    {
        [Test]
        public void MatchBettingDatasController_IndexNotNull_True()
        {
            using (var controller = new MatchBettingDatasController())
            {
                var result = controller.Index(null) as ViewResult;

                Assert.IsNotNull(result);
            }
        }

        [Test]
        public void MatchBettingDatasController_BettingData_2017NotNull_True()
        {
            using (var controller = new MatchBettingDatasController())
            {
                var result = controller.GetBettingData(null) as ViewResult;

                Assert.IsNotNull(result);
            }
        }
    }
}
