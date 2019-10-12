using System;
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
            [NUnit.Framework.Values(0u,1u,2u,10u,100u)]uint numBombs)
        {
            // Use the Assert class to test conditions
            Generator.Generator generator = new Generator.Generator();
            Board board = generator.Generate(10, 4, 8, numBombs, disableSolving:true);
            
            // count bombs
            uint bombSum = 0;
            foreach (BoardCell cell in board.Cells)
            {
                if (cell.IsBomb) { bombSum++; }
            }

            if (numBombs <= 10 * 4 * 8)
            {
                Assert.AreEqual(numBombs, bombSum, "Wrong number of bombs in the board.");
            }
        }

        [Test]
        public void NumBombsTooBig()
        {
            Generator.Generator generator = new Generator.Generator();
            Assert.Throws<ArgumentException>(() => { generator.Generate(10, 4, 8, 1001u); });
        }
    }
}
