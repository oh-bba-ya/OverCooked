using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{

    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;  // 접시에 올릴 수 있는 재료 리스트..

    private List<KitchenObjectSO> kitchenObjectSOList;

    private void Awake()
    {
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        // 접시에 올릴 수 없는 재료라면..
        if(!validKitchenObjectSOList.Contains(kitchenObjectSO))
        {
            return false;
        }


        // 이미 접시 위에 존재한다면..
        if (kitchenObjectSOList.Contains(kitchenObjectSO)){
            return false;
        }
        else
        {
            kitchenObjectSOList.Add(kitchenObjectSO);
            return true;
        }

    }
}
