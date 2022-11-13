using Structures.NetSixZero.Utils.Generators;

namespace Structures_UnitTests_NetSixZero.Utils.Generators {
    public class UlongIdGeneratorUnitTests : BaseIdGeneratorsUnitTests<ULongIdGenerator, ulong> {
        private protected override ULongIdGenerator Constructor(int size) {
            return new ULongIdGenerator((ulong)size);
        }
    }
}