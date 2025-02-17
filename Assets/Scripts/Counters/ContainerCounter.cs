using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectso;
    public event EventHandler OnPlayerGrabbedObject;

    public override void Interact(Player player)
    {
        // 플레이어가 오브젝트를 소유하고 있지 않다면..
        if (!player.HasKitchenObject())
        {
            KitchenObject.SpawnKitchenObject(kitchenObjectso, player);

            InteractLoginServerRpc();
        }


    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLoginServerRpc()
    {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
    }
}
