using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;

namespace Assets.Scripts.Data
{
    public class Hint
    {
        public string Text { get; private set; }
        public List<BoardCell> CellsToHighlight { get; private set; }

        public Hint(string text, List<BoardCell> cellsToHighlight)
        {
            this.Text = text;
            this.CellsToHighlight = cellsToHighlight;
        }

        public Hint(string text, BoardCell cellToHighlight) : this(text, new List<BoardCell>())
        {
            this.CellsToHighlight.Add(cellToHighlight);
        }

        public bool IsSameHintAs(Hint hint)
        {
            // if the text is the same, the reason is the same, it counts as the same hint
            // The text should contain the cell's coordinates
            // TODO: instead store cell coordinates in a field and check it here?
            return hint == this || (hint != null && this.Text.Equals(hint.Text));
        }
    }
}