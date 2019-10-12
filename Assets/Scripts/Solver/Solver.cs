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
    public class Solver
    {
        private readonly Board board;
        /// <summary>
        /// A writeable board that has flags for found bombs as <c>IsSuspect</c> and updates <see cref="BoardCell.AdjacentBombCount"/>
        /// </summary>
        private readonly Board noteBoard;
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
            return solvable.Value;
        }

        private void Compute()
        {
            bool computationAdvanced = false;
            while (this.numUnfoundBombs > 0)
            {
                foreach (BoardCell cell in this.board.Cells)
                {
                    computationAdvanced = false;
                    computationAdvanced |= ConsiderAllNeighborsAreBombs(cell);
                    // TODO: consider no neighbors are bombs
                    // TODO: consider more rules (without breaking if modified)
                }

                if (!computationAdvanced)
                {
                    this.solvable = false;
                    return;
                }
            }

            // we reached this point without any guesses and have found all bombs
            this.solvable = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell"></param>
        /// <returns>Whether the noteBoard has been modified</returns>
        private bool ConsiderAllNeighborsAreBombs(BoardCell cell)
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

            if (revealedSafeCells + cell.AdjacentBombCount == board.GetAdjacentCells(cell.PosX, cell.PosY, cell.PosZ).Count)
            {
                // they are all bombs. Take note on the noteBoard
                foreach (BoardCell bomb in possibleBombs)
                {
                    BoardCell noteBomb = noteBoard.getAt(bomb);
                    noteBomb.IsSuspect = true; 
                }
                // cell has no more unfound bombs around it
                noteBoard.BombCount = 0;
                return true;
            }

            return false;
        }
    }
}
