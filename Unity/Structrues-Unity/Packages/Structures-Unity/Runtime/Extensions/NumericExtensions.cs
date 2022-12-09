using System;

namespace NuclearGames.StructuresUnity.Extensions {
    public static class NumericExtensions {
        /// <summary>
        /// Залупливает число в промежутке от 0 (включительно) до count (невключительно).
        /// </summary>
        public static int Loop(this int value, int count) {
            return value < 0
                ? Math.Abs(count + value) % count
                : value % count;
        }
    }
}
