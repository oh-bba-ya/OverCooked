using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMultiplayerUI : MonoBehaviour
{

    private void Start()
    {
        OverCookGameManager.Instance.OnMultiplayerGamePaused += OverCookGameManager_OnMultiplayerPaused;
        OverCookGameManager.Instance.OnMultiplayerGameUnPaused += OverCookGameManager_OnMultiplayerUnPaused;

        Hide();
    }

    private void OverCookGameManager_OnMultiplayerUnPaused(object sender, EventArgs e)
    {
        Hide();
    }

    private void OverCookGameManager_OnMultiplayerPaused(object sender, EventArgs e)
    {
        Show();
    }

    void Show()
    {
        gameObject.SetActive(true);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}
