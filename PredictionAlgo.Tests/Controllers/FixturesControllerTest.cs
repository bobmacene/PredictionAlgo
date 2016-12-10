
using System.Web.Mvc;
using NUnit.Framework;
using PredictionAlgo.Controllers;

namespace PredictionAlgo.Tests.Controllers
{
    [TestFixture]
    public class FixturesControllerTest
    {

        [Test]
        public void FixturesController_ResultsSeason2016_2017NotNull_True()
        {
            using (var controller = new FixturesController())
            {
                var result = controller.ResultsSeason2016_2017(string.Empty) as ViewResult;

                Assert.IsNotNull(result);
            }
        }

        [Test]
        public void FixturesController_ResultsSeason2015_2016NotNull_True()
        {
            using (var controller = new FixturesController())
            {
                var result = controller.ResultsSeason2015_2016(string.Empty) as ViewResult;

                Assert.IsNotNull(result);
            }
        }
    }
}
