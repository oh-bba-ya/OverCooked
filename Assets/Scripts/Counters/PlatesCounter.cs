using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;


    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;
    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;



    private void Update()
    {
        if(!IsServer) return;

        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnPlateTimerMax)
        {
            spawnPlateTimer = 0f;

            if (platesSpawnedAmount < platesSpawnedAmountMax)
            {
                SpawnPlateServerRpc();
            }
        }
    }

    [ServerRpc]
    private void SpawnPlateServerRpc()
    {
        SpawnPlateClientRpc();
    }

    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        platesSpawnedAmount++;

        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }

    public override void Interact(Player player)
    {
        // �÷��̾ �ƹ� ������Ʈ�� �������� ���� ���¶��..
        if (!player.HasKitchenObject())
        {
            // ���ð� �����Ѵٸ�..
            if(platesSpawnedAmount > 0)
            {
                // �÷��̾� ���� �Ҵ�..
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

                InteractLoginServerRpc();
            }
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
        // ���� �Ѱ� ����..
        platesSpawnedAmount--;

        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }
}
