using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Assets.Scripts.Tests
{
    class SolverTests
    {
        [Test]
        public void HerbertExampleSolvable()
        {
            /*
             * Solution:     Given (with num bombs = 3)
             * x x 1 0       o o 1 o
             * x 3 1 0       o 3 o o
             * 1 1 0 0       1 1 o o
             */
            Board testBoard = new Board(4,3,1);
            testBoard.SetBombState(0, 0, 0, true);
            testBoard.SetBombState(0, 1, 0, true);
            testBoard.SetBombState(1,0,0,true);
            testBoard.get(0, 2, 0).IsRevealed = true;
            testBoard.get(1, 1, 0).IsRevealed = true;
            testBoard.get(2, 0, 0).IsRevealed = true;
            testBoard.get(2, 1, 0).IsRevealed = true;
            Solver.Solver solver = new Solver.Solver(testBoard);

            Assert.AreEqual(true, solver.IsSolvable(), "HerbertExample should be solvable");
        }

        [Test]
        public void KevinExampleSolvable()
        {

        }
    }
}