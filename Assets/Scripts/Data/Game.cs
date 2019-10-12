using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

public class Game : MonoBehaviour, INotifyPropertyChanged
{
    private static Game instance;
    private Board gameBoard;

    public static Game Instance
    {
        get
        {
            if (instance == null)
            {
                var go = GameObject.Find("Singleton Container");
                if (go == null)
                {
                    go = new GameObject("Singleton Container");
                }

                instance = go.AddComponent<Game>();
            }

            return instance;
        }
    }

    public Board GameBoard
    {
        get => gameBoard;
        set
        {
            if (Equals(value, gameBoard)) return;
            gameBoard = value;
            OnPropertyChanged();
        }
    }

    private void OnEnable()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("Singleton instance of Game already exists, removing new instance.");
            Destroy(this);
            return;
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
