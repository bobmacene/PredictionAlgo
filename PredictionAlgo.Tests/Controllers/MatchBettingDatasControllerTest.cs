
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
                var result = controller.Index() as ViewResult;

                Assert.IsNotNull(result);
            }
        }

        [Test]
        public void MatchBettingDatasController_BettingData_2016NotNull_True()
        {
            using (var controller = new MatchBettingDatasController())
            {
                var result = controller.GetBettingData() as ViewResult;

                Assert.IsNotNull(result);
            }
        }

       
    }
}
