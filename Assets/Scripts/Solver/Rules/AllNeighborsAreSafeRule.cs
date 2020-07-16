using System.Collections.Generic;
using Assets.Scripts.Data;
using Assets.Scripts.GameLogic;

namespace Assets.Scripts.Solver.Rules
{
    public class AllNeighborsAreSafeRule : IRule
    {
        public bool Consider(Board board, BoardCell cell, ICollection<ConsiderationReportForCell> mutableConsiderationReportCollection)
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
    }
}