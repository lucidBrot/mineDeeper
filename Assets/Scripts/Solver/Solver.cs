using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public static Hint Hint(Board board)
        {
            Solver solver = new Solver(board);

            // TODO: hint about provably incorrect suspicions of the user
            foreach (BoardCell wronglyFlaggedCell in board.Where(c => !c.IsBomb && c.State == CellState.Suspect))
            {
                Hint hint = UserCouldSeeThatThisFlagIsWrongUnlessThisFunctionReturnsNull(board, wronglyFlaggedCell);
                if (hint != null)
                {
                    return hint; 
                }
            }

            // heuristics from solver as hints
            var computationAdvancedThisTurn = false;
            foreach (BoardCell cell in solver.board.Cells)
            {
                Debug.Assert(cell.AdjacentBombCount <= board.CountNeighbors(cell, c => c.State != CellState.Revealed));
                Debug.Assert(!cell.IsBomb || cell.State != CellState.Revealed);

                computationAdvancedThisTurn |= solver.ConsiderAllHiddenNeighborsAreBombs(cell, modifyBoard:false);
                if (computationAdvancedThisTurn)
                {
                    return new Hint("Consider that all hidden neighbors of "+cell.ToString()+" are bombs.", cell);
                }

                computationAdvancedThisTurn |= solver.ConsiderAllNeighborsAreSafe(cell, modifyBoard: false);
                if (computationAdvancedThisTurn)
                {
                    return new Hint("Consider that all neighbors of " + cell.ToString() + " are certainly safe.", cell);
                }

                computationAdvancedThisTurn |= solver.ConsiderTheLackOfRemainingAdjacentBombs(cell, modifyBoard: false);
                if (computationAdvancedThisTurn)
                {
                    board.Highlight(cell);
                    return new Hint("Consider that there can not be any more bombs around " + cell.ToString() + " than you already found.", cell);
                }
                // TODO: Need to modify this code whenever the solver.Compute function is modified. Bad.
            }

            if (solver.numUnfoundBombs == 0)
            {
                return new Hint("Won.", new List<BoardCell>());
            }

            return new Hint("Look, I'm bamboozled. We're stuck", new List<BoardCell>());
        }

        /// <summary>
        /// Hints about absolutely obviously wrong suspects
        /// </summary>
        /// <param name="board"></param>
        /// <param name="wronglyFlaggedCell"></param>
        /// <returns>null if no hint found, otherwise a hint</returns>
        private static Hint UserCouldSeeThatThisFlagIsWrongUnlessThisFunctionReturnsNull(Board board, BoardCell wronglyFlaggedCell)
        {
            foreach (BoardCell revealedNeighbor in board.GetAdjacentCells(wronglyFlaggedCell.PosX, wronglyFlaggedCell.PosY,
                wronglyFlaggedCell.PosZ).Where(c => c.State==CellState.Revealed))
            {
                int numFlagsAroundRevealedNeighbor =
                    board.CountNeighbors(revealedNeighbor, c => c.State == CellState.Suspect);
                if (revealedNeighbor.AdjacentBombCount < numFlagsAroundRevealedNeighbor)
                {
                    // there must be a wrong flag there
                    return new Hint("Too many flags around " + revealedNeighbor.ToString(), revealedNeighbor);
                }
            }

            return null;
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
            // only perform this check if there are unrevealed unsuspect neighbors => skip if there are only revealed neighbors
            if (cell.State != CellState.Revealed || 
                board.GetAdjacentCells(cell.PosX, cell.PosY, cell.PosZ).All(c => c.State == CellState.Revealed || c.State == CellState.Suspect)
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
