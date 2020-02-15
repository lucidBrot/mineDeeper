using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Assets.Scripts;
using Assets.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class UILayer : SingletonBehaviour<UILayer>
{
    public TextMeshProUGUI BombLeftCounter;

    public TextMeshProUGUI BombsExplodedCounter;

    public TextMeshProUGUI HintsRequestCounter;

    public TextMeshProUGUI HintDrawer;

    public KeyCode HintKey;

    public KeyCode RestartKey;

    private string hintText;

    private PlayerStats playerStats;

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
        Game.Instance.PlayerStatsChanged += GamePropertyChanged;

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
        if (e.PropertyName != nameof(Game.PlayerStats))
        {
            return;
        }

        if (playerStats != null)
        {
            playerStats.PropertyChanged -= PlayerStatsOnPropertyChanged;
        }

        playerStats = Game.Instance.PlayerStats;

        if (playerStats != null)
        {
            playerStats.PropertyChanged += PlayerStatsOnPropertyChanged;
            UpdateStats();
        }
    }

    private void PlayerStatsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        UpdateStats();
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

    private void UpdateStats()
    {
        if (BombsExplodedCounter != null)
        {
            BombsExplodedCounter.text = playerStats.NumBombsExploded.ToString();
        }

        if (HintsRequestCounter != null)
        {
            HintsRequestCounter.text = playerStats.NumHintsRequested.ToString();
        }
    }
}
