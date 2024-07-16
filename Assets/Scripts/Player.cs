using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance
    {
        get; private set;
    }


    public event EventHandler<OnSelectedCounterChangedEvenetArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEvenetArgs : EventArgs
    {
        public ClearCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask counterLayerMask;

    private bool isWalking;

    private Vector3 lastInteractDir;

    private ClearCounter selectedCounter;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("�÷��̾� �ν��Ͻ� 1�� �̻� ����");
        }
        Instance = this;
    }

    private void Start()
    {
        gameInput.OnInterAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if(selectedCounter != null)
        {
            selectedCounter.Interact();
        }

    }

    private void Update()
    {

        HandleMovement();

        HandleInteractions();
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
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
            if(raycastHit.transform.TryGetComponent(out ClearCounter clearCounter))
            {
                if(clearCounter != selectedCounter)
                {
                    SetSelectedCounter(clearCounter);
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
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
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
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            // x�������� ������ �� �ִٸ�..
            if (canMove)
            {
                moveDir = moveDirX;
            }
            else // x�������� ������ �� ���ٸ�..  �ٸ� �������� �̵� �õ�..
            {

                // z�������� �����̷� �Ѵٸ�..
                Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z).normalized;   // ����ȭ, ��ü �浹�� ������ ����
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);


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


    private void SetSelectedCounter(ClearCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEvenetArgs
        {
            selectedCounter = selectedCounter
        });
    }
}
