using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainmenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJointButton;


    private void Awake()
    {
        mainmenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        createLobbyButton.onClick.AddListener(() =>
        {
            OverCookGameLobby.Instance.CreateLobby("LobbyName",false);
        });


        quickJointButton.onClick.AddListener(() =>
        {
            OverCookGameLobby.Instance.QuickJoin();
        });
    }
}
