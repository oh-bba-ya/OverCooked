using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
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
        spawnRecipeTimer -= Time.deltaTime;
        if(spawnRecipeTimer <= 0f )
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            // 현재 대기하고 있는 주문량이 Max보다 작다면..
            if(waitingRecipeSOList.Count < waitingRecipesMax)
            {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];

                waitingRecipeSOList.Add(waitingRecipeSO);

                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for(int i=0;i<waitingRecipeSOList.Count;i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            // 기다리고 있는 주문의 레시피 재료 개수와 접시의 재료 개수가 동일하다면..
            if(waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                // 현재 대기중인 주문의 레시피(필요한 재료)만큼 반복
                bool plateContentsMatchesRecipe = true;
                foreach(KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    bool ingredientFound = false;

                    // 현재 접시의 재료의 개수만큼 반복..
                    foreach(KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        // 재료 동일.
                        if(plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            ingredientFound=true;
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
                    // 주문 완성 개수 카운트 증가..
                    successfulRecipesAmount++;

                    // 현재 대기중인 주문 삭제..
                    waitingRecipeSOList.RemoveAt(i);

                    OnRecipeComplete?.Invoke(this,EventArgs.Empty);

                    OnRecipeSuccess?.Invoke(this,EventArgs.Empty);
                    return;
                }
            }
        }

        //  플레이어가 접시에 담은 음식이 대기중인 음식과 다름..
        OnRecipeFailed?.Invoke(this,EventArgs.Empty);
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
