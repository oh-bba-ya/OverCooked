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

    public event EventHandler OnPickedSomething;  // ���� �̺�Ʈ
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


        // ������ ������ �����ϴ� ���� : Ű���带 ������ ���� ���¿����� ������Ʈ���� �浹�� ������..
        // ����Ű �ȴ����� moveDir = Vector3.Zero �� ������ ���� ����.
        // ����Ű�� �Է��ߴٸ�..
        if(moveDir != Vector3.zero)
        {
            // �������� ���� ����Ű ���� ����..
            lastInteractDir = moveDir;
        }

        float interactionDistance = 2f;
        if(Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactionDistance, counterLayerMask))
        {
            // ������Ʈ Tag ����� ������
            // Tag �� string ���� �����ؾ��ϱ� ������ ���� ã�Ⱑ ���� �ʴ�.
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
        // ��ü �浹 ������ �̵� �Ұ�
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);



        // ���� ������ ��ü�� �ε����� �� �밢�������� ������ ������ �� �ִ� �������� �̵� �ϴ� ����..
        // ��ü�� �����Ǿ� �� �������� ������ �� ���ٸ�..
        if (!canMove)
        {

            // x �������� �����̷��ϸ�..
            Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f).normalized; // ����ȭ, ��ü �浹�� ������ ����
            canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            // x�������� ������ �� �ִٸ�..
            if (canMove)
            {
                moveDir = moveDirX;
            }
            else // x�������� ������ �� ���ٸ�..  �ٸ� �������� �̵� �õ�..
            {

                // z�������� �����̷� �Ѵٸ�..
                Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z).normalized;   // ����ȭ, ��ü �浹�� ������ ����
                canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);


                // Z �������� ������ �� �ִٸ�..
                if (canMove)
                {
                    moveDir = moveDirZ;
                }
                else // Z �������� ������ �� ���ٸ�..
                {

                }
            }
        }

        if (canMove)
        {

            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        isWalking = moveDir != Vector3.zero;

        // W,A,S,D �������� �ε巴�� ȸ��..�ϸ� ĳ���Ͱ� �� ������ �ٶ�
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
