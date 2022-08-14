using System;
using System.Collections.Generic;
using System.Linq;
using Structures.NetSixZero.Randoms.Interfaces;


namespace Structures.NetSixZero.Randoms {
    /// <summary>
    /// Фиксированный раномайзер (по конкретным значениям в конкретных пропорциях)
    /// </summary>
    public class FixedIntegerRandom : IRandomizer {
        private const float APROXIMATE_DELTA = 0.000001f; 
        private readonly struct DistributionPoint {
            public float Probability { get; init; }
            public int Value { get; init; }
        }

        private readonly Random _randomizer = new();
        private readonly List<DistributionPoint> _distributionList;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="distributionMap">Значения - Частота появления значения </param>
        public FixedIntegerRandom(IDictionary<int, int> distributionMap) {
            _distributionList = new List<DistributionPoint>(distributionMap.Count);
            
            var totalProbability = 0f;

            var probabilityMap = new Dictionary<int, float>(distributionMap.Count);
            float totalCount = distributionMap.Values.Sum();
            foreach (var (value, count) in distributionMap) {
                probabilityMap.Add(value, count / totalCount);
            }

            foreach (var (value, probability) in probabilityMap.OrderBy(kvp => kvp.Key)) {
                totalProbability += probability;
                
                _distributionList.Add(new DistributionPoint {
                    Value = value, Probability = totalProbability
                });
            }
        }
        
        /// <summary>
        /// Следующее целочисленное значение
        /// </summary>
        public int Next() {
            var probability = _randomizer.NextSingle();
            int? value = null; 
            for (int i = 0; i < _distributionList.Count; i++) {
                if (_distributionList[i].Probability >= probability) {
                    value = _distributionList[i].Value;
                    break;
                }
            }

            if (value == null) {
                throw new ArgumentNullException($"Invalid distributionList: probaility='{probability}' not found!");
            }

            return value.Value;
        }
        
        /// <summary>
        /// Следующее целочисленное значение
        /// </summary>
        public int Next(int minValue, int maxValue, bool excludeLast = true) {
            return Math.Clamp(Next(), minValue, (excludeLast ? maxValue : maxValue + 1));
        }
        
        /// <summary>
        /// Следующее вещественное значение
        /// </summary>
        public float NextSingle() {
            return Next();
        }

        /// <summary>
        /// Следующее вещественное значение
        /// </summary>
        public float NextSingle(float minValue, float maxValue, bool excludeLast = false) {
            return Math.Clamp(NextSingle(), minValue, (excludeLast ? maxValue : maxValue + APROXIMATE_DELTA));
        }
    }
}