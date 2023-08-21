using System.Runtime.CompilerServices;

namespace Structures.NetSixZero.Utils.Generators {
    public static class FastGuidGenerator {
        [SkipLocalsInit]
        public static unsafe Guid Generate() {
            var bytes = stackalloc byte[16];
            Random.Shared.NextBytes(new Span<byte>(bytes, 16));
            return *(Guid*)bytes;
        }
    }
}
