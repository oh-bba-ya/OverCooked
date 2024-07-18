using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectso;


    public override void Interact(Player player)
    {
        // ī���� ���� ������Ʈ�� �������� �ʴ´ٸ�..
        if (!HasKitchenObject())
        {
            // �÷��̾ ������Ʈ�� �����ϰ� �ִٸ�..
            if (player.HasKitchenObject())
            {
                // ������Ʈ Drop
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else // �÷��̾ �����ϰ� ���� �ʴٸ�..
            {
                // ������Ʈ Pick up


            }
        }
        else  // ī���� ���� ������Ʈ�� �����Ѵٸ�..
        {
            // �÷��̾ ������Ʈ�� �����ϰ� �ִٸ�..
            if (player.HasKitchenObject())
            {
                // �÷��̾ �����ϰ� �ִ� ������Ʈ�� ���� Ŭ������ ����ȯ�� �����ϴٸ�.. PlateKitchenObject Ŭ������ ��ȯ..
                // ��, �÷��̾ ���ø� �����ϰ� �ִٸ�..
               if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {

                    // ���ÿ� �ø� �� �ִ� ������Ʈ���..
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        // ������Ʈ ����..
                        GetKitchenObject().DestroySelf();
                    }
               }
               else // ����ȯ ����..(�÷��̾ ���ð��ƴ� �ٸ� ������Ʈ�� �����ϰ� ����)
               {
                    // ī���Ϳ� �ִ� ������Ʈ�� ���� ������Ʈ���..
                    if(GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        // ���ÿ� �ø� �� �ִ� ������Ʈ���..
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            // ������Ʈ ����..
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
               }
            }
            else // �÷��̾ ������Ʈ�� �����ϰ� ���� �ʴٸ�..
            {
                // ������Ʈ Pick up
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

}
