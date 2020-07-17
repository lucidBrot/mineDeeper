using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Assets.Scripts.Data
{
    public class Board : IEnumerable<BoardCell>, INotifyPropertyChanged
    {
        private BoardCell HighlightedCell;
        private int flagCount;
        private int bombCount;
        private int unknownCount;

        public int Width { get; }

        public int Height { get; }

        public int Depth { get; }

        public BoardCell[] Cells { get; }

        public int BombCount
        {
            get => bombCount;
            set
            {
                if (value == bombCount) return;
                bombCount = value;
                OnPropertyChanged();
            }
        }

        public int FlagCount
        {
            get => flagCount;
            set
            {
                if (value == flagCount) return;
                flagCount = value;
                OnPropertyChanged();
            }
        }

        public int UnknownCount
        {
            get => unknownCount;
            set
            {
                if (value == unknownCount) return;
                unknownCount = value;
                OnPropertyChanged();
            }
        }

        [DebuggerDisplay("{this[0]}")]
        public BoardCell this[int x, int y, int z]
        {
            get { return Cells[x + y * Width + z * Width * Height]; }
            private set { Cells[x + y * Width + z * Width * Height] = value; }
        }

        public Board(int width, int height, int depth)
        {
            this.Width = width;
            this.Height = height;
            this.Depth = depth;

            this.Cells = new BoardCell[width * height * depth];
            this.BuildBoard();
            this.ConnectCells();
        }

        /// <summary>
        /// Sets the isBomb flag of the cell at the given position on this Board.
        /// Also updates the count of adjacent cells to reflect this
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="isBomb"></param>
        public void SetBombState(int posX, int posY, int posZ, bool isBomb)
        {
            BoardCell cell = this[posX, posY, posZ];
            bool prev = cell.IsBomb;
            if (prev == isBomb)
            {
                return;
            }
            cell.IsBomb = isBomb;
            int modifier = isBomb ? 1 : -1;

            foreach (BoardCell neighbor in this.GetAdjacentCells(posX, posY, posZ))
            {
                neighbor.AdjacentBombCount += modifier;
            }

            this.BombCount += modifier;
        }

        public List<BoardCell> GetAdjacentCells(int posX, int posY, int posZ)
        {
            List<BoardCell> neighbors = new List<BoardCell>();
            for (int xx = -1; xx <= 1; xx++)
            {
                for (int yy = -1; yy <= 1; yy++)
                {
                    for (int zz = -1; zz <= 1; zz++)
                    {
                        int x = posX + xx;
                        int y = posY + yy;
                        int z = posZ + zz;
                    
                        // if not out of bounds
                        if (x >= 0 && x < this.Width &&
                            y >= 0 && y < this.Height &&
                            z >= 0 && z < this.Depth      )
                        {
                            // if not the cell itself
                            if (!(xx == 0 && yy == 0 && zz == 0)) { neighbors.Add(this[x,y,z]);}
                        }
                    }
                }
            }

            return neighbors;
        }

        public void ResetCellStates()
        {
            foreach (var cell in Cells)
            {
                cell.State = CellState.Default;
            }
        }

        public void ResetBoard()
        {
            foreach (var cell in Cells)
            {
                cell.IsBomb = false;
                cell.AdjacentBombCount = 0;
                cell.State = CellState.Default;
            }
        }

        public IEnumerator<BoardCell> GetEnumerator()
        {
            return ((IEnumerable<BoardCell>)Cells).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Highlight(BoardCell cell)
        {
            if (this.HighlightedCell == cell)
            {
                return;
            }

            if (this.HighlightedCell != null)
            {
                this.HighlightedCell.Highlighted = false;
            }

            this.HighlightedCell = cell;
            cell.Highlighted = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void BuildBoard()
        {
            for (var x = 0; x < this.Width; x++)
            {
                for (var y = 0; y < this.Height; y++)
                {
                    for (var z = 0; z < this.Depth; z++)
                    {
                        this[x, y, z] = new BoardCell(x, y, z);
                    }
                }
            }
        }

        private void ConnectCells()
        {
            var cellCache = new List<BoardCell>();

            for (var x = 0; x < this.Width; x++)
            {
                for (var y = 0; y < this.Height; y++)
                {
                    for (var z = 0; z < this.Depth; z++)
                    {
                        cellCache.Clear();
                        this.GetNeighbors(x, y, z, cellCache);

                        this[x, y, z].Neighbors = cellCache.ToArray();
                    }
                }
            }
        }

        private void GetNeighbors(int x, int y, int z, List<BoardCell> output)
        {
            for (var a = -1; a <= 1; a++)
            {
                if (x + a < 0 || x + a >= this.Width)
                {
                    continue;
                }

                for (var b = -1; b <= 1; b++)
                {
                    if (b + y < 0 || b + y >= this.Height)
                    {
                        continue;
                    }

                    for (var c = -1; c <= 1; c++)
                    {
                        if (c + z < 0 || c + z >= this.Depth)
                        {
                            continue;
                        }

                        if (a == 0 && b == 0 && c == 0)
                        {
                            continue;
                        }

                        output.Add(this[a + x, b + y, c + z]);
                    }
                }
            }
        }
    }
}
