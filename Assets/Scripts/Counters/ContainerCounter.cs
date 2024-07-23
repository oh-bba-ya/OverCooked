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
        // �÷��̾ ������Ʈ�� �����ϰ� ���� �ʴٸ�..
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
