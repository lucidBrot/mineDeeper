using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.GameLogic;

namespace Assets.Scripts.Solver.Rules
{
    public class LackOfRemainingAdjacentBombsRule : IRule
    {
        public bool Consider(Board board, BoardCell cell, ICollection<ConsiderationReportForCell> mutableConsiderationReportCollection)
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
                    if (neighbor.State != CellState.Suspect)
                    {
                        mutableConsiderationReportCollection.Add(new ConsiderationReportForCell(neighbor.PosX, neighbor.PosY, neighbor.PosZ, CellState.Revealed));
                    }
                });
                return true;
            }

            return false;
        }
    }
}