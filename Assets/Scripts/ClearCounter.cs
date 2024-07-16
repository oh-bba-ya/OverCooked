using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : MonoBehaviour,IKitchenObjectParent
{
    [SerializeField] private KitchenObjectSO kitchenObjectso;
    [SerializeField] private Transform counterTopPoint;


    private KitchenObject kitchenObject;


    public void Interact(Player player)
    {
        if(kitchenObject == null)
        {
            Transform kitchenTransform = Instantiate(kitchenObjectso.prefab, counterTopPoint);
            kitchenTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(this);

        }
        else  // �̹� ������Ʈ�� �����Ѵٸ�..
        {
            // �÷��̾� �ݱ�..
            kitchenObject.SetKitchenObjectParent(player);
        }



    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
