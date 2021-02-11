using System;
using System.Collections.Generic;
using System.ComponentModel;
using Assets.Scripts.Data;
using Assets.Scripts.Frontend;
using Assets.Scripts.GameLogic.EffectTimeline;
using Unity_Tools.Components;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
    public class BoardVisuals : SingletonBehaviour<BoardVisuals>
    {
        private readonly List<FieldVisual> fieldVisuals = new List<FieldVisual>();

        public FieldVisual Prefab;

        public Vector3 FieldSize;

        public Vector3 Margin;

        private CreationState state;

        private bool needsCreation;

        public BoardVisuals()
        {
            FieldSize = new Vector3(2, 2, 2);
            Margin = new Vector3(0.1f, 0.1f, 0.1f);
        }

        // Start is called before the first frame update
        void Start()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            this.state = CreationState.Deleted;
            Game.Instance.NewGameStarting += OnNewGameStarting;
            Game.Instance.NewGameStarted += OnNewGameStarted;
            Game.Instance.StartNewGame();
        }

        public static Vector3 BoardToWorldPosition(int x, int y, int z)
        {
            if (!CanAccessInstance || !Game.CanAccessInstance)
            {
                return Vector3.zero;
            }
            
            var board = Game.Instance.GameBoard;

            var w = board.Width;
            var h = board.Height;
            var d = board.Depth;

            var fieldMarginSize = Instance.FieldSize + Instance.Margin;
            var worldSize = new Vector3(w * fieldMarginSize.x, h * fieldMarginSize.y, d * fieldMarginSize.z);
            var startPoint = -worldSize / 2f;

            return startPoint + new Vector3(x * fieldMarginSize.x, y * fieldMarginSize.y,
                       z * fieldMarginSize.z);
        }

        public static Vector3 BoardToWorldPosition(BoardCell cell)
        {
            return BoardToWorldPosition(cell.PosX, cell.PosY, cell.PosZ);
        }

        private void OnNewGameStarting(object sender, EventArgs e)
        {
            if (this.state == CreationState.Creating || this.state == CreationState.Created)
            {
                BoardEffectOrchestrator.AbortEffects();
                this.DestroyGameFieldAnimated();
            }

            this.needsCreation = false;
        }

        private void OnNewGameStarted(object sender, EventArgs e) 
        {
            if (this.state == CreationState.Deleted)
            {
                this.BuildGameFieldAnimated();
            }
            else if(this.state == CreationState.Deleting) 
            {
                this.needsCreation = true;
            }
            else if (this.state == CreationState.Created)
            {
                this.DestroyGameFieldAnimated();
                this.needsCreation = true;
            }
        }

        private void DestroyGameField()
        {
            foreach (var fieldVisual in fieldVisuals)
            {
                Destroy(fieldVisual.gameObject);
            }

            fieldVisuals.Clear();
        }

        private void DestroyGameFieldAnimated()
        {
            this.state = CreationState.Deleting;

            if (this.fieldVisuals.Count == 0)
            {
                this.OnVisualsDeleted();
                return;
            }

            var startPoint = this.fieldVisuals[0] != null ? this.fieldVisuals[0].transform.position : Vector3.zero;

            BoardEffectOrchestrator.PlayEffect(this.fieldVisuals,
                new SphericalGrowthTimeline(startPoint, 30f),
                item => ((FieldVisual) item).transform.position,
                item => Destroy(((FieldVisual) item).gameObject),
                this.OnVisualsDeleted);
        }

        private void BuildGameField()
        {
            var board = Game.Instance.GameBoard;

            if (board == null)
            {
                return;
            }

            if (fieldVisuals.Capacity < board.Cells.Length)
            {
                fieldVisuals.Capacity = board.Cells.Length;
            }

            var w = board.Width;
            var h = board.Height;
            var d = board.Depth;

            var fieldMarginSize = FieldSize + Margin;
            var worldSize = new Vector3(w * fieldMarginSize.x, h * fieldMarginSize.y, d * fieldMarginSize.z);
            var startPoint = -worldSize / 2f;

            for(var x = 0; x < w; x++)
            {
                for (var y = 0; y < h; y++)
                {
                    for (var z = 0; z < d; z++)
                    {
                        var cell = board[x, y, z];

                        var instance = Instantiate(Prefab);
                        instance.BoardCell = cell;
                        instance.transform.position = startPoint + new Vector3(x * fieldMarginSize.x, y * fieldMarginSize.y,
                                                          z * fieldMarginSize.z);
                        instance.UpdateHullColor();
                        fieldVisuals.Add(instance);
                    }
                }
            }
        }

        private void BuildGameFieldAnimated()
        {
            this.state = CreationState.Creating;
            var board = Game.Instance.GameBoard;

            if (board == null)
            {
                return;
            }

            if (fieldVisuals.Capacity < board.Cells.Length)
            {
                fieldVisuals.Capacity = board.Cells.Length;
            }

            var w = board.Width;
            var h = board.Height;
            var d = board.Depth;

            var fieldMarginSize = FieldSize + Margin; 
            var worldSize = new Vector3(w * fieldMarginSize.x, h * fieldMarginSize.y, d * fieldMarginSize.z);
            var startPoint = -worldSize / 2f;

            BoardEffectOrchestrator.PlayEffect(board.Cells, new SphericalGrowthTimeline(startPoint, 30f),
                item => BoardVisuals.BoardToWorldPosition((BoardCell) item), item =>
                {
                    var cell = (BoardCell) item;
                var instance = Instantiate(Prefab);
                instance.BoardCell = cell;
                instance.transform.position = startPoint + new Vector3(cell.PosX * fieldMarginSize.x, cell.PosY * fieldMarginSize.y,
                    cell.PosZ * fieldMarginSize.z);
                instance.UpdateHullColor();
                fieldVisuals.Add(instance);
            }, this.OnVisualsCreated);
        }

        private void OnVisualsCreated()
        {
            this.state = CreationState.Created;
        }

        private void OnVisualsDeleted()
        {
            this.fieldVisuals.Clear();
            this.state = CreationState.Deleted;

            if (this.needsCreation)
            {
                this.needsCreation = false;
                this.BuildGameFieldAnimated();
            }
        }

        private enum CreationState
        {
            Creating,

            Created,

            Deleting,

            Deleted
        }
    }
}
