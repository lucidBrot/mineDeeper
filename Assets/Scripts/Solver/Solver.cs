using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Solver.Rules;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Solver
{
    public class Solver
    {
        private readonly Board board;
        private int numUnfoundBombs;
        
        /// <summary>
        /// Used to notice when an Abort() has been requested.
        /// </summary>
        private bool ShouldAbort { get; set; }
        
        public bool HasAborted { get; private set; }

        /// <summary>
        /// Used for code legibility as a constant to denote that we aborted the solver and returned `false`
        /// because we did not find the board to be solvable
        /// </summary>
        private const bool ABORTED = false;

        public Solver(Board board)
        {
            this.board = board;
            this.numUnfoundBombs = board.BombCount;
            this.ShouldAbort = false;
            this.HasAborted = false;
        }

        /// <summary>
        /// Expects a board that already offers information to the player.
        /// </summary>
        /// <returns>True if the board is solvable without guessing, False otherwise</returns>
        public bool IsSolvable()
        {
            if (this.HasAborted)
            {
                // safeguard to avoid accidental reuse of solver. 
                // if you were to reuse an aborted solver without having reset ShouldAbort and HasAborted to false,
                // you would not be able to abort it or it would abort itself.
                return ABORTED;
            }
            return Compute();
        }


        private bool Compute()
        {
            while (this.numUnfoundBombs > 0)
            {
                var computationAdvancedThisTurn = false;
                foreach (BoardCell cell in this.board.Cells)
                {
                    if (ShouldAbort)
                    {
                        this.HasAborted = true;
                        return ABORTED;
                    }
                    
                    // below assertion is wrong because it should also consider the amount of revealed neighboring bombs
                    //Debug.Assert(cell.AdjacentBombCount <= board.CountNeighbors(cell, c => c.State != CellState.Revealed));
                    if (cell.IsBomb && cell.State == CellState.Revealed)
                    {
                        continue;
                    }

                    Debug.Assert(!cell.IsBomb || cell.State != CellState.Revealed);
                    // using `if (!computationAdvancedThisTurn)` to short-circuit.
                    // On second thought, It could be better or worse than using |= which does not short-circuit -
                    // because if all options for one cell are checked at the same cell, caching might work better.
                    if (!computationAdvancedThisTurn)
                    {
                        computationAdvancedThisTurn = ConsiderAllHiddenNeighborsAreBombs(cell, modifyBoard: true);
                    }

                    if (!computationAdvancedThisTurn)
                    {
                        computationAdvancedThisTurn = ConsiderAllNeighborsAreSafe(cell, modifyBoard: true);
                    }

                    if (!computationAdvancedThisTurn)
                    {
                        computationAdvancedThisTurn = ConsiderTheLackOfRemainingAdjacentBombs(cell, modifyBoard: true);
                    }

                    if (!computationAdvancedThisTurn)
                    {
                        // this is a more costly operation and hence should only be tried if others don't help
                        computationAdvancedThisTurn =
                            ConsiderAllOptionsForTwoBombsAndFindThatOnlyOneOptionIsLegal(cell, modifyBoard: true,
                                out _);
                    }

                    if (computationAdvancedThisTurn)
                    {
                        break;
                    }

                    // TODO: consider more rules (without breaking if modified). Remember to also update Hint.
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

            // hint about provably incorrect suspicions of the user
            foreach (BoardCell wronglyFlaggedCell in board.Where(c => !c.IsBomb && c.State == CellState.Suspect))
            {
                Hint hint = UserCouldSeeThatThisFlagIsWrongUnlessThisFunctionReturnsNull(board, wronglyFlaggedCell);
                if (hint != null)
                {
                    return hint;
                }
            }

            // heuristics from solver as hints
            foreach (BoardCell cell in solver.board.Cells)
            {
                // if it is a revealed bomb, it's a weird case we didn't think of before. It's safer to just ignore that for now
                if (cell.IsBomb && cell.State == CellState.Revealed)
                {
                    continue;
                }
                Debug.Assert(!cell.IsBomb || cell.State != CellState.Revealed);

                if (solver.ConsiderAllHiddenNeighborsAreBombs(cell, modifyBoard: false))
                {
                    return new Hint(
                        cell, Data.Hint.HintTypes.AllHiddenNeighborsAreBombs,
                        "Consider that all hidden neighbors of " + cell.ToString() + " are bombs.", cell);
                }

                if (solver.ConsiderAllNeighborsAreSafe(cell, modifyBoard: false))
                {
                    return new Hint(cell, Data.Hint.HintTypes.AllNeighborsAreSafe,
                        "Consider that all neighbors of " + cell.ToString() + " are certainly safe.", cell);
                }

                if (solver.ConsiderTheLackOfRemainingAdjacentBombs(cell, modifyBoard: false))
                {
                    return new Hint(cell, Data.Hint.HintTypes.MaxAdjacentBombsReached,
                        "Consider that there can not be any more bombs around " + cell.ToString() +
                        " than you already found.", cell);
                }

                // this is a more costly operation and it is harder for the user to see
                Tuple<BoardCell, BoardCell> bombPair;
                if (solver.ConsiderAllOptionsForTwoBombsAndFindThatOnlyOneOptionIsLegal(cell, modifyBoard:false, out bombPair))
                {
                    List<BoardCell> l;
                    if (bombPair == null)
                    {
                        Debug.Log("bombPair for hint is null, but a solution was found. This should never happen. Highlighting the concerned Cell...");
                        l = new List<BoardCell>() {cell};
                    }
                    else
                    {
                        l = new List<BoardCell>() {bombPair.Item1, bombPair.Item2};
                    }

                    return new Hint(cell,
                        Data.Hint.HintTypes.ThereIsOnlyOneLegalOptionToArrangeTheTwoMissingBombs,
                        "There is only one way the two missing bombs around " + cell.ToString() + " can be placed.",
                        cell);
                }

                // TODO: Need to modify this code whenever the solver.Compute function is modified. Bad.
            }

            if (solver.numUnfoundBombs == 0)
            {
                return new Hint(null, Data.Hint.HintTypes.GameWon, "Won.", new List<BoardCell>());
            }

            return new Hint(null, Data.Hint.HintTypes.Bamboozled, "Look, I'm bamboozled. We're stuck",
                new List<BoardCell>());
        }

        /// <summary>
        /// Hints about absolutely obviously wrong suspects
        /// </summary>
        /// <param name="board"></param>
        /// <param name="wronglyFlaggedCell"></param>
        /// <returns>null if no hint found, otherwise a hint</returns>
        private static Hint UserCouldSeeThatThisFlagIsWrongUnlessThisFunctionReturnsNull(Board board,
            BoardCell wronglyFlaggedCell)
        {
            foreach (BoardCell revealedNeighbor in board.GetAdjacentCells(wronglyFlaggedCell.PosX,
                wronglyFlaggedCell.PosY,
                wronglyFlaggedCell.PosZ).Where(c => c.State == CellState.Revealed))
            {
                int numFlagsAroundRevealedNeighbor =
                    board.CountNeighbors(revealedNeighbor, c => c.State == CellState.Suspect);
                if (revealedNeighbor.AdjacentBombCount < numFlagsAroundRevealedNeighbor)
                {
                    // there must be a wrong flag there
                    return new Hint(revealedNeighbor, Data.Hint.HintTypes.UserPlacedTooManyFlagsHere,
                        "Too many flags around " + revealedNeighbor.ToString(), revealedNeighbor);
                }
            }

            return null;
        }

        /// <summary>
        /// If there are already N suspects among the neighbors of the cell,
        /// then the remaining neighbors are all clean and can be revealed.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private bool ConsiderTheLackOfRemainingAdjacentBombs(BoardCell cell, bool modifyBoard)
        {
            // only perform this check for cells that are not bombs (i.E. cell.State!=Suspect).
            // Because bombs carry no information about their neighbours
            // only perform this check for cells that we know the adjacent bomb count of (i.e. cell.State==Revealed)
            // only perform this check if there are unrevealed unsuspect neighbors => skip if there are only revealed neighbors
            if (cell.State != CellState.Revealed ||
                board.GetAdjacentCells(cell.PosX, cell.PosY, cell.PosZ)
                    .All(c => c.State == CellState.Revealed || c.State == CellState.Suspect)
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
            ICollection<ConsiderationReportForCell> report = new List<ConsiderationReportForCell>();
            IRule rule = new AllHiddenNeighborsAreBombsRule();
            bool computationAdvanced = rule.Consider(this.board, cell, report);
            
            // todo: for paralellization, consider carefully how the unfoundBombs are updated! Best is only once, to avoid counting the same finding twice. Probably should compute it from the list instead and remove the out param.
            // Count number of found Bombs
            int numNewBombsFound = report.Distinct().Count(rep => rep.TargetState == CellState.Suspect);
            this.numUnfoundBombs -= numNewBombsFound;
            
            // update board if that is wished for
            if (modifyBoard)
            {
                // todo: take rule instance from more global list and set new state to board once (and not just everything for each rule)
                // i.e. below block shall be removed soon
                foreach (var c in report)
                {
                    this.board[c.PosX, c.PosY, c.PosZ].State = c.TargetState;
                }
            }

            return computationAdvanced;
        }

        private bool ConsiderAllOptionsForTwoBombsAndFindThatOnlyOneOptionIsLegal(BoardCell cell, bool modifyBoard, [CanBeNull] out Tuple<BoardCell, BoardCell> bombsFound)
        {
            // by default, we found nothing
            bombsFound = null;

            // skip cells we know nothing of and nude cells
            if (cell.State != CellState.Revealed || cell.IsNude)
            {
                return false;
            }

            // consider only cells with exactly 2 missing bombs
            if (cell.AdjacentBombCount - 
                board.NeighborsOf(cell).Count(
                    c => c.State == CellState.Suspect || (c.IsBomb && c.State == CellState.Revealed)) != 2
                )
            {
                return false;
            }

            // we will want to store all possibilities
            List<Tuple<BoardCell, BoardCell>> possibleBombPairs = new List<Tuple<BoardCell, BoardCell>>(2);

            // get all unrevealed neighbors - those could be bombs
            var unrevealedNeighbors = board.NeighborsOf(cell).Where(c => c.State != CellState.Revealed)
                .OrderBy(x => x.PosX).ThenBy(x => x.PosY).ThenBy(x => x.PosZ).ToList();
            foreach (BoardCell possibleBomb1 in unrevealedNeighbors)
            {
                // would that bomb even be valid?
                if (board.NeighborsOf(possibleBomb1).Where(cll => cll.State == CellState.Revealed).Any(
                    // has already enough bombs
                    c => board.NeighborsOf(c).Count(n => (n.IsBomb && n.State == CellState.Revealed) || n.State == CellState.Suspect) >= c.AdjacentBombCount
                    ))
                {
                    continue;
                }

                // try all other bombs
                var unrevealedNeighborsReversed = Enumerable.Reverse(unrevealedNeighbors);
                foreach (BoardCell possibleBomb2 in unrevealedNeighborsReversed)
                {
                    // we only need to consider each couple (a, b) once.
                    // Since the inner loop is reversed, we can stop the inner loop once a == b
                    if (possibleBomb1 == possibleBomb2)
                    {
                        break;
                    }

                    // would that bomb2 even be valid?
                    if (board.NeighborsOf(possibleBomb2).Where(cll => cll.State == CellState.Revealed).Any(
                        // has already enough bombs
                        c => board.NeighborsOf(c).Count(n => (n.State == CellState.Revealed && n.IsBomb) || n.State == CellState.Suspect) >=
                             c.AdjacentBombCount
                    ))
                    {
                        continue;
                    }

                    // Each possibleBomb on its own would be valid. Would the combination still be?
                    // Check all cells that are neighbors of of both bombs for whether they disagree
                    bool atLeastOneJudgeDisagrees = false;
                    var neighborsOfBothPossibleBombs = board.NeighborsOf(possibleBomb1)
                        .Intersect(board.NeighborsOf(possibleBomb2));
                    foreach (BoardCell judge in neighborsOfBothPossibleBombs.Where(c => c.State == CellState.Revealed && !c.IsBomb))
                    {
                        if (board.NeighborsOf(judge)
                                     .Count(n => (n.State == CellState.Revealed 
                                            && n.IsBomb ) || n.State == CellState.Suspect) 
                                            + 2 > judge.AdjacentBombCount
                        )
                        {
                            // at least one judge disagrees. Do not store this option and continue checking other options
                            atLeastOneJudgeDisagrees = true;
                            break;
                        }
                    }

                    if (!atLeastOneJudgeDisagrees)
                    {
                        // everything fine, store this as a possible option
                        possibleBombPairs.Add(new Tuple<BoardCell, BoardCell>(possibleBomb1, possibleBomb2));
                        if (possibleBombPairs.Count > 1)
                        {
                            // too many options - the solution is not unique
                            return false;
                        }
                    }

                }

            }

            if(possibleBombPairs.Count == 0)
            {
                // nothing found
                return false;
            }

            Debug.Assert(possibleBombPairs.Count == 1);

            if (modifyBoard)
            {
                possibleBombPairs[0].Item1.State = CellState.Suspect;
                possibleBombPairs[0].Item2.State = CellState.Suspect;
                numUnfoundBombs -= 2;
            }

            bombsFound = possibleBombPairs[0];
            return true;

        }
        /// <summary>
        /// Abort the Solver as soon as befitting it, discarding any useful result.
        /// </summary>
        public void Abort()
        {
            this.ShouldAbort = true;
        }

    }
}
