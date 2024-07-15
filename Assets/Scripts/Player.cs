using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private GameInput gameInput;

    private bool isWalking;

    private void Update()
    {

        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        // ��ü �浹 ������ �̵� �Ұ�
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up  * playerHeight, playerRadius, moveDir, moveDistance);



        // ���� ������ ��ü�� �ε����� �� �밢�������� ������ ������ �� �ִ� �������� �̵� �ϴ� ����..
        // ��ü�� �����Ǿ� �� �������� ������ �� ���ٸ�..
        if (!canMove)
        {

            // x �������� �����̷��ϸ�..
            Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f).normalized; // ����ȭ, ��ü �浹�� ������ ����
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            // x�������� ������ �� �ִٸ�..
            if(canMove)
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

    public bool IsWalking()
    {
        return isWalking;
    }

}
