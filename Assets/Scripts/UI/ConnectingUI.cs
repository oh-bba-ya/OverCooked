using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnTryingToJoingGame += KitchenGameMultiplayer_OnTryingToJoingGame;
        KitchenGameMultiplayer.Instance.OnFailedToJoingGame += KitchenGameMultiplayer_OnFailedToJoingGame;
        Hide();
    }

    private void KitchenGameMultiplayer_OnFailedToJoingGame(object sender, EventArgs e)
    {
        Hide();
    }

    private void KitchenGameMultiplayer_OnTryingToJoingGame(object sender, EventArgs e)
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


    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnTryingToJoingGame -= KitchenGameMultiplayer_OnTryingToJoingGame;
        KitchenGameMultiplayer.Instance.OnFailedToJoingGame -= KitchenGameMultiplayer_OnFailedToJoingGame;
    }
}
