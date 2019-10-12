using System.ComponentModel;
using Assets.Scripts.Data;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Frontend
{
    public class FieldVisual : MonoBehaviour
    {
        public GameObject TextContainer;

        public GameObject CubeVisual;

        public GameObject OutlineVisual;

        public GameObject BombVisual;

        public Material DefaultFieldMaterial;

        public Material UnknownFieldMaterial;

        public Material SuspectedFieldMaterial;

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
                            cubeRenderer.sharedMaterial = DefaultFieldMaterial;
                            break;
                        case CellState.Suspect:
                            cubeRenderer.sharedMaterial = SuspectedFieldMaterial;
                            break;
                        case CellState.Unknown:
                            cubeRenderer.sharedMaterial = UnknownFieldMaterial;
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

            if (TextContainer != null)
            {
                TextContainer.SetActive(boardCell.State == CellState.Revealed && !boardCell.IsNude);

                var text = boardCell.IsNude ? string.Empty : boardCell.AdjacentBombCount.ToString();
                var textObjects = TextContainer.GetComponentsInChildren<TextMeshPro>();

                foreach (var textObject in textObjects)
                {
                    textObject.text = text;
                }
            }
        }
    }
}
