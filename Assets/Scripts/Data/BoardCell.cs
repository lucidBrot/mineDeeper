using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class BoardCell
    {
        private static int nextFocusOrderNumber = 0;

        private CellState state;
        private bool highlighted;
        private BoardCell[] neighbors;

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
        public bool IsNude => this.AdjacentBombCount == 0;

        public bool Highlighted
        {
            get => highlighted;
            set
            {
                if (value == highlighted) return;
                highlighted = value;
                this.VisualStateChangedFlag = true;
            }
        }

        public bool Focused { get; private set; }

        public int FocusColorId { get; private set; }

        public int FocusOrderNumber { get; private set; }

        public CellState State
        {
            get => state;
            set
            {
                if (value == state)
                {
                    return;
                }

                state = value;
                this.VisualStateChangedFlag = true;

                if (this.Focused)
                {
                    this.SetFocus(-1);
                }
            }
        }

        public bool VisualStateChangedFlag { get; set; }

        public BoardCell[] Neighbors
        {
            get => this.neighbors;
            set => this.neighbors = value ?? throw new ArgumentNullException(nameof(value));
        }

        public BoardCell(int posX, int posY, int posZ)
        {
            this.PosX = posX;
            this.PosY = posY;
            this.PosZ = posZ;

            AdjacentBombCount = 0;
            IsBomb = false;
            State = CellState.Default;
            this.neighbors = Array.Empty<BoardCell>();
        }

        public void SetFocus(int focusColorId)
        {
            if (focusColorId < 0)
            {
                if (!this.Focused)
                {
                    return;
                }

                this.Focused = false;
                this.FocusColorId = -1;
                this.FocusOrderNumber = -1;
            }
            else
            {
                this.Focused = true;
                this.FocusColorId = focusColorId;
                this.FocusOrderNumber = nextFocusOrderNumber++;
            }
            
            foreach (var neighbor in this.neighbors)
            {
                neighbor.VisualStateChangedFlag = true;
            }

            this.VisualStateChangedFlag = true;
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

        public override string ToString()
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