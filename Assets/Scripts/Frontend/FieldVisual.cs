using System.ComponentModel;
using System.Linq;
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

                this.boardCell = value;
                this.UpdateVisual();
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

        private void Update()
        {
            if (this.boardCell.VisualStateChangedFlag)
            {
                this.boardCell.VisualStateChangedFlag = false;
                this.UpdateVisual();
            }
        }

        private void OnStyleChanged()
        {
            this.UpdateHullColor();
        }

        public void UpdateHullColor()
        {
            if (this.cubeRenderer == null)
            {
                return;
            }

            Color color;

            if (this.boardCell != null && this.boardCell.Highlighted)
            {
                color = Color.yellow;
            }
            else if (this.boardCell != null && this.boardCell.Focused)
            {
                color = Data.StyleManager.GetFocusColor(this.boardCell.FocusColorId);
            }
            else
            {
                color = Data.StyleManager.GetCubeColor(this.transform.position);
            }

            this.cubeRenderer.GetPropertyBlock(this.matPropBlock);
            this.matPropBlock.SetColor("_Color", color);
            this.cubeRenderer.SetPropertyBlock(this.matPropBlock);
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

                    this.UpdateHullColor();
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
                    if (!this.boardCell.Highlighted)
                    {
                        if (this.GetNeighborFocusColor(out var neighborFocusColor))
                        {
                            this.Text.color = neighborFocusColor;
                        }
                        else
                        {
                            var colors = Data.StyleManager.Instance.NumberColors;
                            var index = this.boardCell.AdjacentBombCount - 1;
                            this.Text.color = index < colors.Length ? colors[index] : colors[colors.Length - 1];
                        }

                        this.Text.fontSize = this.defaultFontSize;
                    }
                    else
                    {
                        this.Text.color = Color.yellow;
                        this.Text.fontSize = 1.5f * this.defaultFontSize;
                    }
                }
            }
        }

        private bool GetNeighborFocusColor(out Color color)
        {
            var maxColorId = -1;
            var maxOrder = -1;

            foreach (var neighbor in this.boardCell.Neighbors)
            {
                if (neighbor.Focused && neighbor.State != CellState.Revealed && neighbor.FocusOrderNumber > maxOrder)
                {
                    maxOrder = neighbor.FocusOrderNumber;
                    maxColorId = neighbor.FocusColorId;
                }
            }

            if (maxOrder >= 0)
            {
                color = Data.StyleManager.GetFocusColor(maxColorId);
                return true;
            }

            color = Color.black;
            return false;
        }
    }
}
