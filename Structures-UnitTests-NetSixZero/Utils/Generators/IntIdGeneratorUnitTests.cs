using Structures.NetSixZero.Utils.Generators;

namespace Structures_UnitTests_NetSixZero.Utils.Generators {
    public class IntIdGeneratorUnitTests : BaseIdGeneratorsUnitTests<IntIdGenerator, int> {
        private protected override IntIdGenerator Constructor(int size) {
            return new IntIdGenerator(size);
        }
    }
}