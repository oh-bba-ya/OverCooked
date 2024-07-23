using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour ,IKitchenObjectParent
{

    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPickedSomething;

    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
    }
    
    public static Player LocalInstance{ get; private set;}

    public event EventHandler OnPickedSomething;  // 사운드 이벤트
    public event EventHandler<OnSelectedCounterChangedEvenetArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEvenetArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private bool isWalking;

    private Vector3 lastInteractDir;

    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;


    private void Start()
    {
        GameInput.Instance.OnInterAction += GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAlternate += GameInput_OnInteractAlternateAction;
    }

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            LocalInstance = this;
        }

        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!OverCookGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (!OverCookGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }

    }

    private void Update()
    {
        if(!IsOwner) return;

        HandleMovement();

        HandleInteractions();
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);


        // 마지막 방향을 저장하는 이유 : 키보드를 누르지 않은 상태에서도 오브젝트와의 충돌을 감지함..
        // 방향키 안누르면 moveDir = Vector3.Zero 와 동일한 값을 가짐.
        // 방향키를 입력했다면..
        if(moveDir != Vector3.zero)
        {
            // 마지막에 누른 방향키 방향 저장..
            lastInteractDir = moveDir;
        }

        float interactionDistance = 2f;
        if(Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactionDistance, counterLayerMask))
        {
            // 오브젝트 Tag 사용을 피하자
            // Tag 는 string 으로 구분해야하기 때문에 에러 찾기가 쉽지 않다.
            if(raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if(baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }

    }


    private void HandleMovement()
    {
        if (!OverCookGameManager.Instance.IsGamePlaying()) return;

        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        // 물체 충돌 감지시 이동 불가
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);



        // 다음 로직은 물체에 부딪혔을 때 대각선방향을 누르면 움직일 수 있는 방향으로 이동 하는 로직..
        // 물체가 감지되어 그 방향으로 움직일 수 없다면..
        if (!canMove)
        {

            // x 방향으로 움직이려하면..
            Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f).normalized; // 정규화, 물체 충돌시 느려짐 방지
            canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            // x방향으로 움직일 수 있다면..
            if (canMove)
            {
                moveDir = moveDirX;
            }
            else // x방향으로 움직일 수 없다면..  다른 방향으로 이동 시도..
            {

                // z방향으로 움직이려 한다면..
                Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z).normalized;   // 정규화, 물체 충돌시 느려짐 방지
                canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);


                // Z 방향으로 움직일 수 있다면..
                if (canMove)
                {
                    moveDir = moveDirZ;
                }
                else // Z 방향으로 움직일 수 없다면..
                {

                }
            }
        }

        if (canMove)
        {

            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        isWalking = moveDir != Vector3.zero;

        // W,A,S,D 방향으로 부드럽게 회전..하며 캐릭터가 그 방향을 바라봄
        transform.forward = Vector3.Slerp(transform.forward, moveDir, rotateSpeed * Time.deltaTime);

    }


    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEvenetArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if(kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
            OnAnyPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
