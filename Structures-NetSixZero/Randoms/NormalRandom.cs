using System;
using Structures.NetSixZero.Randoms.Interfaces;
using Structures.NetSixZero.Randoms.Utils;


namespace Structures.NetSixZero.Randoms {
    /// <summary>
    /// Рандомайзер по Гауссу
    /// </summary>
    public class NormalRandom : IRandomizer {
        private const float APROXIMATE_DELTA = 0.000001f; 

        private readonly NormalDistributor _distributor;
        
        public NormalRandom(double mean, double stdev) {
            _distributor = new NormalDistributor(mean, stdev);
        }
        
        /// <summary>
        /// Получить следующее целочисленное значение
        /// </summary>
        public int Next() {
            return (int)Math.Round(_distributor.Next());
        }

        /// <summary>
        /// Получить следующее целочисленное значение
        /// </summary>
        public int Next(int minValue, int maxValue, bool excludeLast = true) {
            maxValue = excludeLast ? maxValue : maxValue + 1;
            
            return Math.Clamp(Next(), minValue, maxValue);
        }
        
        /// <summary>
        /// Получить следующее вещественое значение
        /// </summary>
        public float NextSingle() {
            return NextSingle(0f, 1f);
        }

        /// <summary>
        /// Получить следующее вещественое значение
        /// </summary>
        public float NextSingle(float minValue, float maxValue, bool excludeLast = false) {
            maxValue = excludeLast ? maxValue : maxValue + APROXIMATE_DELTA;

            float value = (float)_distributor.Next();
            
            return Math.Clamp(value, minValue, maxValue);
        }
    }
}