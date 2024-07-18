using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{


    public override void Interact(Player player)
    {
        // �÷��̾ ������Ʈ�� �����ϰ� �ִٸ�..
        if (player.HasKitchenObject())
        {
            // �÷��̾ ������ ������Ʈ�� ���ö��..
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);

                player.GetKitchenObject().DestroySelf();
            }
        }
    }
}
