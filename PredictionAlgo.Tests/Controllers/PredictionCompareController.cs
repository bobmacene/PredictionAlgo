
using System.Web.Mvc;
using NUnit.Framework;
using PredictionAlgo.Controllers;

namespace PredictionAlgo.Tests.Controllers
{
    [TestFixture]
    public class PredictionCompareController
    {
        [Test]
        public void PredictionComparisonsController_IndexNotNull_True()
        {
            using (var controller = new PredictionComparisonsController())
            {
                var result = controller.Index(string.Empty) as ViewResult;

                Assert.IsNotNull(result);
            }
        }


    }
}
