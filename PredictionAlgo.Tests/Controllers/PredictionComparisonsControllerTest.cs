using System.Web.Mvc;
using NUnit.Framework;
using PredictionAlgo.Controllers;

namespace PredictionAlgo.Tests.Controllers
{
    [TestFixture]
    public class PredictionComparisonsControllerTest
    {
        [Test]
        public void PredictionComparisonsController_IndexNotNull_True()
        {
            using (var controller = new PredictionComparisonsController())
            {
                var result = controller.Index() as ViewResult;

                Assert.IsNotNull(result);
            }
        }

        [Test]
        public void PredictionComparisonsController_PreviousComparionsNotNull_True()
        {
            using (var controller = new PredictionComparisonsController())
            {
                var result = controller.AllPreviousComparisons() as ViewResult;

                Assert.IsNotNull(result);
            }
        }
    }
}
