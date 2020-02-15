using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Assets.Scripts;
using Assets.Scripts.Data;
using TMPro;
using Unity_Tools.Components;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class UILayer : SingletonBehaviour<UILayer>
{
    public TextMeshProUGUI BombLeftCounter;

    public TextMeshProUGUI BombsExplodedCounter;

    public TextMeshProUGUI HintsRequestCounter;

    public TextMeshProUGUI BombsLeftCounter;

    public TextMeshProUGUI HintDrawer;

    public KeyCode HintKey;

    public KeyCode RestartKey;

    private string hintText;

    private PlayerStats prevPlayerStats;

    private Board prevBoard;

    public GameObject PanelBehindHintText;

    public string HintText
    {
        get => hintText;
        set
        {
            hintText = value;

            if (HintDrawer != null)
            {
                HintDrawer.text = hintText;
            }

            PanelBehindHintText.SetActive(!string.IsNullOrWhiteSpace(hintText));
        }
    }

    void Start()
    {
        Game.Instance.PropertyChanged += GamePropertyChanged;

        if (PanelBehindHintText != null && string.IsNullOrWhiteSpace(hintText))
        {
            PanelBehindHintText.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(HintKey))
        {
            ShowHint();
        }

        if (Input.GetKeyUp(RestartKey))
        {
            RestartGame();
        }
    }

    private void GamePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Game.PlayerStats):
            {
                if (prevPlayerStats != null)
                {
                    prevPlayerStats.PropertyChanged -= PlayerStatsOnPropertyChanged;
                }

                prevPlayerStats = Game.Instance.PlayerStats;

                if (prevPlayerStats != null)
                {
                    prevPlayerStats.PropertyChanged += PlayerStatsOnPropertyChanged;
                    UpdatePlayerStats();
                    UpdateBoardStats();
                }

                break;
            }
            case nameof(Game.GameBoard):
            {
                if (prevBoard != null)
                {
                    prevBoard.PropertyChanged -= BoardOnPropertyChanged;
                }

                prevBoard = Game.Instance.GameBoard;

                if (prevBoard != null)
                {
                    prevBoard.PropertyChanged += BoardOnPropertyChanged;
                    UpdateBoardStats();
                }

                break;
            }
        }
    }

    private void BoardOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        UpdateBoardStats();
    }

    private void PlayerStatsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        UpdatePlayerStats();
        UpdateBoardStats();
    }

    public void ShowHint()
    {
        Game.Instance.RequestHint();
    }

    public void RestartGame()
    {
        Game.Instance.StartNewGame();
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void UpdatePlayerStats()
    {
        if (BombsExplodedCounter != null)
        {
            BombsExplodedCounter.text = prevPlayerStats.NumBombsExploded.ToString();
        }

        if (HintsRequestCounter != null)
        {
            HintsRequestCounter.text = prevPlayerStats.NumHintsRequested.ToString();
        }
    }

    private void UpdateBoardStats()
    {
        if (BombsLeftCounter == null)
        {
            return;
        }

        if (prevBoard == null || prevPlayerStats == null)
        {
            BombsLeftCounter.text = "...";
            return;
        }

        BombsLeftCounter.text =
            (prevBoard.BombCount - prevBoard.FlagCount - prevPlayerStats.NumBombsExploded).ToString();
    }
}
