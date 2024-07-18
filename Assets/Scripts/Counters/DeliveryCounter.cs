using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{


    public override void Interact(Player player)
    {
        // 카운터 위에 오브젝트가 존재하지 않는다면..
        if (!HasKitchenObject())
        {
            // 플레이어가 오브젝트를 소유하고 있다면..
            if (player.HasKitchenObject())
            {
                // 플레이어가 소유한 오브젝트가 접시라면..
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    player.GetKitchenObject().DestroySelf();
                }
            }

        }
    }
}
