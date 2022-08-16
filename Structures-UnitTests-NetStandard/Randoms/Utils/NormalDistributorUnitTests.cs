using System;
using System.Collections.Generic;
using NUnit.Framework;
using Structures.NetStandard.Randoms.Utils;


namespace Structures.UnitTests.NetStandard.Randoms.Utils {
    public class NormalDistributorUnitTests {
        private NormalDistributor _distributor;

        [TestCase(2000, 35, 980, 50, 100000, 10)]
        [Repeat(10)]
        public void CheckDistribution(double mean, double meanWindow, double stdev, double stdevWindow, int totalCount, float periods) {
            _distributor = new NormalDistributor(mean, stdev);

            IDictionary<int, double> buffer = new Dictionary<int, double>();
            
            for (int i = 0; i < totalCount; i++) {
                var rating = _distributor.Next();
                var key = (int)(Math.Round(rating / periods) * periods);
                if (buffer.TryGetValue(key, out var count)) {
                    count++;
                } else {
                    count = 1;
                }

                buffer[key] = count;
            }

            var newMean = 0d;
            foreach (var (value, count) in buffer) {
                newMean += value * count;
            }
            newMean /= totalCount;
            (double Min, double Max) meanBounds = (mean - meanWindow, mean + meanWindow);
            
            var newDispersion = 0d;
            foreach (var (value, count) in buffer) {
                newDispersion += Math.Pow(value - newMean, 2) * count;
            }
            newDispersion /= totalCount;
            var newStdev = Math.Sqrt(newDispersion);
            (double Min, double Max) stdevBounds = (stdev - stdevWindow, stdev + stdevWindow);
            
            Assert.GreaterOrEqual(newMean, meanBounds.Min);
            Assert.LessOrEqual(newMean, meanBounds.Max);
            
            Assert.GreaterOrEqual(newStdev, stdevBounds.Min);
            Assert.LessOrEqual(newStdev, stdevBounds.Max);
            
            Console.WriteLine($"Mean: '{newMean}'; Stdev: '{newStdev}'");
        }
    }
}