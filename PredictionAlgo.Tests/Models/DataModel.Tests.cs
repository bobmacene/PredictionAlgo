using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using PredictionAlgo.Models;
using PredictionAlgo.Models.DataModel;
using PredictionAlgo.Models.ViewModel;

namespace PredictionAlgo.Tests.Models
{
    /// <summary>
    /// Summary description for Webscraper
    /// </summary>
    [TestFixture]
    public class DataModel
    {
        private IEnumerable<MatchBettingData> _matchBettingDataList;
        private string _uriHCapCouponFake;
        private WebScraper _fakeWebscraper;
        private PredictionComparisonData _predictedData;
        private IEnumerable<PredictionComparison> _predictionComparisons;
        private PredictionAlgoContext _context;
        private FixtureData _fixtureData;

        [SetUp]
        public void Setup()
        {
            var testDataPath = Assembly.GetCallingAssembly().CodeBase;
            testDataPath = testDataPath.Replace(".Tests/bin/Debug/nunit.framework.DLL", 
                "\\bin\\TestData\\2016.10.25_HCapCoupon - Paddy Power.html");

            _uriHCapCouponFake = new Uri(testDataPath).LocalPath;
           
            _fakeWebscraper = new WebScraper();
            _matchBettingDataList = _fakeWebscraper.GetMatchBettingData(_fakeWebscraper.GetSpreadsAndOdds(_uriHCapCouponFake));

            _predictedData = new PredictionComparisonData();
            _predictionComparisons = _predictedData.GetPredictionComparisons(_matchBettingDataList);

            _context = new PredictionAlgoContext();

            _fixtureData = new FixtureData();
        }


        [Test]
        public void A_Webscraper_ReadWebPageObjectCorrectly_True()
        {
            Assert.AreEqual(_matchBettingDataList.Count(), 6);
        }


        [Test]
        public void MatchBettingData_ObjectTypeCorrect_False()
        {
            var typeIsCorrectList =
                _matchBettingDataList.Select(data => data.GetType() == typeof(MatchBettingData));
            var allAreTypeMatchBettingData = typeIsCorrectList.Any(v => v == false);

            Assert.AreEqual(allAreTypeMatchBettingData, false);
        }


        [Test]
        public void PredictionComparison_CollectionCount_True()
        {
            Assert.AreEqual(_predictionComparisons.Count(), 6);
        }


        [Test]
        public void PredictionComparison_ObjectType_False()
        {
            var typeIsCorrectList =
                _predictionComparisons.Select(data => data.GetType() == typeof(PredictionComparison));
            var typeIsPredcitionComparison = typeIsCorrectList.Any(v => v == false);

            Assert.AreEqual(typeIsPredcitionComparison, false);
        }


        [Test]
        public void GetFixtures_CountDbFixtures_True()
        {
            var date = new DateTime(2016, 7, 1);
            var allFixtures = _fixtureData.GetFixturesAndResults.Count();

            var totalDbFixtures = _context.Fixtures
                .Where(x => x.FixtureDate > date)
                .GroupBy(x => x.FixtureReference)
                .Count();

            Assert.AreEqual(allFixtures, totalDbFixtures);
        }


        [TearDown]
        public void TearDown()
        {
            _matchBettingDataList = null;
            _predictionComparisons = null;
            _fakeWebscraper = null;
            _uriHCapCouponFake = null;
        }

    }
}
