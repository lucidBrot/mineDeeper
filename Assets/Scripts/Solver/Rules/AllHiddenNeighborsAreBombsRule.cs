using System.Collections.Generic;
using Assets.Scripts.Data;
using Assets.Scripts.GameLogic;

namespace Assets.Scripts.Solver.Rules
{
    /// <summary>
    /// if the number of adjacent uncertainties equals the number on the cell, every uncertainty is a bomb
    /// </summary>
    public class AllHiddenNeighborsAreBombsRule : IRule, IHintRule
    {
        public bool Consider(Board board, BoardCell cell, ICollection<ConsiderationReportForCell> mutableConsiderationReportCollection)
        {
            // skip cells we know nothing of and nude cells
            if (cell.State != CellState.Revealed || cell.IsNude)
            {
                return false;
            }

            var unrevealedNeighborAmount = board.CountNeighbors(cell, c => c.State != CellState.Revealed);
            var revealedNeighborBombsAmount =
                board.CountNeighbors(cell, c => c.State == CellState.Revealed && c.IsBomb);

            if (cell.AdjacentBombCount - revealedNeighborBombsAmount == unrevealedNeighborAmount)
            {
                var hasChanges = false;
                board.ForEachNeighbor(cell, c =>
                {
                    if (c.State != CellState.Revealed && c.State != CellState.Suspect)
                    {
                        mutableConsiderationReportCollection.Add(new ConsiderationReportForCell(c.PosX, c.PosY, c.PosZ, CellState.Suspect));
                        hasChanges = true;
                    }
                });

                return hasChanges;
            }

            return false;
        }

        public Hint GenerateHint(BoardCell consideredCell)
        {
            var cell = consideredCell;
            return new Hint(
                cell, Data.Hint.HintTypes.AllHiddenNeighborsAreBombs,
                "Consider that all hidden neighbors of " + cell.ToString() + " are bombs.", cell);
        }
    }
}