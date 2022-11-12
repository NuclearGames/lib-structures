using System;
using NuclearGames.StructuresUnity.Extension;
using NuclearGames.StructuresUnity.Randoms.Interfaces;

namespace NuclearGames.StructuresUnity.Randoms {
    /// <summary>
    /// Равномерный рандомайзер
    /// </summary>
    public class UniformRandom: IRandomizer {
        private const float APROXIMATE_DELTA = 0.000001f; 
        
        private readonly Random _distributor = new Random();
        
        /// <summary>
        /// Следующее целочисленное значение
        /// </summary>
        public int Next() {
            return _distributor.Next();
        }
        
        /// <summary>
        /// Следующее целочисленное значение
        /// </summary>
        public int Next(int minValue, int maxValue, bool excludeLast = true) {
            maxValue = excludeLast ? maxValue : maxValue + 1;
            return _distributor.Next(minValue, maxValue);
        }
        
        /// <summary>
        /// Следующее вещественное значение
        /// </summary>
        public float NextSingle() {
            return _distributor.NextSingle();
        }

        /// <summary>
        /// Следующее вещественное значение
        /// </summary>
        public float NextSingle(float minValue, float maxValue, bool excludeLast = false) {
            maxValue = excludeLast ? maxValue : maxValue + APROXIMATE_DELTA;
            var delta = maxValue - minValue;

            return _distributor.NextSingle() * delta + minValue;
        }
    }
}