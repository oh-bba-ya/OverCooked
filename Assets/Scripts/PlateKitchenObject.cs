using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Awake()
    {
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
            kitchenObjectSOList.Add(kitchenObjectSO);

            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
            {
                kitchenObjectSO = kitchenObjectSO
            });
            return true;
        }

    }

    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return kitchenObjectSOList;
    }
}
