using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity_Tools.Core;
using UnityEngine;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGameboardChanged(object sender, PropertyChangedEventArgs e)
    {
        DestroyGameField();
        BuildGameField();
    }

    private void DestroyGameField()
    {
        foreach (var fieldVisual in fieldVisuals)
        {
            Destroy(fieldVisual.gameObject);
        }
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
                    var cell = board.Cells[x, y, z];

                    var instance = Instantiate(Prefab);
                    instance.BoardCell = cell;
                }
            }
        }
    }
}
