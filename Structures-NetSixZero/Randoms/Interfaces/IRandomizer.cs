namespace Structures.NetSixZero.Randoms.Interfaces {
    public interface IRandomizer {
        /// <summary>
        /// Получить следующее целочисленное значение
        /// </summary>
        int Next();
        
        /// <summary>
        /// Получить следующее вещественое значение
        /// </summary>
        float NextSingle();
        
        /// <summary>
        /// Получить следующее целочисленное значение
        /// </summary>
        int Next(int minValue, int maxValue, bool excludeLast = true);
        
        /// <summary>
        /// Получить следующее вещественое значение
        /// </summary>
        float NextSingle(float minValue, float maxValue, bool excludeLast = false);
    }
}