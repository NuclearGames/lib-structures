﻿namespace Structures.NetSixZero.Extensions {
    public static class FlagsExtensions {
        /// <summary>
        /// Возвращает перечисление флагов маски.
        /// Нулевое значение не возвращает.
        /// </summary>
        public static IEnumerable<T> GetFlags<T>(this T mask) where T : Enum {
            var defaultT = default(T);
            foreach (Enum value in Enum.GetValues(mask.GetType())) {
                if (mask.HasFlag(value) && !Equals(value, defaultT)) {
                    yield return (T)value;
                }
            }
        }
    }
}
