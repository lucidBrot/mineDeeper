using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data;
using NUnit.Framework;

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
            Board testBoard = new Board(4,3,1);
            testBoard.SetBombState(0, 0, 0, true);
            testBoard.SetBombState(0, 1, 0, true);
            testBoard.SetBombState(1,0,0,true);
            testBoard[0, 2, 0].State = CellState.Revealed;
            testBoard[1, 1, 0].State = CellState.Revealed;
            testBoard[2, 0, 0].State = CellState.Revealed;
            testBoard[2, 1, 0].State = CellState.Revealed;
            Solver.Solver solver = new Solver.Solver(testBoard);

            Assert.AreEqual(true, solver.IsSolvable(), "HerbertExample should be solvable");
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
            board.SetBombState(0,0,0, true);
            board.SetBombState(2,0,0,true);

            board.SetBombState(0,1,0, true);
            board.SetBombState(1,1,0,true);
            board.SetBombState(2,1,0,true);

            board.SetBombState(0,2,0,true);
            board.SetBombState(2,2,0,true);

            board.SetBombState(0,3,0, true);
            board.SetBombState(1, 3, 0, true);
            board.SetBombState(2, 3, 0, true);

            board[1, 0, 0].State = CellState.Revealed;
            board[1, 2, 0].State = CellState.Revealed;

            Assert.AreEqual(10, board.BombCount, "Wrong number of bombs!");

            Solver.Solver solver = new Solver.Solver(board);
            Assert.AreEqual(true, solver.IsSolvable(), "Should be solvable using a simple rule: all neighbors are bombs");
        }

        [Test]
        public void TinaTest()
        {
            /*
             * ? ? ? ?
             * ? 7 7 ?
             * ? ? ? ?
             */
            Board board = new Board(4,3,1);
            foreach (BoardCell cell in board.Cells)
            {
                board.SetBombState(cell.PosX, cell.PosY, cell.PosZ, true);
            }
            board.SetBombState(1, 1, 0, false);
            board.SetBombState(2,1,0,false);
            board[1, 1, 0].State = CellState.Revealed;
            board[2, 1, 0].State = CellState.Revealed;

            Solver.Solver solver = new Solver.Solver(board);
            Assert.True(solver.IsSolvable());
        }

        [Test]
        public void NoraTest()
        {
            // A cube with no bombs and one revealed
            Board board = new Board(4,4,4);
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
            board.SetBombState(0,0,0,true);

            Solver.Solver solver = new Solver.Solver(board);
            Assert.True(solver.IsSolvable());
        }

        [Test]
        public void MinimalTest()
        {
            // A cube with no bombs and one revealed
            Board board = new Board(1,1,2);
            board[0,0,0].State = CellState.Revealed;
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
    }
}