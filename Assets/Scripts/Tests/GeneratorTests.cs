using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Assets.Scripts.Tests
{
    class GeneratorTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void NumBombsCorrect(
            [NUnit.Framework.Values(0,1,2,10,100, 400, 1001, -2)]int numBombs)
        {
            // Use the Assert class to test conditions
            Generator.Generator generator = new Generator.Generator();
            Board board = generator.Generate(10, 4, 8, numBombs);
            
            // count bombs
            int bombSum = 0;
            foreach (BoardCell cell in board.Cells)
            {
                if (cell.IsBomb) { bombSum++; }
            }

            Assert.AreEqual(numBombs, bombSum, "Wrong number of bombs in the board.");
        }
    }
}
