using System;
using System.Collections.Generic;
using NuclearGames.StructuresUnity.Randoms;
using NuclearGames.StructuresUnity.Randoms.Interfaces;
using NUnit.Framework;

namespace Tests.Editor.Randoms {
    public class UniformRandomUnitTests {
        private IRandomizer _randomizer;
        
        [Repeat(100)]
        [TestCase(int.MaxValue / 2, 7000000, 100000)]
        public void CheckIntDistribution(double mean, double meanWindow, int totalCount) {
            _randomizer = new UniformRandom();

            var sum = 0d;
            
            for (int i = 0; i < totalCount; i++) {
                var rating = _randomizer.Next();
                sum += rating;
            }
            
            CheckMeanMap(sum, mean, meanWindow, totalCount);
        }
        
        [Repeat(100)]
        [TestCase(2000, 20, 1155, 10, 100000, 0,4000)]
        [TestCase(2000, 10, 1155 / 2 , 5, 100000, 1000,3000)]
        [TestCase(2000, 30, 1155 * 1.5 , 15, 100000, -1000,5000)]
        public void CheckIntClampDistribution(double mean, double meanWindow, double stdev, double stdevWindow, int totalCount, int minClamp, int maxClapm) {
            _randomizer = new UniformRandom();

            IDictionary<int, double> buffer = new Dictionary<int, double>();
            
            for (int i = 0; i < totalCount; i++) {
                var rating = _randomizer.Next(minClamp, maxClapm);
                var key = rating;
                if (buffer.TryGetValue(key, out var count)) {
                    count++;
                } else {
                    count = 1;
                }

                buffer[key] = count;
            }

            FullCheckMap(buffer, mean, meanWindow, stdev, stdevWindow, totalCount);
        }
        
        [Repeat(100)]
        [TestCase(0.5f, 0.01f, 100000, 1000)]
        public void CheckFloatDistribution(double mean, double meanWindow, int totalCount, float step) {
            _randomizer = new UniformRandom();

            var sum = 0d;
            
            for (int i = 0; i < totalCount; i++) {
                var rating = _randomizer.NextSingle();
                sum += rating;
            }
            
            CheckMeanMap(sum, mean, meanWindow, totalCount);
        }
        
        [Repeat(100)]
        [TestCase(2000, 20, 1155, 10, 100000, 10, 0,4000)]
        [TestCase(2000, 10, 1155 / 2 , 5, 100000, 10, 1000,3000)]
        [TestCase(2000, 30, 1155 * 1.5 , 15, 100000, 10, -1000,5000)]
        public void CheckFloatClampDistribution(double mean, double meanWindow, double stdev, double stdevWindow, int totalCount, float step, float minClamp, float maxClapm) {
            _randomizer = new UniformRandom();

            IDictionary<int, double> buffer = new Dictionary<int, double>();
            
            for (int i = 0; i < totalCount; i++) {
                var rating = _randomizer.NextSingle(minClamp, maxClapm);
                var key = (int)(Math.Round(rating / step) * step);
                if (buffer.TryGetValue(key, out var count)) {
                    count++;
                } else {
                    count = 1;
                }

                buffer[key] = count;
            }

            FullCheckMap(buffer, mean, meanWindow, stdev, stdevWindow, totalCount);
        }
        
        private static void CheckMeanMap(double sum, double mean, double meanWindow, int totalCount) {
            var newMean = sum / totalCount;
            (double Min, double Max) meanBounds = (mean - meanWindow, mean + meanWindow);

            Assert.GreaterOrEqual(newMean, meanBounds.Min);
            Assert.LessOrEqual(newMean, meanBounds.Max);
            
            Console.WriteLine($"Mean: '{newMean}';");
        }
        
        private static void FullCheckMap(IDictionary<int, double> buffer, double mean, double meanWindow, double stdev, double stdevWindow, int totalCount) {
            var newMean = 0d;
            foreach (var kvp  in buffer) {
                var value = kvp.Key; 
                var count = kvp.Value;
                newMean += value * count;
            }
            newMean /= totalCount;
            (double Min, double Max) meanBounds = (mean - meanWindow, mean + meanWindow);
            
            var newDispersion = 0d;
            foreach (var kvp  in buffer) {
                var value = kvp.Key; 
                var count = kvp.Value;
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