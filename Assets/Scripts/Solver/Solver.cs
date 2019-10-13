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
            return Compute();
        }

        private bool Compute()
        {
            while (this.numUnfoundBombs > 0)
            {
                var computationAdvancedThisTurn = false;
                foreach (BoardCell cell in this.board.Cells)
                {
                    Debug.Assert(cell.AdjacentBombCount <= board.CountNeighbors(cell, c => c.State != CellState.Revealed));
                    Debug.Assert(!cell.IsBomb || cell.State != CellState.Revealed);
                    computationAdvancedThisTurn |= ConsiderAllHiddenNeighborsAreBombs(cell, modifyBoard: true);
                    computationAdvancedThisTurn |= ConsiderAllNeighborsAreSafe(cell, modifyBoard: true);
                    computationAdvancedThisTurn |= ConsiderTheLackOfRemainingAdjacentBombs(cell, modifyBoard: true);
                    // TODO: consider more rules (without breaking if modified)
                }

                if (!computationAdvancedThisTurn)
                {
                    return numUnfoundBombs == 0;
                }
            }

            // we reached this point without any guesses and have found all bombs
            return true;
        }

        public static String Hint(Board board)
        {
            Solver solver = new Solver(board);
            // TODO: ensure we are not modifying the original board state
            var computationAdvancedThisTurn = false;
            foreach (BoardCell cell in solver.board.Cells)
            {
                Debug.Assert(cell.AdjacentBombCount <= board.CountNeighbors(cell, c => c.State != CellState.Revealed));
                Debug.Assert(!cell.IsBomb || cell.State != CellState.Revealed);

                computationAdvancedThisTurn |= solver.ConsiderAllHiddenNeighborsAreBombs(cell, modifyBoard:false);
                if (computationAdvancedThisTurn)
                {
                    return "Consider that all hidden neighbors of "+cell.ToString()+" are bombs.";
                }

                computationAdvancedThisTurn |= solver.ConsiderAllNeighborsAreSafe(cell, modifyBoard: false);
                if (computationAdvancedThisTurn)
                {
                    return "Consider that all neighbors of " + cell.ToString() + " are certainly safe.";
                }

                computationAdvancedThisTurn |= solver.ConsiderTheLackOfRemainingAdjacentBombs(cell, modifyBoard: false);
                if (computationAdvancedThisTurn)
                {
                    return "Consider that there can not be any more bombs around " + cell.ToString() + " than you already found.";
                }
                // TODO: Need to modify this code whenever the solver.Compute function is modified. Bad.
            }

            if (solver.numUnfoundBombs == 0)
            {
                return "Won.";
            }

            return "Look, I'm bamboozled. We're stuck";
        }

        /// <summary>
        /// If there are already N suspects among the neighbors of the cell, then the remaining neighbors are all clean and can be revealed.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private bool ConsiderTheLackOfRemainingAdjacentBombs(BoardCell cell, bool modifyBoard)
        {
            // only perform this check for cells that are not bombs (i.E. cell.State!=Suspect). Because bombs carry no information about their neighbours
            // only perform this check for cells that we know the adjacent bomb count of (i.e. cell.State==Revealed)
            // only perform this check if there are unrevealed neighbors
            if (cell.State != CellState.Revealed || 
                board.CountNeighbors(cell, c => c.State!=CellState.Revealed)==0
                )
            {
                return false;
            }

            int numSurroundingSuspects = board.CountNeighbors(cell, n => n.State == CellState.Suspect);
            if (numSurroundingSuspects == cell.AdjacentBombCount)
            {
                board.ForEachNeighbor(cell, neighbor =>
                {
                    if (neighbor.State != CellState.Suspect && modifyBoard)
                    {
                        board.Reveal(neighbor);
                    }
                });
                return true;
            }

            return false;
        }

        /// <summary>
        /// if the number of adjacent uncertainties equals 0, every uncertainty is safe
        /// </summary>
        /// <param name="cell"></param>
        /// <returns>Whether the noteBoard has been modified during this call</returns>
        private bool ConsiderAllNeighborsAreSafe(BoardCell cell, bool modifyBoard)
        {
            if (cell.State != CellState.Revealed)
            {
                return false;
            }

            if (cell.IsNude)
            {
                var hadUnrevealed = false;
                board.ForEachNeighbor(cell, c => hadUnrevealed |= c.State != CellState.Revealed);
                if (hadUnrevealed && modifyBoard)
                {
                    board.Reveal(cell);
                }

                return hadUnrevealed;
            }

            return false;
        }

        /// <summary>
        /// if the number of adjacent uncertainties equals the number on the cell, every uncertainty is a bomb
        /// </summary>
        /// <returns>Whether the noteBoard has been modified</returns>
        private bool ConsiderAllHiddenNeighborsAreBombs(BoardCell cell, bool modifyBoard)
        {
            // skip cells we know nothing of and nude cells
            if (cell.State != CellState.Revealed || cell.IsNude)
            {
                return false;
            }

            var unrevealedNeighborAmount = board.CountNeighbors(cell, c => c.State != CellState.Revealed);

            if (cell.AdjacentBombCount == unrevealedNeighborAmount)
            {
                var hasChanges = false;

                board.ForEachNeighbor(cell, c =>
                {
                    if (c.State != CellState.Revealed && c.State != CellState.Suspect)
                    {
                        if (modifyBoard)
                        {
                            c.State = CellState.Suspect;
                        }

                        hasChanges = true;
                        numUnfoundBombs--;
                    }
                });

                return hasChanges;
            }

            return false;
        }
    }
}
