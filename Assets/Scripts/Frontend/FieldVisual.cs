using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class FieldVisual : MonoBehaviour
{
    public GameObject TextContainer;

    public GameObject FlagContainer;

    public GameObject CubeVisual;

    public GameObject OutlineVisual;

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

        if (FlagContainer != null)
        {
            FlagContainer.SetActive(!boardCell.IsRevealed && boardCell.IsSuspect);
        }

        if (CubeVisual != null)
        {
            CubeVisual.SetActive(!boardCell.IsRevealed);
        }

        if (OutlineVisual != null)
        {
            OutlineVisual.SetActive(boardCell.IsRevealed && !boardCell.IsNude);
        }

        if (TextContainer != null)
        {
            TextContainer.SetActive(boardCell.IsRevealed && !boardCell.IsNude);

            var text = boardCell.IsNude ? string.Empty : boardCell.AdjacentBombCount.ToString();
            var textObjects = TextContainer.GetComponentsInChildren<TextMeshPro>();

            foreach (var textObject in textObjects)
            {
                textObject.text = text;
            }
        }
    }
}
