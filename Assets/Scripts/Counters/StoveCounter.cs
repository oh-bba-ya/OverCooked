using System;
using System.Collections;
using System.Collections.Generic;
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



    private State state;
    private float fryingTimer;
    private FryingRecipeSO fryingRecipeSO;
    private float burningTimer;
    private BurningRecipeSO burningRecipeSO;

    private void Start()
    {
        state = State.Idle;
    }

    private void Update()
    {

        // ����꿡 ������Ʈ�� �����ϸ�..
        if (HasKitchenObject())
        {
            // ���� ������Ʈ �ӽ�
            // ���� �Ϸ� �� �����̻� �ð��� ���� �� ��ᰡ ��Ž
            switch (state)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    // Ÿ�̸� ����
                    fryingTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
                    });

                    // ������ �ð� �̻��� �Ǿ��ٸ�..
                    if (fryingTimer > fryingRecipeSO.fryingTimerMax)
                    {

                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                        // ���� �Ϸ�
                        state = State.Fried;
                        burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                        // Ÿ�� �ð� 0���� �ʱ�ȭ.
                        burningTimer = 0f;

                        // ������Ʈ �̺�Ʈ ����
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });
                    }
                    break;
                case State.Fried:
                    // Ÿ�̸� ����
                    burningTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = burningTimer / burningRecipeSO.burningTimerMax
                    });

                    // ������ �ð� �̻��� �Ǿ��ٸ�..
                    if (burningTimer > burningRecipeSO.burningTimerMax)
                    {

                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                        // ��� �ҿ� Ž
                        state = State.Burned;

                        // ������Ʈ �̺�Ʈ ����
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });


                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
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
                    // ������Ʈ Drop �� ����
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    // ���� ����..
                    state = State.Frying;
                    fryingTimer = 0f;

                    // ������Ʈ �̺�Ʈ ����
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = state
                    });

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
                    });
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
                        // ������Ʈ ����..
                        GetKitchenObject().DestroySelf();

                        state = State.Idle;

                        // ������Ʈ �̺�Ʈ ����
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                }
            }
            else // �÷��̾ ������Ʈ�� �����ϰ� ���� �ʴٸ�..
            {
                // ������Ʈ Pick up
                GetKitchenObject().SetKitchenObjectParent(player);

                state = State.Idle;

                // ������Ʈ �̺�Ʈ ����
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {
                    state = state
                });

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                });
            }
        }
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
}
