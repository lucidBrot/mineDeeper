using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Solver.Rules;
using JetBrains.Annotations;
using Unity_Tools.Core;

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
        
        // This list contains all (nicely behaving, i.e. not the above) IHintRules so that they will be checked
        // in the order they are listed here.
        private readonly List<IHintRule> hintRules = new List<IHintRule>
        {
            new AllHiddenNeighborsAreBombsRule(),
            new AllNeighborsAreSafeRule(),
            new LackOfRemainingAdjacentBombsRule(),
            new TheSetOfAllOptionsForTwoBombsConsistsOfOnlyOneOptionThatIsLegalRule(),
        };
        // TODO: Need to modify this list whenever the solver.Compute function is modified. Bad.
        
        // List of Rules to consider in order (for solving)
        private readonly List<IRule> rules = new List<IRule>()
        {
            new AllHiddenNeighborsAreBombsRule(),
            new AllNeighborsAreSafeRule(),
            new LackOfRemainingAdjacentBombsRule(),
            new TheSetOfAllOptionsForTwoBombsConsistsOfOnlyOneOptionThatIsLegalRule(),
        };

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

                    // Commented out because UnityEngine is not threadsafe
                    ///Debug.Assert(!cell.IsBomb || cell.State != CellState.Revealed);

                    foreach (var rule in rules)
                    {
                        // using `if (!computationAdvancedThisTurn)` to short-circuit.
                        // On second thought, It could be better or worse than using |= which does not short-circuit -
                        // because if all options for one cell are checked at the same cell, caching might work better.
                        if (!computationAdvancedThisTurn)
                        {
                            computationAdvancedThisTurn =
                                SolveConsideringTheRule(rule, cell);
                        }
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
                // Commented out because UnityEngine is not threadsafe
                ///Debug.Assert(!cell.IsBomb || cell.State != CellState.Revealed);
                
                // generate a Hint if possible
                foreach (var hintRule in solver.hintRules) {
                    if (solver.ConsiderTheRule(hintRule, cell, false))
                    {
                        return hintRule.GenerateHint(cell);
                    }
                }
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
        /// Hence this is not useful for the Solver but is useful for hints.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="wronglyFlaggedCell"></param>
        /// <returns>null if no hint found, otherwise a hint</returns>
        private static Hint UserCouldSeeThatThisFlagIsWrongUnlessThisFunctionReturnsNull(Board board,
            BoardCell wronglyFlaggedCell)
        {   // this function is not transformed into an IHintRule because the hint requires access to `revealedNeighbor` 
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
        /// Modifies the solver's board if the rule is useful and returns true iff it the rule is useful i.e. helped progress.
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="cell"></param>
        /// <returns></returns>
        private bool SolveConsideringTheRule(IRule rule, BoardCell cell)
        {
            return ConsiderTheRule(rule, cell, true);
        }

        private bool ConsiderTheRule(IRule rule, BoardCell cell, bool modifyBoard)
        {
            ICollection<ConsiderationReportForCell> report = new List<ConsiderationReportForCell>();
            bool computationAdvanced = rule.Consider(this.board, cell, report);

            if (modifyBoard)
            {
                // todo: for paralellization, consider carefully how the unfoundBombs are updated! Best is only once, to avoid counting the same finding twice. Probably should compute it from the list instead and remove the out param.
                // todo: make this method static in the end.
                // reduce number of Unfound Bombs
                report.Where(c => c.TargetState == CellState.Suspect).Distinct()
                    .ForAll(cc =>
                    {
                        numUnfoundBombs--;
                        board[cc.PosX, cc.PosY, cc.PosZ].State = CellState.Suspect;
                    });
                
                // reveal safe cells
                report.Where(c => c.TargetState == CellState.Revealed).Distinct()
                    .ForAll(cc => 
                        board.Reveal(board[cc.PosX, cc.PosY, cc.PosZ]
                        ));
            }

            // todo: check if some cell is in both sets (or is already set to Suspect but should be revealed according to some rule now) and generate according hint to the user.
            return computationAdvanced;
        }
        
        
        
        /// <summary>
        /// Abort the Solver as soon as befitting it, discarding any useful result.
        /// </summary>
        public void Abort()
        {
            this.ShouldAbort = true;
        }

        public Tuple<List<IRule>, List<IHintRule>> GetRuleListsForTesting()
        {
            return new Tuple<List<IRule>, List<IHintRule>>(this.rules, this.hintRules);
        }

    }
}
