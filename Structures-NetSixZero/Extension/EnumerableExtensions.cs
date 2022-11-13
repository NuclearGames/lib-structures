namespace Structures.NetSixZero.Extension {
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
    }
}