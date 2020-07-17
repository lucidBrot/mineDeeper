using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Assets.Scripts.Data;
using UnityEngine;

namespace Assets.Scripts.Solver.Rules
{
    public interface IRule
    {
        /// <summary>
        /// Consider this Rule.
        /// Returns True if considering this rule advances the board in any way.
        /// </summary>
        /// <param name="board">A board that shall not be modified</param>
        /// <param name="cell">The target cell to consider.</param>
        /// <param name="mutableConsiderationReportCollection">A collection of recommended modifications to the board that the caller should apply if
        /// the caller wants to have the board modified. This collection can already contain some modifications and if it does,
        /// this Rule Consideration will only add new (possibly duplicate) entries but never remove any.
        /// This Collection must not be null!</param>
        /// <param name="numBombsFound">The number of found bombs during this Rule Consideration. Useful for during this refactoring transition time. Probably no more later...</param>
        /// <returns>
        
        /// </returns>
        bool Consider(in Board board, BoardCell cell, ICollection<ConsiderationReportForCell> mutableConsiderationReportCollection);
    }

    /// <summary>
    /// The IEquatable implementation entails all Coordinates AND the CellState because it is used in two cases:
    /// For Hints, the CellState must be retained when removing Duplicates to detect a problem on the user side
    ///
    /// For Solving, the CellStates for the same coordinates from different considered cells will never contradict.
    /// </summary>
    public readonly struct ConsiderationReportForCell : IEquatable<ConsiderationReportForCell>
    {
        public int PosX { get; }

        public int PosY { get; }

        public int PosZ { get; }
        
        public CellState TargetState { get; }

        public ConsiderationReportForCell(int posX, int posY, int posZ, CellState targetState)
        {
            this.PosX = posX;
            this.PosY = posY;
            this.PosZ = posZ;
            this.TargetState = targetState;
        }

        public ConsiderationReportForCell(BoardCell c, CellState state): this(c.PosX, c.PosY, c.PosZ, state)
        {
            
        }

        public bool Equals(ConsiderationReportForCell other)
        {
            return PosX == other.PosX && PosY == other.PosY && PosZ == other.PosZ && TargetState == other.TargetState;
        }

        public override bool Equals(object obj)
        {
            return obj is ConsiderationReportForCell other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = PosX;
                hashCode = (hashCode * 397) ^ PosY;
                hashCode = (hashCode * 397) ^ PosZ;
                hashCode = (hashCode * 397) ^ (int) TargetState;
                return hashCode;
            }
        }
    }
}