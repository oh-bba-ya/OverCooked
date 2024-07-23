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
    private float spawnRecipeTimerMax = 4f; // �ֹ� ��Ÿ��..
    private int waitingRecipesMax = 4;   // �ִ� ������ �ֹ� ����
    private int successfulRecipesAmount;   // �ֹ� ���� ����..


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

            // ���� ����ϰ� �ִ� �ֹ����� Max���� �۴ٸ�..
            if (waitingRecipeSOList.Count < waitingRecipesMax)
            {
                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);

                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);
            }
        }
    }


    /// <summary>
    /// ������ �ֹ� ���� ����ȭ �Լ�
    /// �������� ����� Method ��� Client���� ����ȭ
    /// </summary>
    /// <param name="waitingRecipeSO">���� ������ �ֹ��� ���� Index</param>
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

            // ��ٸ��� �ִ� �ֹ��� ������ ��� ������ ������ ��� ������ �����ϴٸ�..
            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                // ���� ������� �ֹ��� ������(�ʿ��� ���)��ŭ �ݺ�
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    bool ingredientFound = false;

                    // ���� ������ ����� ������ŭ �ݺ�..
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        // ��� ����.
                        if (plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            ingredientFound = true;
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
                    DeliveryCorrectRecipeServerRpc(i);
                    return;
                }
            }
        }

        //  �÷��̾ ���ÿ� ���� ������ ������� ���İ� �ٸ�..
        DeliveryIncorrectRecipeServerRpc();
    }

    // ServerRPC�� NetworkObject�� �������� ���� Ŭ���̾�Ʈ�� ȣ�� ����
    // RequireOwnership = fasls :  �������� ���� Ŭ���̾�Ʈ�� ServerRPC ȣ�Ⱑ��
    [ServerRpc(RequireOwnership =false)]
    private void DeliveryIncorrectRecipeServerRpc()
    {
        DeliveryIncorrectRecipeClientRpc();
    }

    [ClientRpc]
    private void DeliveryIncorrectRecipeClientRpc()
    {
        //  �÷��̾ ���ÿ� ���� ������ ������� ���İ� �ٸ�..
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Ŭ���̾�Ʈ -> ������ ����ȭ
    /// ������ ��� Ŭ���̾�Ʈ��� ����ȭ�Ǿ� �ֱ� ������
    /// ������ ���� ����ȭ�� ���� �� �� �ֵ��� ServerRPC ���
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    private void DeliveryCorrectRecipeServerRpc(int waitingRecipeSOListIndex)
    {
        DeliveryCorrectRecipeClientRpc(waitingRecipeSOListIndex);
    }

    /// <summary>
    /// ���� -> Ŭ���̾�Ʈ ����ȭ
    /// ��� Ŭ���̾�Ʈ���� �ֹ� �Ϸ� ����ȭ
    /// </summary>
    [ClientRpc]
    private void DeliveryCorrectRecipeClientRpc(int waitingRecipeSOListIndex)
    {
        // �ֹ� �ϼ� ���� ī��Ʈ ����..
        successfulRecipesAmount++;

        // ���� ������� �ֹ� ����..
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
