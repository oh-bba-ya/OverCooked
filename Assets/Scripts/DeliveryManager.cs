using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeComplete;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;

    public static DeliveryManager Instance { get; private set; }
    [SerializeField] private RecipeListSO recipeListSO;
    private List<RecipeSO> waitingRecipeSOList;


    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f; // 주문 쿨타임..
    private int waitingRecipesMax = 4;   // 최대 웨이팅 주문 개수
    private int successfulRecipesAmount;   // 주문 성공 개수..


    private void Awake()
    {
        Instance = this;


        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update()
    {
        if (!IsServer) { return; }


        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            // 현재 대기하고 있는 주문량이 Max보다 작다면..
            if (waitingRecipeSOList.Count < waitingRecipesMax)
            {
                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);

                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);
            }
        }
    }


    /// <summary>
    /// 웨이팅 주문 생성 동기화 함수
    /// 서버에서 실행된 Method 모든 Client에게 동기화
    /// </summary>
    /// <param name="waitingRecipeSO">랜덤 웨이팅 주문을 위한 Index</param>
    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex)
    {
        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];

        waitingRecipeSOList.Add(waitingRecipeSO);

        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            // 기다리고 있는 주문의 레시피 재료 개수와 접시의 재료 개수가 동일하다면..
            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                // 현재 대기중인 주문의 레시피(필요한 재료)만큼 반복
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    bool ingredientFound = false;

                    // 현재 접시의 재료의 개수만큼 반복..
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        // 재료 동일.
                        if (plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            ingredientFound = true;
                            break;
                        }
                    }
                    // 현재 대기중인 주문이 접시에 담긴 재료와 다르다면..
                    if (!ingredientFound)
                    {
                        plateContentsMatchesRecipe = false;
                    }
                }

                // 플레이어가 접시에 담은 재료와 일치..
                if (plateContentsMatchesRecipe)
                {
                    DeliveryCorrectRecipeServerRpc(i);
                    return;
                }
            }
        }

        //  플레이어가 접시에 담은 음식이 대기중인 음식과 다름..
        DeliveryIncorrectRecipeServerRpc();
    }

    // ServerRPC는 NetworkObject의 소유권을 가진 클라이언트만 호출 가능
    // RequireOwnership = fasls :  소유권이 없는 클라이언트도 ServerRPC 호출가능
    [ServerRpc(RequireOwnership =false)]
    private void DeliveryIncorrectRecipeServerRpc()
    {
        DeliveryIncorrectRecipeClientRpc();
    }

    [ClientRpc]
    private void DeliveryIncorrectRecipeClientRpc()
    {
        //  플레이어가 접시에 담은 음식이 대기중인 음식과 다름..
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 클라이언트 -> 서버로 동기화
    /// 서버는 모든 클라이언트들과 동기화되어 있기 때문에
    /// 서버를 통해 동기화를 진행 할 수 있도록 ServerRPC 사용
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    private void DeliveryCorrectRecipeServerRpc(int waitingRecipeSOListIndex)
    {
        DeliveryCorrectRecipeClientRpc(waitingRecipeSOListIndex);
    }

    /// <summary>
    /// 서버 -> 클라이언트 동기화
    /// 모든 클라이언트에게 주문 완료 동기화
    /// </summary>
    [ClientRpc]
    private void DeliveryCorrectRecipeClientRpc(int waitingRecipeSOListIndex)
    {
        // 주문 완성 개수 카운트 증가..
        successfulRecipesAmount++;

        // 현재 대기중인 주문 삭제..
        waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);

        OnRecipeComplete?.Invoke(this, EventArgs.Empty);

        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipesAmount()
    {
        return successfulRecipesAmount;
    }
}
