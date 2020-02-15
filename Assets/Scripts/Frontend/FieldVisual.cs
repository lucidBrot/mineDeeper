using System.ComponentModel;
using Assets.Scripts.Data;
using Assets.Scripts.GameLogic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Frontend
{
    public class FieldVisual : MonoBehaviour
    {
        public TextMeshPro Text;

        public GameObject CubeVisual;

        public GameObject OutlineVisual;

        public GameObject BombVisual;

        public Material DefaultFieldMaterial;

        public Material UnknownFieldMaterial;

        public Material SuspectedFieldMaterial;

        public Material DefaultFieldMaterialHighlighted;
        public Material UnknownFieldMaterialHighlighted;
        public Material SuspectedFieldMaterialHighlighted;

        private BoardCell boardCell;

        public BoardCell BoardCell
        {
            get => boardCell;
            set
            {
                if (boardCell == value)
                {
                    return;
                }

                if (boardCell != null)
                {
                    boardCell.PropertyChanged -= OnBoardCellPropertyChanged;
                }

                boardCell = value;

                if (boardCell != null)
                {
                    boardCell.PropertyChanged += OnBoardCellPropertyChanged;
                    UpdateVisual();
                }
            }
        }

        private float defaultFontSize;

        private void OnEnable()
        {
            if (Text != null)
            {
                defaultFontSize = Text.fontSize;
            }
        }

        private void OnBoardCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (boardCell == null)
            {
                return;
            }

            if (CubeVisual != null) 
            {
                if (boardCell.State == CellState.Revealed)
                {
                    CubeVisual.SetActive(false);
                }
                else
                {
                    CubeVisual.SetActive(true);
                    var cubeRenderer = CubeVisual.GetComponent<Renderer>();
                    
                    switch (boardCell.State)
                    {
                        case CellState.Default:
                            cubeRenderer.sharedMaterial = boardCell.Highlighted ? DefaultFieldMaterialHighlighted : DefaultFieldMaterial;
                            break;
                        case CellState.Suspect:
                            cubeRenderer.sharedMaterial = boardCell.Highlighted ? SuspectedFieldMaterialHighlighted : SuspectedFieldMaterial;
                            break;
                        case CellState.Unknown:
                            cubeRenderer.sharedMaterial = boardCell.Highlighted? UnknownFieldMaterialHighlighted : UnknownFieldMaterial;
                            break;
                    }

                }
            }

            if (BombVisual != null)
            {
                BombVisual.SetActive(boardCell.IsBomb && boardCell.State == CellState.Revealed);
            }

            if (OutlineVisual != null)
            {
                OutlineVisual.SetActive(boardCell.State == CellState.Revealed && !boardCell.IsNude);
            }

            if (Text != null)
            {
                Text.gameObject.SetActive(boardCell.State == CellState.Revealed && !boardCell.IsNude);

                var text = boardCell.IsNude ? string.Empty : boardCell.AdjacentBombCount.ToString();

                Text.text = text;

                if (!boardCell.IsNude)
                {
                    var colors = ColorProvider.Instance.NumberColors;
                    var index = boardCell.AdjacentBombCount - 1;
                    this.Text.color = index < colors.Length ? colors[index] : colors[colors.Length - 1];
                }

                if (boardCell.Highlighted)
                {
                    Text.color = Color.yellow;
                    Text.fontSize = 1.5f * defaultFontSize;
                }
                else
                {
                    Text.fontSize = defaultFontSize;
                }
            }
        }

    }
}
