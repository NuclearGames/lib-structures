using System;
using System.Collections.Generic;
using System.Linq;
using NuclearGames.StructuresUnity.Randoms;
using NuclearGames.StructuresUnity.Randoms.Interfaces;
using NUnit.Framework;

namespace Tests.Editor.Randoms {
    public class FixedIntegerRandomUnitTests {
        private IRandomizer _randomizer;

        [SetUp]
        public void SetUp() {
            var map = new Dictionary<int, int> {
                { 0, 1 },
                { 1, 2 },
                { 2, 3 },
                { 3, 4 },
                { 4, 5 },
                { 5, 6 }
            };
            _randomizer = new FixedIntegerRandom(map);
        }

        [Repeat(100)]
        [TestCase(3.33, 0.02, 100000)]
        public void CheckIntDistribution(double mean, double meanWindow, int totalCount) {
            var map = new Dictionary<int, int>();

            var sum = 0d;
            for (int i = 0; i < totalCount; i++) {
                var rating = _randomizer.Next();
                sum += rating;

                if (!map.TryGetValue(rating, out var count)) {
                    count = 0;
                }

                map[rating] = count + 1;
            }

            CheckMean(sum, mean, meanWindow, totalCount);
            CheckMap(map);
        }
        
        [Repeat(100)]
        [TestCase(3.33, 0.02, 100000)]
        public void CheckFloatDistribution(double mean, double meanWindow, int totalCount) {
            var map = new Dictionary<int, int>();

            var sum = 0d;
            for (int i = 0; i < totalCount; i++) {
                var rating = _randomizer.NextSingle();
                var key = (int)rating;
                sum += rating;

                if (!map.TryGetValue(key, out var count)) {
                    count = 0;
                }

                map[key] = count + 1;
            }

            CheckMean(sum, mean, meanWindow, totalCount);
            CheckMap(map);
        }

        [Repeat(100)]
        [TestCase( 3.53, 0.02, 100000, 2, 4)]
        public void CheckIntClampDistribution(double mean, double meanWindow, int totalCount, int minClamp, int maxClamp) {
            var map = new Dictionary<int, int>();

            var sum = 0d;
            for (int i = 0; i < totalCount; i++) {
                var rating = _randomizer.Next(minClamp, maxClamp, false);
                sum += rating;

                if (!map.TryGetValue(rating, out var count)) {
                    count = 0;
                }

                map[rating] = count + 1;
            }

            CheckMean(sum, mean, meanWindow, totalCount);
            // CheckMap(map);
        }
        
        [Repeat(100)]
        [TestCase( 3.23, 0.02, 100000, 2, 4)]
        public void CheckFloatClampDistribution(double mean, double meanWindow, int totalCount, int minClamp, int maxClamp) {
            var map = new Dictionary<int, int>();

            var sum = 0d;
            for (int i = 0; i < totalCount; i++) {
                var rating = _randomizer.NextSingle(minClamp, maxClamp, false);
                var key = (int)rating;
                sum += rating;

                if (!map.TryGetValue(key, out var count)) {
                    count = 0;
                }

                map[key] = count + 1;
            }

            CheckMean(sum, mean, meanWindow, totalCount);
            // CheckMap(map);
        }

#region Utils

        private static void CheckMean(double sum, double mean, double meanWindow, int totalCount) {
            var newMean = sum / totalCount;
            (double Min, double Max) meanBounds = (mean - meanWindow, mean + meanWindow);

            Assert.GreaterOrEqual(newMean, meanBounds.Min);
            Assert.LessOrEqual(newMean, meanBounds.Max);
            
            Console.WriteLine($"Mean: '{newMean}';");
        }

        private static void CheckMap(IDictionary<int, int> map) {
            var prevCount = int.MinValue;
            foreach (var key in map.Keys.OrderBy(v => v)) {
                var newCount = map[key];
                Assert.GreaterOrEqual(newCount, prevCount);
                prevCount = newCount;
            }
        }

#endregion
    }
}