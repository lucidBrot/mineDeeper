using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Data;
using Assets.Scripts.GameLogic;
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
                computationAdvancedThisTurn = false;
                foreach (BoardCell cell in this.board.Cells)
                {
                    computationAdvancedThisTurn |= ConsiderAllHiddenNeighborsAreBombs(cell);
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
            if (cell.IsNude)
            {
                var hadUnrevealed = false;
                board.ForEachNeighbor(cell, c => hadUnrevealed |= c.State != CellState.Revealed);
                board.Reveal(cell);
                return hadUnrevealed;
            }

            return false;
        }

        /// <summary>
        /// if the number of adjacent uncertainties equals the number on the cell, every uncertainty is a bomb
        /// </summary>
        /// <param name="cell"></param>
        /// <returns>Whether the noteBoard has been modified</returns>
        private bool ConsiderAllHiddenNeighborsAreBombs(BoardCell cell)
        {
            // skip cells we know nothing of and nude cells
            if (cell.State != CellState.Revealed || cell.IsNude)
            {
                return false;
            }

            var unrevealedNeighborAmount = 0;

            board.ForEachNeighbor(cell, c =>
            {
                if (c.State != CellState.Revealed)
                {
                    unrevealedNeighborAmount++;
                }
            });

            if (cell.AdjacentBombCount == unrevealedNeighborAmount)
            {
                var hasChanges = false;

                board.ForEachNeighbor(cell, c =>
                {
                    if (c.State != CellState.Revealed && c.State != CellState.Suspect)
                    {
                        c.State = CellState.Suspect;
                        hasChanges = true;
                    }
                });

                return hasChanges;
            }

            return false;
        }
    }
}
