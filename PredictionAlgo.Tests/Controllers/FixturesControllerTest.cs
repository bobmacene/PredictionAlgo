
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
                var result = controller.ResultsSeason2015_2016() as ViewResult;

                Assert.IsNotNull(result);
            }
        }

        [Test]
        public void FixturesController_ResultsSeason2014_2015NotNull_True()
        {
            using (var controller = new FixturesController())
            {
                var result = controller.ResultsSeason2014_2015() as ViewResult;

                Assert.IsNotNull(result);
            }
        }

        [Test]
        public void FixturesController_ResultsSeason2013_2014NotNull_True()
        {
            using (var controller = new FixturesController())
            {
                var result = controller.ResultsSeason2013_2014() as ViewResult;

                Assert.IsNotNull(result);
            }
        }

        [Test]
        public void FixturesController_ResultsSeason2012_2013NotNull_True()
        {
            using (var controller = new FixturesController())
            {
                var result = controller.ResultsSeason2012_2013() as ViewResult;

                Assert.IsNotNull(result);
            }
        }

        [Test]
        public void FixturesController_ResultsSeason2011_2012NotNull_True()
        {
            using (var controller = new FixturesController())
            {
                var result = controller.ResultsSeason2011_2012() as ViewResult;

                Assert.IsNotNull(result);
            }
        }

        [Test]
        public void FixturesController_ResultsSeason2010_2011NotNull_True()
        {
            using (var controller = new FixturesController())
            {
                var result = controller.ResultsSeason2010_2011() as ViewResult;

                Assert.IsNotNull(result);
            }
        }
    }

}
