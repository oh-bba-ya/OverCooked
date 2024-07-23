using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgress
{    
    // 프로그레스바 이벤트
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    // 스토브 상태 이벤트
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
        // 스테이트 이벤트 실행
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


        // 스토브에 오브젝트가 존재하면..
        if (HasKitchenObject())
        {
            // 굽기 스테이트 머신
            // 굽기 완료 후 일정이상 시간이 지난 후 재료가 불탐
            switch (state.Value)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    // 타이머 동작
                    fryingTimer.Value += Time.deltaTime;


                    // 설정된 시간 이상이 되었다면..
                    if (fryingTimer.Value > fryingRecipeSO.fryingTimerMax)
                    {

                        KitchenObject.DestroyKitchenObject(GetKitchenObject());

                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                        // 굽기 완료
                        state.Value = State.Fried;
                        // 타는 시간 0으로 초기화.
                        burningTimer.Value = 0f;

                        SetBurningRecipeSOClientRpc(
                            KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO())
                        );

                    }
                    break;
                case State.Fried:
                    // 타이머 동작
                    burningTimer.Value += Time.deltaTime;


                    // 설정된 시간 이상이 되었다면..
                    if (burningTimer.Value > burningRecipeSO.burningTimerMax)
                    {

                        KitchenObject.DestroyKitchenObject(GetKitchenObject());

                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                        // 재료 불에 탐
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
        // 카운터 위에 오브젝트가 존재하지 않는다면..
        if (!HasKitchenObject())
        {
            // 플레이어가 오브젝트를 소유하고 있다면..
            if (player.HasKitchenObject())
            {
                // 레시피(화구가 필요한 레시피,FryingRecipeSO)가 존재한다면..
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    KitchenObject kitchenObject = player.GetKitchenObject();

                    // 오브젝트 Drop 및 굽기
                    kitchenObject.SetKitchenObjectParent(this);

                    InteractLogicPlaceObjectOnCounterServerRpc(
                        KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO())
                        );

                }
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
                // 접시 클래스로 형변환이 가능하다면.. PlateKitchenObject 클래스로 반환..
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {

                    // 접시에 오브젝트를 놓을 수 있다면..
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());

                        SetStateIdleServerRpc();

                    }
                }
            }
            else // 플레이어가 오브젝트를 소유하고 있지 않다면..
            {
                // 오브젝트 Pick up
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

        // 굽기 시작..
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
        // 화구가 필요한 레시피라면.. 
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);


        // FryingRecipeSO 클래스가 존재한다면.. True
        return fryingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        if (fryingRecipeSO != null)
        {
            // 화구 레시피.. 반환
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
            // 현재 컷팅 카운터 위에 존재하는 오브젝트와 컷팅 카운터를 사용할 수 있는 recipe라면..
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                // 컷팅 데이터 반환..
                return fryingRecipeSO;
            }
        }

        return null;
    }

    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            // 현재 컷팅 카운터 위에 존재하는 오브젝트와 컷팅 카운터를 사용할 수 있는 recipe라면..
            if (burningRecipeSO.input == inputKitchenObjectSO)
            {
                // 컷팅 데이터 반환..
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
