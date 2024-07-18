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
                // 플레이어가 소유하고 있는 오브젝트를 접시 클래스로 형변환이 가능하다면.. PlateKitchenObject 클래스로 반환..
                // 즉, 플레이어가 접시를 소유하고 있다면..
               if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {

                    // 접시에 올릴 수 있는 오브젝트라면..
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        // 오브젝트 삭제..
                        GetKitchenObject().DestroySelf();
                    }
               }
               else // 형변환 실패..(플레이어가 접시가아닌 다른 오브젝트를 소유하고 있음)
               {
                    // 카운터에 있는 오브젝트가 접시 오브젝트라면..
                    if(GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        // 접시에 올릴 수 있는 오브젝트라면..
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            // 오브젝트 삭제..
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
               }
            }
            else // 플레이어가 오브젝트를 소유하고 있지 않다면..
            {
                // 오브젝트 Pick up
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

}
