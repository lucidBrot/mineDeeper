using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCell
{
    /// <summary>
    /// My summary.
    /// </summary>
    public int PosX { get; }

    public int PosY { get; }

    public int PosZ { get; }

    public int AdjacentBombCount { get; set; }

    public bool IsBomb { get; set; }

    public bool IsNude => AdjacentBombCount == 0;

    public bool IsRevealed { get; set; }

    public bool IsSuspect { get; set; }

    public BoardCell(int posX, int posY, int posZ)
    {
        this.PosX = posX;
        this.PosY = posY;
        this.PosZ = posZ;
    }
}
