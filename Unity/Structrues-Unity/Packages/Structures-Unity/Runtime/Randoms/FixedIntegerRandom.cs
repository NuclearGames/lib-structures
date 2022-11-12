using System;
using System.Collections.Generic;
using System.Linq;
using NuclearGames.StructuresUnity.Randoms.Interfaces;
using UnityEngine;
using Random = System.Random;

namespace NuclearGames.StructuresUnity.Randoms {
    /// <summary>
    /// Фиксированный раномайзер (по конкретным значениям в конкретных пропорциях)
    /// </summary>
    public class FixedIntegerRandom : IRandomizer {
        private const float APROXIMATE_DELTA = 0.000001f; 
        private readonly struct DistributionPoint {
            public float Probability { get; }
            public int Value { get; }

            public DistributionPoint(float probability, int value) {
                Probability = probability;
                Value = value;
            }
        }

        private readonly Random _randomizer = new Random();
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
            foreach (var kvp in distributionMap) {
                var value = kvp.Key;
                var count = kvp.Value;
                probabilityMap.Add(value, count / totalCount);
            }

            foreach (var kvp in probabilityMap.OrderBy(kvp => kvp.Key)) {
                var value = kvp.Key;
                var probability = kvp.Value;
                totalProbability += probability;
                
                _distributionList.Add(new DistributionPoint(totalProbability, value));
            }
        }
        
        /// <summary>
        /// Следующее целочисленное значение
        /// </summary>
        public int Next() {
            var probability = _randomizer.NextDouble();
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
            return Mathf.Clamp(Next(), minValue, (excludeLast ? maxValue : maxValue + 1));
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
            return Mathf.Clamp(NextSingle(), minValue, (excludeLast ? maxValue : maxValue + APROXIMATE_DELTA));
        }
    }
}