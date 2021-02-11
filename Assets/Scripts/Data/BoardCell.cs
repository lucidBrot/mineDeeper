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
    [Serializable]
    public class BoardCell
    {
        private static int nextFocusOrderNumber = 0;

        [SerializeField]
        private CellState state;
        [SerializeField]
        private bool highlighted;
        [NonSerialized]
        private BoardCell[] neighbors;
        [SerializeField]
        private int posX;
        [SerializeField]
        private int posY;
        [SerializeField]
        private int posZ;
        [SerializeField]
        private int adjacentBombCount;
        [SerializeField]
        private bool isBomb;
        [SerializeField]
        private bool focused;
        [SerializeField]
        private int focusColorId;
        [SerializeField]
        private int focusOrderNumber;

        public int PosX => posX;

        public int PosY => posY;

        public int PosZ => posZ;

        public int AdjacentBombCount
        {
            get => adjacentBombCount;
            set => adjacentBombCount = value;
        }

        public bool IsBomb
        {
            get => isBomb;
            set => isBomb = value;
        }

        /// <summary>
        /// It is naked, has no value and is basically useless for the game.
        /// </summary>
        public bool IsNude => AdjacentBombCount == 0;

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

        public bool Focused
        {
            get => focused;
            private set => focused = value;
        }

        public int FocusColorId
        {
            get => focusColorId;
            private set => focusColorId = value;
        }

        public int FocusOrderNumber
        {
            get => focusOrderNumber;
            private set => focusOrderNumber = value;
        }

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
            this.posX = posX;
            this.posY = posY;
            this.posZ = posZ;

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
        
        public static void SetNextFocusOrderNumberOnSerializeFinished( int num)
        {
            nextFocusOrderNumber = num;
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