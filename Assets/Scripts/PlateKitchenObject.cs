using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{

    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;  // ���ÿ� �ø� �� �ִ� ��� ����Ʈ..

    private List<KitchenObjectSO> kitchenObjectSOList;

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
            return true;
        }

    }
}
