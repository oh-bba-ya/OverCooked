using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{

    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;   // ��ᰡ �߰� �̺�Ʈ..
    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO kitchenObjectSO;
    }

    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;  // ���ÿ� �ø� �� �ִ� ��� ����Ʈ..

    private List<KitchenObjectSO> kitchenObjectSOList;  // ���� ���ÿ� �����ϴ� ������Ʈ ����Ʈ..

    protected override void Awake()
    {
        base.Awake();
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        // ���ÿ� �ø� �� ���� �����..
        if(!validKitchenObjectSOList.Contains(kitchenObjectSO))
        {
            return false;
        }


        // �̹� ���� ���� �����Ѵٸ�..
        if (kitchenObjectSOList.Contains(kitchenObjectSO)){
            return false;
        }
        else
        {
            AddIngredientServerRpc(
                KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO)
                );
            return true;
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int kitchenObjectSOIndex)
    {
        AddIngredientClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

        kitchenObjectSOList.Add(kitchenObjectSO);

        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
        {
            kitchenObjectSO = kitchenObjectSO
        });
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return kitchenObjectSOList;
    }
}
