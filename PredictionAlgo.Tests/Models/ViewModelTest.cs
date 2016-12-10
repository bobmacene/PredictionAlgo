using System;
using System.Linq;
using NUnit.Framework;
using PredictionAlgo.Models;
using PredictionAlgo.Models.ViewModel;

namespace PredictionAlgo.Tests.Models
{

    [TestFixture]
    public class ViewModelTest : IDisposable
    {
        private readonly PredictionAlgoContext _db = new PredictionAlgoContext();
        private ResultsSeason2016_2017 _fixture2016To2017;


        [SetUp]
        public void SetUp()
        {
            _fixture2016To2017 = new ResultsSeason2016_2017();
        }


        [Test]
        public void ResultsBySeasonCollection_CollectionsAreEqual_True()
        {
            var fixtures = _fixture2016To2017.GetSeasonResults(_db);

            var fixturesFromDb = _db.Fixtures.Where(x => x.FixtureDate > new DateTime(2016, 7, 1))
                .OrderBy(x => x.FixtureDate);

            CollectionAssert.AreEqual(fixtures, fixturesFromDb);
        }


        [Test]
        public void ResultsBySeasonCollection_TestCollectionSize_True()
        {
            var fixturesFromDb = _db.Fixtures
                .Where(x => x.FixtureDate > new DateTime(2015, 7, 1) && x.FixtureDate < new DateTime(2016, 7, 1))
                .OrderBy(x => x.FixtureDate);

            Assert.AreEqual(fixturesFromDb.Count(), 135);
        }

        [TearDown]
        public void TearDown()
        {
            _fixture2016To2017 = null;
        }

        public void Dispose()
        {
            _db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
