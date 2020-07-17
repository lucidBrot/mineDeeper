using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.GameLogic;
using UnityEngine;

namespace Assets.Scripts.Solver.Rules
{
    public class TheSetOfAllOptionsForTwoBombsConsistsOfOnlyOneOptionThatIsLegalRule : IRule, IHintRule
    {
        public bool Consider(in Board board, BoardCell cell, ICollection<ConsiderationReportForCell> mutableConsiderationReportCollection)
        {
            // by default, we found nothing
            // Could be used as output parameter, but is never used.
            Tuple<BoardCell, BoardCell> bombsFound = null;

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
                var board1 = board; // need that copy to use the board in the lambda. Whyever...
                if (board.NeighborsOf(possibleBomb1).Where(cll => cll.State == CellState.Revealed).Any(
                    // has already enough bombs
                    (BoardCell c) => board1.NeighborsOf(c).Count(n => (n.IsBomb && n.State == CellState.Revealed) || n.State == CellState.Suspect) >= c.AdjacentBombCount
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
                    var board2 = board;
                    if (board.NeighborsOf(possibleBomb2).Where(cll => cll.State == CellState.Revealed).Any(
                        // has already enough bombs
                        c => board2.NeighborsOf(c).Count(n => (n.State == CellState.Revealed && n.IsBomb) || n.State == CellState.Suspect) >=
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

            BoardCell b1 = possibleBombPairs[0].Item1;
            BoardCell b2 = possibleBombPairs[0].Item2;
            mutableConsiderationReportCollection.Add(new ConsiderationReportForCell(b1, CellState.Suspect));
            mutableConsiderationReportCollection.Add(new ConsiderationReportForCell(b2, CellState.Suspect));
            
            // technically could be output parameter but is not used
            bombsFound = possibleBombPairs[0];
            
            return true;
        }

        public Hint GenerateHint(BoardCell consideredCell)
        {
            return new Hint(consideredCell,
                Data.Hint.HintTypes.ThereIsOnlyOneLegalOptionToArrangeTheTwoMissingBombs,
                "There is only one way the two missing bombs around " + consideredCell.ToString() + " can be placed.",
                consideredCell);
        }
    }
}