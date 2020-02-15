using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using JetBrains.Annotations;
using Microsoft.Win32.SafeHandles;

namespace Assets.Scripts.Data
{
    public class Hint
    {
        public string Text { get; private set; }
        public BoardCell ConcernedCell { get; private set; }
        public HintTypes HintType { get; private set; }
        public List<BoardCell> CellsToHighlight { get; private set; }

        /**
         * concernedCell and hintType should identify hints that are equal
         */
        public Hint([CanBeNull] BoardCell concernedCell, HintTypes hintType, string text, List<BoardCell> cellsToHighlight)
        {
            this.Text = text;
            this.CellsToHighlight = cellsToHighlight;
            this.ConcernedCell = concernedCell;
            this.HintType = hintType;
        }

        public Hint([CanBeNull] BoardCell concernedCell, HintTypes hintType, string text, BoardCell cellToHighlight) : 
            this(concernedCell, hintType, text, new List<BoardCell>())
        {
            this.CellsToHighlight.Add(cellToHighlight);
        }

        public bool IsEquivalentTo(Hint hint)
        {
            // if the text is the same, the reason is the same, it counts as the same hint
            // But for that to work, the text should contain the cell's coordinates
            // That's why we changed this to also store/compare the reason as an enum and the concerned cell as a cell. 
            // If we ever get a reason to have multiple concerned cells, this will have to be modified. Until then,
            // it is redundant to check for this.Text equality as well as we check for the other criteria already.
            return hint == this || (hint != null && this.Text.Equals(hint.Text) 
                                                 && this.ConcernedCell.Equals(hint.ConcernedCell)
                                                 && this.HintType.Equals(hint.HintType));
        }

        public enum HintTypes
        {
            AllHiddenNeighborsAreBombs,
            AllNeighborsAreSafe,
            MaxAdjacentBombsReached,
            GameWon,
            Bamboozled,
            UserPlacedTooManyFlagsHere,
            AllHiddenNeighborsAreBombsBecauseSomeNeighborsForbidOtherOptions,
        }
    }
}