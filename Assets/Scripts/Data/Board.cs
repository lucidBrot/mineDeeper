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
}
