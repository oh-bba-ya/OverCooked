using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance { get; private set; }
    [SerializeField] private RecipeListSO recipeListSO;
    private List<RecipeSO> waitingRecipeSOList;


    private float spawnRecipeTimer;   
    private float spawnRecipeTimerMax = 4f; // �ֹ� ��Ÿ��..
    private int waitingRecipesMax = 4;   // �ִ� ������ �ֹ� ����


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

            // ���� ����ϰ� �ִ� �ֹ����� Max���� �۴ٸ�..
            if(waitingRecipeSOList.Count < waitingRecipesMax)
            {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[Random.Range(0, recipeListSO.recipeSOList.Count)];
                Debug.Log(waitingRecipeSO.recipeName);
                waitingRecipeSOList.Add(waitingRecipeSO);
            }
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for(int i=0;i<waitingRecipeSOList.Count;i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            // ��ٸ��� �ִ� �ֹ��� ������ ��� ������ ������ ��� ������ �����ϴٸ�..
            if(waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                // ���� ������� �ֹ��� ������(�ʿ��� ���)��ŭ �ݺ�
                bool plateContentsMatchesRecipe = true;
                foreach(KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    bool ingredientFound = false;

                    // ���� ������ ����� ������ŭ �ݺ�..
                    foreach(KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        // ��� ����.
                        if(plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            ingredientFound=true;
                            break;
                        }
                    }
                    // ���� ������� �ֹ��� ���ÿ� ��� ���� �ٸ��ٸ�..
                    if (!ingredientFound)
                    {
                        plateContentsMatchesRecipe = false;
                    }
                }

                // �÷��̾ ���ÿ� ���� ���� ��ġ..
                if (plateContentsMatchesRecipe)
                {
                    // ���� ������� �ֹ� ����..
                    waitingRecipeSOList.RemoveAt(i);
                    Debug.Log("�ֹ���ġ");
                    return;
                }
            }
        }

        //  �÷��̾ ���ÿ� ���� ������ ������� ���İ� �ٸ�..
        Debug.Log("�ֹ� ����ġ");
    }
}
