using System;
using System.Collections.Generic;
using Assets.Scripts.Data;
using Unity_Tools.Components;
using Unity_Tools.Pooling;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
    public sealed class Revelation
    {
        public bool IsFinished { get; private set; }

        public float RevelationSpeed { get; }

        public event Action Finished;

        private readonly Board board;

        private List<BoardCell> prevCells;

        private List<BoardCell> buffer;

        private readonly List<Action> followupActions;

        private float lastStep;

        public Revelation(Board board, BoardCell cell, float revelationSpeed)
        {
            this.prevCells = GlobalListPool<BoardCell>.Get();
            this.buffer = GlobalListPool<BoardCell>.Get();
            this.followupActions = GlobalListPool<Action>.Get();
            this.RevelationSpeed = revelationSpeed;
            this.board = board;

            cell.State = CellState.Revealed;
            prevCells.Add(cell);
            lastStep = Time.time;

            CallProvider.AddUpdateListener(DoUpdate);
        }

        public void FollowWith(Action action)
        {
            if (IsFinished)
            {
                action.Invoke();
            }
            else
            {
                followupActions.Add(action);
            }
        }

        private void OnFinished()
        {
            Finished?.Invoke();
        }

        private void DoUpdate()
        {
            if (Time.time <= lastStep + RevelationSpeed)
            {
                return;
            }
            
            lastStep += RevelationSpeed;
            
            foreach (var cell in prevCells)
            {
                if (!cell.IsNude || cell.IsBomb)
                {
                    continue;
                }

                foreach (var neighbor in board.NeighborsOf(cell))
                {
                    if (neighbor.State == CellState.Default)
                    {
                        neighbor.State = CellState.Revealed;

                        if (neighbor.IsNude && !neighbor.IsBomb)
                        {
                            buffer.Add(neighbor);
                        }
                    }
                }
            }

            if (buffer.Count == 0)
            {
                IsFinished = true;

                foreach (var action in followupActions)
                {
                    action.Invoke();
                }

                OnFinished();

                GlobalListPool<BoardCell>.Put(prevCells);
                GlobalListPool<BoardCell>.Put(buffer);
                GlobalListPool<Action>.Put(followupActions);

                CallProvider.RemoveUpdateListener(DoUpdate);
                return;
            }

            var tmp = prevCells;
            prevCells = buffer;
            buffer = tmp;
            buffer.Clear();
        }
    }
}
