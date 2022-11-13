using Structures.NetSixZero.Utils.Generators;

namespace Structures_UnitTests_NetSixZero.Utils.Generators {
    public class LongIdGeneratorUnitTests : BaseIdGeneratorsUnitTests<LongIdGenerator, long> {
        private protected override LongIdGenerator Constructor(int size) {
            return new LongIdGenerator(size);
        }
    }
}