using System.Collections.Generic;
using Assets.Scripts.Data;
using Assets.Scripts.GameLogic;

namespace Assets.Scripts.Solver.Rules
{
    /// <summary>
    /// if the number of adjacent uncertainties equals 0, every uncertainty is safe
    /// </summary>
    public class AllNeighborsAreSafeRule : IRule, IHintRule
    {
        public bool Consider(in Board board, BoardCell cell, ICollection<ConsiderationReportForCell> mutableConsiderationReportCollection)
        {
            if (cell.State != CellState.Revealed)
            {
                return false;
            }

            if (cell.IsNude)
            {
                var hadUnrevealed = false;
                board.ForEachNeighbor(cell, c => hadUnrevealed |= c.State != CellState.Revealed);
                if (hadUnrevealed)
                {
                    mutableConsiderationReportCollection.Add(new ConsiderationReportForCell(cell.PosX, cell.PosY, cell.PosZ, CellState.Revealed));
                }

                return hadUnrevealed;
            }

            return false;
        }

        public Hint GenerateHint(BoardCell consideredCell)
        {
            var cell = consideredCell;
            return new Hint(cell, Data.Hint.HintTypes.AllNeighborsAreSafe,
                "Consider that all neighbors of " + cell.ToString() + " are certainly safe.", cell);
        }
    }
}