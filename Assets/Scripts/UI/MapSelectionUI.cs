using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectionUI : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Image mapSelectionImage;


    public List<Sprite> mapSelectionThumbnail;

    private int mapSelectionIndex;

    private void Awake()
    {
        mapSelectionIndex = 0;
        // 서버 일 경우에만 버튼활성화..
        nextButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        mapSelectionImage.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        nextButton.onClick.AddListener(OnNextButton);
    }

    private void OnNextButton()
    {
        mapSelectionIndex = mapSelectionIndex == 0 ? 1 : 0;
        NextButtonServerRpc();
    }

    [ServerRpc]
    private void NextButtonServerRpc()
    {
        if (mapSelectionIndex == 0)
        {
            CharacterSelectReady.Instance.selectionScene = CharacterSelectReady.GameScene.GameScene;
        }
        else
        {
            CharacterSelectReady.Instance.selectionScene = CharacterSelectReady.GameScene.GameTwoScene;
        }


        mapSelectionImage.sprite = mapSelectionThumbnail[mapSelectionIndex];
    }




}
