using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Data;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Assets.Scripts.Solver
{
    public class Solver
    {
        private readonly Board board;
        private bool? solvable;

        private int numUnfoundBombs;

        public Solver(Board board)
        {
            this.board = board;
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
            bool computationAdvancedThisTurn = false;
            int computationEverAdvanced = 0;
            while (this.numUnfoundBombs > 0)
            {
                foreach (BoardCell cell in this.board.Cells)
                {
                    computationAdvancedThisTurn = false;
                    computationAdvancedThisTurn |= ConsiderAllNeighborsAreBombs(cell);
                    computationAdvancedThisTurn |= ConsiderAllNeighborsAreSafe(cell);
                    // TODO: consider no neighbors are bombs
                    // TODO: consider more rules (without breaking if modified)
                }

                if (computationAdvancedThisTurn)
                {
                    computationEverAdvanced++;
                }

                if (!computationAdvancedThisTurn)
                {
                    this.solvable = computationEverAdvanced > 0;
                    return;
                }
            }

            // we reached this point without any guesses and have found all bombs
            this.solvable = true;
        }

        /// <summary>
        /// if the number of adjacent uncertainties equals 0, every uncertainty is safe
        /// </summary>
        /// <param name="cell"></param>
        /// <returns>Whether the noteBoard has been modified during this call</returns>
        private bool ConsiderAllNeighborsAreSafe(BoardCell cell)
        {
            List<BoardCell> neighbors = board.GetAdjacentCells(cell.PosX, cell.PosY, cell.PosZ);
            int numAdjacentSafeCells = neighbors.Count(c => c.State == CellState.Revealed);
            if (cell.AdjacentBombCount - numAdjacentSafeCells == 0 && numAdjacentSafeCells != neighbors.Count)
            {
                // all adjacent unrevealed cells are safe
                neighbors.ForEach(c => c.State = CellState.Revealed);
                return true;
            }

            return false;
        }

        /// <summary>
        /// if the number of adjacent uncertainties equals the number on the cell, every uncertainty is a bomb
        /// </summary>
        /// <param name="cell"></param>
        /// <returns>Whether the noteBoard has been modified</returns>
        private bool ConsiderAllNeighborsAreBombs(BoardCell cell)
        {
            // skip cells we know nothing of
            if (cell.State!=CellState.Revealed)
            {
                return false;
            }

            int revealedSafeCells = 0;
            List<BoardCell> possibleBombs = new List<BoardCell>();
            foreach (BoardCell neighbor in board.GetAdjacentCells(cell.PosX, cell.PosY, cell.PosZ))
            {
                if (neighbor.State == CellState.Revealed)
                {
                    revealedSafeCells++;
                }

                if (neighbor.State != CellState.Revealed)
                {
                    possibleBombs.Add(neighbor);
                }
            }

            if (revealedSafeCells + cell.AdjacentBombCount == board.GetAdjacentCells(cell.PosX, cell.PosY, cell.PosZ).Count)
            {
                // they are all bombs. Take note on the noteBoard
                foreach (BoardCell bomb in possibleBombs)
                {
                    if (bomb.State != CellState.Suspect)
                    {
                        bomb.State = CellState.Suspect;
                        numUnfoundBombs--;
                    }
                }
                
                return true;
            }

            return false;
        }
    }
}
