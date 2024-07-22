using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }


    [SerializeField] private Button soundsEffectButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;

    [SerializeField] private TextMeshProUGUI soundEffectsText;
    [SerializeField] private TextMeshProUGUI musicText;

    private void Awake()
    {
        Instance = this;

        soundsEffectButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        });

        musicButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeVolume(); 
            UpdateVisual();
        });

        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    private void Start()
    {
        OverCookGameManager.Instance.OnGameUnPaused += OverCookGameManager_OnGameUnPaused;

        UpdateVisual();

        Hide();
    }

    private void OverCookGameManager_OnGameUnPaused(object sender, EventArgs e)
    {
        Hide();
    }

    void UpdateVisual()
    {
        soundEffectsText.text = "Sound Effects : " +  Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
        musicText.text = "Music : " +  Mathf.Round(MusicManager.Instance.GetVolume() * 10f);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
