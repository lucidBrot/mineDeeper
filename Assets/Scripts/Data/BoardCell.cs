﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class BoardCell : INotifyPropertyChanged
    {
        private CellState state;

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
    
        public CellState State
        {
            get => state;
            set
            {
                if (value == state) return;
                state = value;
                OnPropertyChanged();
            }
        }

        public BoardCell(int posX, int posY, int posZ)
        {
            this.PosX = posX;
            this.PosY = posY;
            this.PosZ = posZ;

            AdjacentBombCount = 0;
            IsBomb = false;
            State = CellState.Default;
        }

        public void ToggleMarking()
        {
            switch (State)
            {
                case CellState.Default:
                    this.State = CellState.Suspect;
                    break;
                case CellState.Revealed:
                    break;
                case CellState.Suspect:
                    this.State = CellState.Unknown;
                    break;
                case CellState.Unknown:
                    this.State = CellState.Default;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public String ToString()
        {
            StringBuilder b = new StringBuilder("Cell(");
            return b.Append(PosX).Append(", ").Append(PosY).Append(", ").Append(PosZ).Append(")").ToString();
        }
    }

    public enum CellState
    {
        /// <summary>
        /// Unrevealed, not suspected, not unknown.
        /// </summary>
        Default,

        /// <summary>
        /// Revealed, not suspected, not unknown.
        /// </summary>
        Revealed,

        /// <summary>
        /// Unrevealed, suspected, not unknown.
        /// </summary>
        Suspect,

        /// <summary>
        /// Unrevealed, not suspected, unknown.
        /// </summary>
        Unknown
    }
}