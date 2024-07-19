using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{


    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainmenuButton;
    [SerializeField] private Button optionsButton;


    private void Awake()
    {
        resumeButton.onClick.AddListener(() =>
        {
            OverCookGameManager.Instance.TogglePauseGame();
        });

        mainmenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        optionsButton.onClick.AddListener(() =>
        {
            OptionsUI.Instance.Show();
        });
    }

    private void Start()
    {
        OverCookGameManager.Instance.OnGamePaused += OverCookGameManager_OnGamePaused;
        OverCookGameManager.Instance.OnGameUnPaused += OverCookGameManager_OnGameUnPaused;

        Hide();
    }

    private void OverCookGameManager_OnGameUnPaused(object sender, EventArgs e)
    {
        Hide();
    }

    private void OverCookGameManager_OnGamePaused(object sender, EventArgs e)
    {
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
