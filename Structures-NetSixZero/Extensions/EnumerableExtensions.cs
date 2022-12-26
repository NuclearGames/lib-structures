namespace Structures.NetSixZero.Extensions {
    public static class EnumerableExtensions {
        public static IEnumerable<long> Range(long start, long count) {
            var maxBound = long.MaxValue / 2 > count ? (start + count) : long.MaxValue; 
            
            for (var i = start; i < maxBound; i++) {
                yield return i;
            }
        }
        
        public static IEnumerable<ulong> Range(ulong start, ulong count) {
            var maxBound = ulong.MaxValue / 2 > count ? (start + count) : ulong.MaxValue; 
            
            for (var i = start; i < maxBound; i++) {
                yield return i;
            }
        }
        
        /// <summary>
        /// Перемешивает массив/лист.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list) {
            int n = list.Count;
            var rnd = new Random();
            while (n > 1) {
                n--;
                var k = rnd.Next(0, n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}