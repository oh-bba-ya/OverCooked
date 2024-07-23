using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgress
{    
    // ���α׷����� �̺�Ʈ
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    // ����� ���� �̺�Ʈ
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }


    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;



    private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);
    private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
    private FryingRecipeSO fryingRecipeSO;
    private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);
    private BurningRecipeSO burningRecipeSO;


    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
        burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
        state.OnValueChanged += State_OnValueChanged;
    }

    private void FryingTimer_OnValueChanged(float previousValue, float newValue)
    {
        float fryingTimerMax = fryingRecipeSO != null ? fryingRecipeSO.fryingTimerMax : 1f;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = fryingTimer.Value / fryingTimerMax
        });
    }
    private void BurningTimer_OnValueChanged(float previousValue, float newValue)
    {
        float burningTimerMax = burningRecipeSO != null ? burningRecipeSO.burningTimerMax : 1f;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = burningTimer.Value / burningTimerMax
        });
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        // ������Ʈ �̺�Ʈ ����
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = state.Value
        });

        if(state.Value == State.Burned || state.Value == State.Idle)
        {
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = 0f
            });
        }
    }

    private void Update()
    {
        if(!IsServer) { return; }


        // ����꿡 ������Ʈ�� �����ϸ�..
        if (HasKitchenObject())
        {
            // ���� ������Ʈ �ӽ�
            // ���� �Ϸ� �� �����̻� �ð��� ���� �� ��ᰡ ��Ž
            switch (state.Value)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    // Ÿ�̸� ����
                    fryingTimer.Value += Time.deltaTime;


                    // ������ �ð� �̻��� �Ǿ��ٸ�..
                    if (fryingTimer.Value > fryingRecipeSO.fryingTimerMax)
                    {

                        KitchenObject.DestroyKitchenObject(GetKitchenObject());

                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                        // ���� �Ϸ�
                        state.Value = State.Fried;
                        // Ÿ�� �ð� 0���� �ʱ�ȭ.
                        burningTimer.Value = 0f;

                        SetBurningRecipeSOClientRpc(
                            KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO())
                        );

                    }
                    break;
                case State.Fried:
                    // Ÿ�̸� ����
                    burningTimer.Value += Time.deltaTime;


                    // ������ �ð� �̻��� �Ǿ��ٸ�..
                    if (burningTimer.Value > burningRecipeSO.burningTimerMax)
                    {

                        KitchenObject.DestroyKitchenObject(GetKitchenObject());

                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                        // ��� �ҿ� Ž
                        state.Value = State.Burned;

                    }
                    break;
                case State.Burned:
                    break;
            }
        }
    }
    public override void Interact(Player player)
    {
        // ī���� ���� ������Ʈ�� �������� �ʴ´ٸ�..
        if (!HasKitchenObject())
        {
            // �÷��̾ ������Ʈ�� �����ϰ� �ִٸ�..
            if (player.HasKitchenObject())
            {
                // ������(ȭ���� �ʿ��� ������,FryingRecipeSO)�� �����Ѵٸ�..
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    KitchenObject kitchenObject = player.GetKitchenObject();

                    // ������Ʈ Drop �� ����
                    kitchenObject.SetKitchenObjectParent(this);

                    InteractLogicPlaceObjectOnCounterServerRpc(
                        KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO())
                        );

                }
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
                // ���� Ŭ������ ����ȯ�� �����ϴٸ�.. PlateKitchenObject Ŭ������ ��ȯ..
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {

                    // ���ÿ� ������Ʈ�� ���� �� �ִٸ�..
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());

                        SetStateIdleServerRpc();

                    }
                }
            }
            else // �÷��̾ ������Ʈ�� �����ϰ� ���� �ʴٸ�..
            {
                // ������Ʈ Pick up
                GetKitchenObject().SetKitchenObjectParent(player);

                SetStateIdleServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc()
    {
        state.Value = State.Idle;
    }


    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc(int kitchenObjectSOIndex)
    {
        fryingTimer.Value = 0f;

        // ���� ����..
        state.Value = State.Frying;

        SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

        fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);

    }

    [ClientRpc]
    private void SetBurningRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

        burningRecipeSO = GetBurningRecipeSOWithInput(kitchenObjectSO);

    }


    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        // ȭ���� �ʿ��� �����Ƕ��.. 
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);


        // FryingRecipeSO Ŭ������ �����Ѵٸ�.. True
        return fryingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        if (fryingRecipeSO != null)
        {
            // ȭ�� ������.. ��ȯ
            return fryingRecipeSO.output;
        }
        else
        {
            return null;
        }

    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            // ���� ���� ī���� ���� �����ϴ� ������Ʈ�� ���� ī���͸� ����� �� �ִ� recipe���..
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                // ���� ������ ��ȯ..
                return fryingRecipeSO;
            }
        }

        return null;
    }

    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            // ���� ���� ī���� ���� �����ϴ� ������Ʈ�� ���� ī���͸� ����� �� �ִ� recipe���..
            if (burningRecipeSO.input == inputKitchenObjectSO)
            {
                // ���� ������ ��ȯ..
                return burningRecipeSO;
            }
        }

        return null;
    }

    public bool IsFried()
    {
        return state.Value == State.Fried;
    }
}
