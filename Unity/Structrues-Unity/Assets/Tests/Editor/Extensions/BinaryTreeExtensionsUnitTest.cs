using NuclearGames.StructuresUnity.Extensions;
using NuclearGames.StructuresUnity.Randoms;
using NuclearGames.StructuresUnity.Structures.BST;
using NUnit.Framework;

namespace Tests.Editor.Extensions {
    internal class BinaryTreeExtensionsUnitTest {
        [Test]
        public void RandomTest() {
            var tree = new BinaryTree<int>();
            var random = new UniformRandom();

            for (int i = 0; i < 1000; i++) {
                int sign = random.Next(0, 1, false) == 1 ? -1 : 1;
                tree.Add(i * sign);
            }

            for (int i = 0; i < 100; i++) {
                var randomElement = tree.GetRandom(random);
                Assert.IsTrue(tree.Contains(randomElement));
            }
        }
    }
}
