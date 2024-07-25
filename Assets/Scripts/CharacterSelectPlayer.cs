using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectPlayer : MonoBehaviour
{

    [SerializeField] private int playerIndex;

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;

        UpdatePlayer();
    }

    /// <summary>
    /// �÷��̾ �κ� �����Ҷ����� ȣ��Ǵ� �̺�Ʈ..
    /// </summary>
    private void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        // ���� ������ �ε��� ��ȣ�� ����� �÷��̾��� ������ ������
        if(KitchenGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            // ĳ���� ������ Ȱ��ȭ..
            Show();
        }
        else
        {
            // ��Ȱ��ȭ..
            Hide();
        }
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
