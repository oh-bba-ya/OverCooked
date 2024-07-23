using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter , IHasProgress
{

    public static event EventHandler OnAnyCut;   // 사운드

    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }

    // 프로그레스바 이벤트
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    // 컷팅 애니메이션 이벤트
    public event EventHandler OnCut;   // 애니메이션


    [SerializeField] private CuttingRecipeSO[] cuttingRecipeArray;


    private int cuttingProgress;

    public override void Interact(Player player)
    {
        // 카운터 위에 오브젝트가 존재하지 않는다면..
        if (!HasKitchenObject())
        {
            // 플레이어가 오브젝트를 소유하고 있다면..
            if (player.HasKitchenObject())
            {
                // 레시피(다듬을 수 있는 재료라면)가 존재한다면..
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    // 오브젝트 Drop 및 컷팅
                    KitchenObject kitchenObject = player.GetKitchenObject();

                    kitchenObject.SetKitchenObjectParent(this);

                    InteractLogicPlaceObjectOnCounterServerRpc();


                }
            }
            else // 플레이어가 소유하고 있지 않다면..
            {


            }
        }
        else  // 카운터 위에 오브젝트가 존재한다면..
        {
            // 플레이어가 오브젝트를 소유하고 있다면..
            if (player.HasKitchenObject())
            {
                // 접시 클래스로 형변환이 가능하다면.. PlateKitchenObject 클래스로 반환..
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {

                    // 접시에 오브젝트를 놓을 수 있다면..
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        // 오브젝트 삭제..
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
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

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc()
    {
        InteractLogicPlaceObjectOnCounterClientRpc();
    }

    [ClientRpc]
    private void InteractLogicPlaceObjectOnCounterClientRpc()
    {
        cuttingProgress = 0;


        // 프로그레스바 이벤트
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = 0f
        });
    }



    public override void InteractAlternate(Player player)
    {
        // 오브젝트가 존재하고 레시피(다듬을 수 있는 재료라면)가 존재한다면..
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

        // 애니메이션 이벤트 
        OnCut?.Invoke(this, EventArgs.Empty);

        // 사운드 이벤트
        OnAnyCut?.Invoke(this, EventArgs.Empty);

        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

        // 프로그레스바 이벤트
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
        });

    }

    [ServerRpc(RequireOwnership = false)]
    private void TestCuttingProgressDoneServerRpc()
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

        // 진행이 완료되면..
        if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
        {
            KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

            KitchenObject.DestroyKitchenObject(GetKitchenObject());

            KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
        }
    }


    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        // 컷팅이 가능한 오브젝트라면..
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);

        return cuttingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        if(cuttingRecipeSO != null)
        {
            // 오브젝트 컷팅.. 반환
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
            // 현재 컷팅 카운터 위에 존재하는 오브젝트와 컷팅 카운터를 사용할 수 있는 recipe라면..
            if (cuttingRecipe.input == inputKitchenObjectSO)
            {
                // 컷팅 데이터 반환..
                return cuttingRecipe;
            }
        }

        return null;
    }
}
