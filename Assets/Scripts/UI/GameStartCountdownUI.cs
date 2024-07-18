using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;

    private void Start()
    {
        OverCookGameManager.Instance.OnStateChanged += OverCookGameManager_OnStateChanged;

        Hide();
    }

    private void OverCookGameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (OverCookGameManager.Instance.IsCountdownToStartActive())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Update()
    {
        countdownText.text = OverCookGameManager.Instance.GetCountdownToStartTimer().ToString("#.##");
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
