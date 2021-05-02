using NUnit.Framework;
using Bellman_ford;
using System;
using System.Collections.Generic;
using System.IO;

namespace BellmanFordTests
{
    public class Tests
    {

        [Test]
        public void GetPath_NoExistPath_Exception()
        {
            BellmanFord bellmanFord = new BellmanFord("TestPath.txt");

            Assert.Throws<Exception>(() => bellmanFord.GetPath("Khabarovsk", "St. Petersburg"));
        }

        [Test]
        public void GetPath_BadFileData_Exception()
        {
            BellmanFord bellmanFord = new BellmanFord("IncorrectInput.txt");

            Assert.Throws<Exception>(() => bellmanFord.GetPath("Khabarovsk", "St. Petersburg"));
        }

        [Test]
        public void GetPath_BadInputData_Exception()
        {
            BellmanFord bellmanFord = new BellmanFord("TestPath.txt");

            Assert.Throws<Exception>(() => bellmanFord.GetPath("Abooba", "Khabarovsk"));
        }


        [Test]
        public void GetPath_ExistPath_Price()
        {
            BellmanFord bellmanFord = new BellmanFord("TestPath.txt");

            bellmanFord.GetPath("St. Petersburg", "Moscow");

            int price = 10;

            Assert.AreEqual(price, bellmanFord.totalPricePath);
        }


        [Test]
        public void GetPath_ExistPath_Path()
        {
            var bellmanFord = new BellmanFord("TestPaths.txt");

            bellmanFord.GetPath("Vladivostok", "Moscow");

            var coasts = new List<int>
            {
                13,
                35
            };

            var citiesName = new List<string>
            {
                "Vladivostok",
                "Moscow"
            };

            var expected = (citiesName, coasts);

            var path = (bellmanFord.pathByCities, bellmanFord.pathByPrice);

            Assert.IsTrue(AreEqual(path, expected));
        }


        private bool AreEqual((List<string>, List<int>) tuple1, (List<string>, List<int>) tuple2)
        {
            var citiesName1 = tuple1.Item1;

            var coasts1 = tuple1.Item2;

            var citiesName2 = tuple2.Item1;

            var coasts2 = tuple2.Item2;

            if (citiesName1.Count != citiesName2.Count)
            {
                return false;
            }

            if (coasts1.Count != coasts2.Count)
            {
                return false;
            }

            for (var i = 0; i < coasts1.Count; i++)
            {
                if (coasts1[i] != coasts2[i] || citiesName1[i] != citiesName2[i])
                {
                    return false;
                }
            }

            return true;
        }

    }
}