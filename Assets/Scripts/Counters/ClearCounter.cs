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
               
            }
            else // �÷��̾ ������Ʈ�� �����ϰ� ���� �ʴٸ�..
            {
                // ������Ʈ Pick up
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

}
