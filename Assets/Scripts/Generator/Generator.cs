using System;
using System.Linq;
using Assets.Scripts.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Generator
{
    public class Generator
    { 
        public Generator ()
        {
        }

        /// <summary>
        /// Generates a board with an arbitrary initial guess. Tries to solve that board, and if that works, returns it.
        /// </summary>
        /// <param name="boardWidth"></param>
        /// <param name="boardHeight"></param>
        /// <param name="boardDepth"></param>
        /// <param name="numBombs"></param>
        /// <returns></returns>
        public Board Generate (uint boardWidth, uint boardHeight, uint boardDepth, uint numBombs, bool disableSolving = false)
        {
            if (boardWidth * boardHeight * boardDepth < numBombs)
            {
                throw new ArgumentException("Number of bombs is larger than the Board");
            }

            var board = new Board((int)boardWidth, (int)boardHeight, (int)boardDepth);

            for (uint bombCount = 0; bombCount < numBombs; bombCount++)
            {
                PlaceBombRandomlyOnBoard(board);
            }
            SeedFirstNude(board);

            // return early for tests that are afraid of endless retries (i.e. tests that don't care about the solver)
            if (disableSolving)
            {
                return board;
            }

            var solver = new Solver.Solver(board);
            var tries = 0;

            while (!solver.IsSolvable())
            {
                tries++;

                if (tries > 20)
                {
                    throw new EricException();
                }

                Debug.Log("Trying again, round " + tries);

                board.ResetBoard();

                for (var i = 0u; i < numBombs; i++)
                {
                    PlaceBombRandomlyOnBoard(board);
                }
                SeedFirstNude(board);
            }

            board.ResetCellStates();
            return board;
        }

        private void SeedFirstNude(Board board)
        {
            bool foundNude = false;
            foreach (BoardCell cell in board.Cells)
            {
                if (cell.IsNude)
                {
                    cell.State = CellState.Revealed;
                    foundNude = true;
                    break;
                }
            }

            if (!foundNude)
            {
                // no nudes available. use any non-bomb
                foreach (BoardCell cell in board.Cells)
                {
                    if (!cell.IsBomb)
                    {
                        cell.State = CellState.Revealed;
                        return;
                    }
                }

                // only bombs available!
                throw new EricException();
            }
        }

        private void PlaceBombRandomlyOnBoard(Board board)
        {
            int posx = Random.Range(0, board.Width);
            int posy = Random.Range(0, board.Height);
            int posz = Random.Range(0, board.Depth);
            BoardCell cell = board[posx, posy, posz];

            if (board[posx, posy, posz].IsBomb)
            {
                // try again with different random values
                PlaceBombRandomlyOnBoard(board);
            }
            else
            {
                board.SetBombState(posx, posy, posz, true);
            }
        }
    }
}
