using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainmenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJointButton;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;


    private void Awake()
    {
        mainmenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        createLobbyButton.onClick.AddListener(() =>
        {
            lobbyCreateUI.Show();
        });


        quickJointButton.onClick.AddListener(() =>
        {
            OverCookGameLobby.Instance.QuickJoin();
        });

        joinCodeButton.onClick.AddListener(() =>
        {
            OverCookGameLobby.Instance.JoinWithCode(joinCodeInputField.text);
        });
    }
}
