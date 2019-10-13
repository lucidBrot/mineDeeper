using System.Collections.Generic;
using System.ComponentModel;
using Assets.Scripts.Data;
using Assets.Scripts.Frontend;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        private readonly List<FieldVisual> fieldVisuals = new List<FieldVisual>();

        public FieldVisual Prefab;

        public Vector3 FieldSize;

        public Vector3 Margin;

        public GameManager()
        {
            FieldSize = new Vector3(2, 2, 2);
            Margin = new Vector3(0.1f, 0.1f, 0.1f);
        }

        // Start is called before the first frame update
        void Start()
        {
            Game.Instance.PropertyChanged += OnGameboardChanged;
            Game.Instance.StartNewGame();
        }

        private void OnGameboardChanged(object sender, PropertyChangedEventArgs e) 
        {
            if (e.PropertyName != nameof(Game.GameBoard))
            {
                return;
            }

            DestroyGameField();
            BuildGameField();
        }

        private void DestroyGameField()
        {
            foreach (var fieldVisual in fieldVisuals)
            {
                Destroy(fieldVisual.gameObject);
            }

            fieldVisuals.Clear();
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
            var d = board.Height;

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
                        fieldVisuals.Add(instance);
                    }
                }
            }
        }
    }
}
