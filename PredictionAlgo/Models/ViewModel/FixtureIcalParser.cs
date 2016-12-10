//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;

//namespace PredictionAlgo.Models.ViewModel
//{
//    public class FixtureIcalParser
//    {
//        private const string LocalPath = @"C:\Users\Temp\Fixtures.txt";
//        private const string Pro12FixturesPath = @"http://www.pro12rugby.com/tools/calendars/celtic-fixtures.ics";
//        private readonly string _uriPro12Fixtures = new Uri(Pro12FixturesPath).AbsoluteUri;

//        public string[] GetFixturesIcalToFile()
//        {
//            using (var client = new WebClient())
//            {
//                client.DownloadFile(_uriPro12Fixtures, LocalPath);
//            }
//            return File.ReadAllLines(LocalPath);
//        }

//        public IEnumerable<string> GetFixtureData(string[] icalData)
//        {
//            var fixturesRawData = new List<string>(100);
//            fixturesRawData.AddRange(icalData.Where(line => line.StartsWith("UID")));
//            return fixturesRawData;
//        }
//    }
//}