using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

public class BoardCell : INotifyPropertyChanged
{
    private bool isRevealed;
    private bool isSuspect;

    /// <summary>
    /// My summary.
    /// </summary>
    public int PosX { get; }

    public int PosY { get; }

    public int PosZ { get; }

    public int AdjacentBombCount { get; set; }

    public bool IsBomb { get; set; }

    /// <summary>
    /// It is naked, has no value and is basically useless for the game.
    /// </summary>
    public bool IsNude => AdjacentBombCount == 0;

    public bool IsRevealed
    {
        get => isRevealed;
        set
        {
            if (value == isRevealed) return;
            isRevealed = value;
            OnPropertyChanged();
        }
    }

    public bool IsSuspect
    {
        get => isSuspect;
        set
        {
            if (value == isSuspect) return;
            isSuspect = value;
            OnPropertyChanged();
        }
    }

    public BoardCell(int posX, int posY, int posZ)
    {
        this.PosX = posX;
        this.PosY = posY;
        this.PosZ = posZ;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
