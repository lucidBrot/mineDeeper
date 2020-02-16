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

        public Color Color;

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

            if (ColorProvider.CanAccessInstance)
            {
                ColorProvider.Instance.StyleChanged += OnStyleChanged;
            }
        }

        private void OnDisable()
        {
            if (ColorProvider.CanAccessInstance)
            {
                ColorProvider.Instance.StyleChanged -= OnStyleChanged;
            }
        }

        private void OnStyleChanged()
        {
            UpdateColor();
        }

        public void UpdateColor()
        {
            if (CubeVisual != null)
            {
                var cubeRenderer = CubeVisual.GetComponent<Renderer>();
                if (cubeRenderer != null)
                {
                    var propBlock = new MaterialPropertyBlock();
                    cubeRenderer.GetPropertyBlock(propBlock);
                    var color = ColorProvider.GetCubeColor(this.transform.position);
                    propBlock.SetColor("_BaseColor", color * this.Color);
                    cubeRenderer.SetPropertyBlock(propBlock);
                }
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
                OutlineVisual.SetActive(boardCell.State == CellState.Revealed && !boardCell.IsNude && !boardCell.IsBomb);
            }

            if (Text != null)
            {
                Text.gameObject.SetActive(boardCell.State == CellState.Revealed && !boardCell.IsNude && !boardCell.IsBomb);

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
