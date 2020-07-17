using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Assets.Scripts.Data;
using Assets.Scripts.Solver.Rules;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    class SolverTests
    {
        [Test, Timeout(2000)]
        public void HerbertExampleSolvable()
        {
            /* 
             * Solution:     Given (with num bombs = 3)
             * x x 1 0       o o 1 o
             * x 3 1 0       o 3 1 o
             * 1 1 0 0       1 1 o o
             */
            Board testBoard = new Board(4, 3, 1);
            testBoard.SetBombState(0, 0, 0, true);
            testBoard.SetBombState(0, 1, 0, true);
            testBoard.SetBombState(1, 0, 0, true);
            testBoard[0, 2, 0].State = CellState.Revealed;
            testBoard[1, 1, 0].State = CellState.Revealed;
            testBoard[2, 0, 0].State = CellState.Revealed;
            testBoard[2, 1, 0].State = CellState.Revealed;
            Solver.Solver solver = new Solver.Solver(testBoard);

            Assert.AreEqual(true, solver.IsSolvable(),
                "HerbertExample should be solvable but that functionality is not implemented yet. See " +
                "ConsiderAllOptionsForTwoBombsAndFindThatOnlyOneOptionIsLegal for inspiration");
        }

        [Test]
        public void KevinExampleSolvable()
        {
            /* Solution:    Given:      Num bombs: 10
             * x 5 x        ? 5 ?
             * x x x        ? ? ?
             * x 8 x        ? 8 ?
             * x x x        ? ? ?
             */
            Board board = new Board(3, 4, 1);
            board.SetBombState(0, 0, 0, true);
            board.SetBombState(2, 0, 0, true);

            board.SetBombState(0, 1, 0, true);
            board.SetBombState(1, 1, 0, true);
            board.SetBombState(2, 1, 0, true);

            board.SetBombState(0, 2, 0, true);
            board.SetBombState(2, 2, 0, true);

            board.SetBombState(0, 3, 0, true);
            board.SetBombState(1, 3, 0, true);
            board.SetBombState(2, 3, 0, true);

            board[1, 0, 0].State = CellState.Revealed;
            board[1, 2, 0].State = CellState.Revealed;

            Assert.AreEqual(10, board.BombCount, "Wrong number of bombs!");

            Solver.Solver solver = new Solver.Solver(board);
            Assert.AreEqual(true, solver.IsSolvable(),
                "Should be solvable using a simple rule: all neighbors are bombs");
        }

        [Test]
        public void TinaTest()
        {
            /*
             * ? ? ? ?
             * ? 7 7 ?
             * ? ? ? ?
             */
            Board board = new Board(4, 3, 1);
            foreach (BoardCell cell in board.Cells)
            {
                board.SetBombState(cell.PosX, cell.PosY, cell.PosZ, true);
            }

            board.SetBombState(1, 1, 0, false);
            board.SetBombState(2, 1, 0, false);
            board[1, 1, 0].State = CellState.Revealed;
            board[2, 1, 0].State = CellState.Revealed;

            Solver.Solver solver = new Solver.Solver(board);
            Assert.True(solver.IsSolvable());
        }

        [Test]
        public void NoraTest()
        {
            // A cube with no bombs and one revealed
            Board board = new Board(4, 4, 4);
            board[3, 2, 2].State = CellState.Revealed;

            Solver.Solver solver = new Solver.Solver(board);
            Assert.True(solver.IsSolvable());
        }

        [Test]
        public void BoratTest()
        {
            // A cube with no bombs and one revealed
            Board board = new Board(4, 4, 4);
            board[3, 2, 2].State = CellState.Revealed;
            board.SetBombState(0, 0, 0, true);

            Solver.Solver solver = new Solver.Solver(board);
            Assert.True(solver.IsSolvable());
        }

        [Test]
        public void MinimalTest()
        {
            // A cube with no bombs and one revealed
            Board board = new Board(1, 1, 2);
            board[0, 0, 0].State = CellState.Revealed;
            board.SetBombState(0, 0, 1, true);

            Solver.Solver solver = new Solver.Solver(board);
            Assert.True(solver.IsSolvable());
        }

        [Test]
        public void JolandaTest()
        {
            Board board = new Board(2, 2, 2);
            board[0, 0, 0].State = CellState.Revealed;
            board.SetBombState(0, 0, 1, true);

            Solver.Solver solver = new Solver.Solver(board);
            Assert.False(solver.IsSolvable(), "This is not solvable without guesswork");
        }

        [Test, Timeout(2000)]
        public void TaelurExampleSolvable()
        {
            /*
             * Solution:     Given (with num bombs = 3)
             * 0 1 1       ? ? 1
             * 1 2 x       ? ? ?
             * 1 x 2       1 ? 2
             */
            Board testBoard = new Board(3, 3, 1);
            testBoard.SetBombState(1, 2, 0, true);
            testBoard.SetBombState(2, 1, 0, true);
            testBoard[0, 2, 0].State = CellState.Revealed;
            testBoard[2, 2, 0].State = CellState.Revealed;
            testBoard[2, 0, 0].State = CellState.Revealed;
            Solver.Solver solver = new Solver.Solver(testBoard);

            Assert.AreEqual(true, solver.IsSolvable(), "TaelurExample should be solvable");
        }

        [Test, Timeout(2000)]
        public void Gandalf2DExampleNotSolvable()
        {
            // This should not be solvable
            /*                    given:
             * x 2 x                ? 2 ?
             * 1 2 1                ? 2 ?
             * 0 0 0                0 0 0
             */
            Board testBoard = new Board(3, 3, 1);
            testBoard.SetBombState(0, 0, 0, true);
            testBoard.SetBombState(0, 2, 0, true);
            testBoard[0, 1, 0].State = CellState.Revealed;
            testBoard[1, 1, 0].State = CellState.Revealed;
            Solver.Solver solver = new Solver.Solver(testBoard);
            Assert.AreEqual(false, solver.IsSolvable(), "Gandalf shall not pass");
        }

        [Test, Timeout(2000)]
        public void Gandalf3DExampleNotSolvable()
        {
            // This should not be solvable
            /*                    given:
             * x 2 x                ? 2 ?
             * 1 2 1                ? 2 ?
             * 0 0 0                0 0 0
             */
            Board testBoard = new Board(3, 3, 3);
            testBoard.SetBombState(0, 0, 1, true);
            testBoard.SetBombState(0, 2, 1, true);
            testBoard[0, 1, 1].State = CellState.Revealed;
            testBoard[1, 1, 1].State = CellState.Revealed;
            Solver.Solver solver = new Solver.Solver(testBoard);
            Assert.AreEqual(false, solver.IsSolvable(), "Gandalf 3D shall not pass.");
        }

        [Test, Timeout(2000)]
        public void AbortReturnsFalseAndTrue()
        {
            // This should not be solvable (same Test as Gandalf)
            /*                    given:
             * x 2 x                ? 2 ?
             * 1 2 1                ? 2 ?
             * 0 0 0                0 0 0
             */
            Board testBoard = new Board(3, 3, 3);
            testBoard.SetBombState(0, 0, 1, true);
            testBoard.SetBombState(0, 2, 1, true);
            testBoard[0, 1, 1].State = CellState.Revealed;
            testBoard[1, 1, 1].State = CellState.Revealed;
            Solver.Solver solver = new Solver.Solver(testBoard);
            var Blubb = Task.Run(() =>
            {
                var res = solver.IsSolvable();
                Debug.Log("solving done!");
                return res;
            });
            solver.Abort();
            Debug.Log("aborted.");
            bool solvable = Blubb.Result;
            Assert.AreEqual(false, solvable, "Gandalf 3D the white shall not pass.");
            Assert.AreEqual(true, solver.HasAborted, "Gandalf 3D the grey shall not pass.");
        }

        [Test, Timeout(2000)]
        public void AllSolverRulesCanGiveHints()
        {
            Board testBoard = new Board(3, 3, 3);
            testBoard.SetBombState(0, 0, 1, true);
            testBoard.SetBombState(0, 2, 1, true);
            testBoard[0, 1, 1].State = CellState.Revealed;
            testBoard[1, 1, 1].State = CellState.Revealed;
            Solver.Solver solver = new Solver.Solver(testBoard);

            (var a, var b) = solver.GetRuleListsForTesting();
            List<Type> hintRuleTypes = new List<Type>();
            foreach (IHintRule hintRule in b)
            {
                var tb = hintRule.GetType();
                hintRuleTypes.Add(tb);
            }
            foreach (IRule rule in a)
            {
                var ta = rule.GetType();
                bool okay = hintRuleTypes.Contains(ta);
                Assert.True(okay, "Every Solver Rule should be represented in the hint rules. If this is not the case, think again about whether this should be required. Look at the lists in Solver.cs");
            }
        }

    }
}