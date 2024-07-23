using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter , IHasProgress
{

    public static event EventHandler OnAnyCut;   // ����

    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }

    // ���α׷����� �̺�Ʈ
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    // ���� �ִϸ��̼� �̺�Ʈ
    public event EventHandler OnCut;   // �ִϸ��̼�


    [SerializeField] private CuttingRecipeSO[] cuttingRecipeArray;


    private int cuttingProgress;

    public override void Interact(Player player)
    {
        // ī���� ���� ������Ʈ�� �������� �ʴ´ٸ�..
        if (!HasKitchenObject())
        {
            // �÷��̾ ������Ʈ�� �����ϰ� �ִٸ�..
            if (player.HasKitchenObject())
            {
                // ������(�ٵ��� �� �ִ� �����)�� �����Ѵٸ�..
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    // ������Ʈ Drop �� ����
                    KitchenObject kitchenObject = player.GetKitchenObject();

                    kitchenObject.SetKitchenObjectParent(this);

                    InteractLogicPlaceObjectOnCounterServerRpc();


                }
            }
            else // �÷��̾ �����ϰ� ���� �ʴٸ�..
            {


            }
        }
        else  // ī���� ���� ������Ʈ�� �����Ѵٸ�..
        {
            // �÷��̾ ������Ʈ�� �����ϰ� �ִٸ�..
            if (player.HasKitchenObject())
            {
                // ���� Ŭ������ ����ȯ�� �����ϴٸ�.. PlateKitchenObject Ŭ������ ��ȯ..
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {

                    // ���ÿ� ������Ʈ�� ���� �� �ִٸ�..
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        // ������Ʈ ����..
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
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

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc()
    {
        InteractLogicPlaceObjectOnCounterClientRpc();
    }

    [ClientRpc]
    private void InteractLogicPlaceObjectOnCounterClientRpc()
    {
        cuttingProgress = 0;


        // ���α׷����� �̺�Ʈ
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = 0f
        });
    }



    public override void InteractAlternate(Player player)
    {
        // ������Ʈ�� �����ϰ� ������(�ٵ��� �� �ִ� �����)�� �����Ѵٸ�..
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            CutObjectServerRpc();
            TestCuttingProgressDoneServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CutObjectServerRpc()
    {
        CutObjectClientRpc();
    }

    [ClientRpc]
    private void CutObjectClientRpc()
    {
        cuttingProgress++;

        // �ִϸ��̼� �̺�Ʈ 
        OnCut?.Invoke(this, EventArgs.Empty);

        // ���� �̺�Ʈ
        OnAnyCut?.Invoke(this, EventArgs.Empty);

        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

        // ���α׷����� �̺�Ʈ
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
        });

    }

    [ServerRpc(RequireOwnership = false)]
    private void TestCuttingProgressDoneServerRpc()
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

        // ������ �Ϸ�Ǹ�..
        if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
        {
            KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

            KitchenObject.DestroyKitchenObject(GetKitchenObject());

            KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
        }
    }


    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        // ������ ������ ������Ʈ���..
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);

        return cuttingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        if(cuttingRecipeSO != null)
        {
            // ������Ʈ ����.. ��ȯ
            return cuttingRecipeSO.output;
        }
        else
        {
            return null;
        }

    }

    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipe in cuttingRecipeArray)
        {
            // ���� ���� ī���� ���� �����ϴ� ������Ʈ�� ���� ī���͸� ����� �� �ִ� recipe���..
            if (cuttingRecipe.input == inputKitchenObjectSO)
            {
                // ���� ������ ��ȯ..
                return cuttingRecipe;
            }
        }

        return null;
    }
}
