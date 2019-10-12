using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public int Width { get; }

    public int Height { get; }

    public int Depth { get; }

    public BoardCell[,,] Cells { get; }

    public int BombCount { get; set; }

    public Board(int width, int height, int depth)
    {
        this.Width = width;
        this.Height = height;
        this.Depth = depth;

        this.Cells = new BoardCell[width, height, depth];

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                for (var z = 0; z < depth; z++)
                {
                    this.Cells[x, y, z] = new BoardCell(x, y, z);
                }
            }
        }
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
        BoardCell cell = this.Cells[posX, posY, posZ];
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
                        if (!(x == 0 && y == 0 && z == 0)) { neighbors.Add(this.Cells[x,y,z]);}
                    }
                }
            }
        }

        return neighbors;
    }

    public BoardCell get(int posX, int posY, int posZ)
    {
        return this.Cells[posX, posY, posZ];
    }

    public BoardCell getAt(BoardCell otherCell)
    {
        return this.Cells[otherCell.PosX, otherCell.PosY, otherCell.PosZ];
    }

    public void resetAllCellStates()
    {
        foreach (BoardCell cell in Cells)
        {
            cell.State = CellState.Default;
        }
    }
}
