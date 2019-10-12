using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Assets.Scripts.Solver
{
    class Solver
    {
        private readonly Board board;
        /// <summary>
        /// A writeable board that has flags for found bombs as <c>IsSuspect</c> and updates <see cref="BoardCell.AdjacentBombCount"/>
        /// </summary>
        private readonly Board noteBoard;
        private const int NUM_NEIGHBORS = 3*3*3-1;
        private bool? solvable;

        private int numUnfoundBombs;

        public Solver(Board board)
        {
            this.board = board;
            this.noteBoard = new Board(board.Width, board.Height, board.Depth);
            this.numUnfoundBombs = board.BombCount;
        }

        /// <summary>
        /// Expects a board that already offers information to the player.
        /// </summary>
        /// <returns>True if the board is solvable without guessing, False otherwise</returns>
        public bool IsSolvable()
        {
            if (solvable == null)
            {
                Compute();
            }

            //TODO: return solvable.Value;
            return true;
        }

        private void Compute()
        {
            foreach (BoardCell cell in this.board.Cells)
            {
                ConsiderAllNeighborsAreBombs(cell);
                // TODO: consider no neighbors are bombs
            }
        }

        private void ConsiderAllNeighborsAreBombs(BoardCell cell)
        {
            // if the number of adjacent uncertainties equals the number on the cell, every uncertainty is a bomb
            int revealedSafeCells = 0;
            List<BoardCell> possibleBombs = new List<BoardCell>();
            foreach (BoardCell neighbor in board.GetAdjacentCells(cell.PosX, cell.PosY, cell.PosZ))
            {
                if (neighbor.IsRevealed)
                {
                    revealedSafeCells++;
                }

                if (!neighbor.IsRevealed)
                {
                    possibleBombs.Add(neighbor);
                    numUnfoundBombs--;
                }
            }

            if (revealedSafeCells + cell.AdjacentBombCount == NUM_NEIGHBORS)
            {
                // they are all bombs. Take note on the noteBoard
                foreach (BoardCell bomb in possibleBombs)
                {
                    BoardCell noteBomb = noteBoard.getAt(bomb);
                    noteBomb.IsSuspect = true; 
                }
                // cell has no more unfound bombs around it
                noteBoard.BombCount = 0;
            }
        }
    }
}
