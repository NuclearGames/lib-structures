using System;
using System.Collections.Generic;
using LogicUtils.Randoms.Interfaces;
using NUnit.Framework;
using Structures.NetStandard.Randoms;


namespace Structures.UnitTests.NetStandard.Randoms {
    public class NormalRandomUnitTests {

        private IRandomizer _random;
        
        [Repeat(10)]
        [TestCase(2000, 35, 980, 50, 100000)]
        public void CheckIntDistribution(double mean, double meanWindow, double stdev, double stdevWindow, int totalCount) {
            _random = new NormalRandom(mean, stdev);

            IDictionary<int, double> buffer = new Dictionary<int, double>();
            
            for (int i = 0; i < totalCount; i++) {
                var rating = _random.Next();
                var key = rating;
                if (buffer.TryGetValue(key, out var count)) {
                    count++;
                } else {
                    count = 1;
                }

                buffer[key] = count;
            }

            CheckMap(buffer, mean, meanWindow, stdev, stdevWindow, totalCount);
        }

        [Repeat(10)]
        [TestCase(2000, 35, 980, 50, 100000, 0,5000)]
        public void CheckIntClampDistribution(double mean, double meanWindow, double stdev, double stdevWindow, int totalCount, int minClamp, int maxClapm) {
            _random = new NormalRandom(mean, stdev);

            IDictionary<int, double> buffer = new Dictionary<int, double>();
            
            for (int i = 0; i < totalCount; i++) {
                var rating = _random.Next(minClamp, maxClapm);
                var key = rating;
                if (buffer.TryGetValue(key, out var count)) {
                    count++;
                } else {
                    count = 1;
                }

                buffer[key] = count;
            }

            CheckMap(buffer, mean, meanWindow, stdev, stdevWindow, totalCount);
        }
        
        [Repeat(10)]
        [TestCase(2000, 35, 980, 50, 100000, 10)]
        public void CheckFloatDistribution(double mean, double meanWindow, double stdev, double stdevWindow, int totalCount, float step) {
            _random = new NormalRandom(mean, stdev);

            IDictionary<int, double> buffer = new Dictionary<int, double>();
            
            for (int i = 0; i < totalCount; i++) {
                var rating = _random.NextSingle();
                var key = (int)(Math.Round(rating / step) * step);
                if (buffer.TryGetValue(key, out var count)) {
                    count++;
                } else {
                    count = 1;
                }

                buffer[key] = count;
            }

            CheckMap(buffer, mean, meanWindow, stdev, stdevWindow, totalCount);
        }
        
        [Repeat(10)]
        [TestCase(2000, 35, 980, 50, 100000, 10, 0,5000)]
        public void CheckFloatClampDistribution(double mean, double meanWindow, double stdev, double stdevWindow, int totalCount, float step, int minClamp, int maxClapm) {
            _random = new NormalRandom(mean, stdev);

            IDictionary<int, double> buffer = new Dictionary<int, double>();
            
            for (int i = 0; i < totalCount; i++) {
                var rating = _random.NextSingle(minClamp, maxClapm);
                var key = (int)(Math.Round(rating / step) * step);
                if (buffer.TryGetValue(key, out var count)) {
                    count++;
                } else {
                    count = 1;
                }

                buffer[key] = count;
            }

            CheckMap(buffer, mean, meanWindow, stdev, stdevWindow, totalCount);
        }

        private static void CheckMap(IDictionary<int, double> buffer, double mean, double meanWindow, double stdev, double stdevWindow, int totalCount) {
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