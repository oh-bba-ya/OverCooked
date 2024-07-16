using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectso;


    public override void Interact(Player player)
    {
        // 카운터 위에 오브젝트가 존재하지 않는다면..
        if (!HasKitchenObject())
        {
            // 플레이어가 오브젝트를 소유하고 있다면..
            if (player.HasKitchenObject())
            {
                // 오브젝트 Drop
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else // 플레이어가 소유하고 있지 않다면..
            {
                // 오브젝트 Pick up


            }
        }
        else  // 카운터 위에 오브젝트가 존재한다면..
        {
            // 플레이어가 오브젝트를 소유하고 있다면..
            if (player.HasKitchenObject())
            {
               
            }
            else // 플레이어가 오브젝트를 소유하고 있지 않다면..
            {
                // 오브젝트 Pick up
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

}
