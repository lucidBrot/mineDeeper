using System.ComponentModel;
using System.Threading.Tasks;
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
        
        /*public Material DefaultFieldMaterialHighlighted;

        public Material UnknownFieldMaterialHighlighted;

        public Material SuspectedFieldMaterialHighlighted;*/

        private BoardCell boardCell;

        public BoardCell BoardCell
        {
            get => this.boardCell;
            set
            {
                if (this.boardCell == value)
                {
                    return;
                }

                if (this.boardCell != null)
                {
                    this.boardCell.PropertyChanged -= this.OnBoardCellPropertyChanged;
                }

                this.boardCell = value;

                if (this.boardCell != null)
                {
                    this.boardCell.PropertyChanged += this.OnBoardCellPropertyChanged;
                    this.UpdateVisual();
                }
            }
        }

        private float defaultFontSize;

        private Renderer cubeRenderer;

        private MaterialPropertyBlock matPropBlock;

        private void OnEnable()
        {
            if (this.Text != null)
            {
                this.defaultFontSize = this.Text.fontSize;
            }

            if (Data.StyleManager.CanAccessInstance)
            {
                Data.StyleManager.Instance.StyleChanged += this.OnStyleChanged;
            }

            this.cubeRenderer = this.CubeVisual != null ? this.CubeVisual.GetComponent<Renderer>() : null;
            this.matPropBlock = new MaterialPropertyBlock();
        }

        private void OnDisable()
        {
            if (Data.StyleManager.CanAccessInstance)
            {
                Data.StyleManager.Instance.StyleChanged -= this.OnStyleChanged;
            }
        }

        private void OnStyleChanged()
        {
            this.UpdateColor();
        }

        public void UpdateColor()
        {
            if (this.cubeRenderer == null)
            {
                return;
            }

            var color = this.GetActiveColor();

            this.cubeRenderer.GetPropertyBlock(this.matPropBlock);
            this.matPropBlock.SetColor("_Color", color);
            this.cubeRenderer.SetPropertyBlock(this.matPropBlock);

        }

        private void OnBoardCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (this.boardCell == null)
            {
                return;
            }

            if (this.CubeVisual != null) 
            {
                if (this.boardCell.State == CellState.Revealed)
                {
                    this.CubeVisual.SetActive(false);
                }
                else
                {
                    this.CubeVisual.SetActive(true);

                    if (this.cubeRenderer != null)
                    {
                        switch (this.boardCell.State)
                        {
                            case CellState.Default:
                                this.cubeRenderer.sharedMaterial = this.DefaultFieldMaterial;
                                break;
                            case CellState.Suspect:
                                this.cubeRenderer.sharedMaterial = this.SuspectedFieldMaterial;
                                break;
                            case CellState.Unknown:
                                this.cubeRenderer.sharedMaterial = this.UnknownFieldMaterial;
                                break;
                        }
                    }

                    this.UpdateColor();
                }
            }
            
            if (this.BombVisual != null)
            {
                this.BombVisual.SetActive(this.boardCell.IsBomb && this.boardCell.State == CellState.Revealed);
            }

            if (this.OutlineVisual != null)
            {
                this.OutlineVisual.SetActive(this.boardCell.State == CellState.Revealed && !this.boardCell.IsNude && !this.boardCell.IsBomb);
            }

            if (this.Text != null)
            {
                this.Text.gameObject.SetActive(this.boardCell.State == CellState.Revealed && !this.boardCell.IsNude && !this.boardCell.IsBomb);

                var text = this.boardCell.IsNude ? string.Empty : this.boardCell.AdjacentBombCount.ToString();

                this.Text.text = text;

                if (!this.boardCell.IsNude)
                {
                    if (this.boardCell.Focused)
                    {
                        this.Text.color = Data.StyleManager.GetFocusColor(this.boardCell.FocusId);
                    }
                    else
                    {
                        var colors = Data.StyleManager.Instance.NumberColors;
                        var index = this.boardCell.AdjacentBombCount - 1;
                        this.Text.color = index < colors.Length ? colors[index] : colors[colors.Length - 1];
                    }
                }

                if (this.boardCell.Highlighted)
                {
                    this.Text.color = Color.yellow;
                    this.Text.fontSize = 1.5f * this.defaultFontSize;
                }
                else
                {
                    this.Text.fontSize = this.defaultFontSize;
                }
            }
        }

        private Color GetActiveColor()
        {
            if (this.boardCell != null && this.boardCell.Highlighted)
            {
                return Color.yellow;
            }

            if (this.boardCell != null && this.boardCell.Focused)
            {
                return Data.StyleManager.GetFocusColor(this.boardCell.FocusId);
            }

            return Data.StyleManager.GetCubeColor(this.transform.position);
        }
    }
}
