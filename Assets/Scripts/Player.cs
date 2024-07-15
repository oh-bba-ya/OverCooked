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
        // 물체 충돌 감지시 이동 불가
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up  * playerHeight, playerRadius, moveDir, moveDistance);



        // 다음 로직은 물체에 부딪혔을 때 대각선방향을 누르면 움직일 수 있는 방향으로 이동 하는 로직..
        // 물체가 감지되어 그 방향으로 움직일 수 없다면..
        if (!canMove)
        {

            // x 방향으로 움직이려하면..
            Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f).normalized; // 정규화, 물체 충돌시 느려짐 방지
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            // x방향으로 움직일 수 있다면..
            if(canMove)
            {
                moveDir = moveDirX;
            }
            else // x방향으로 움직일 수 없다면..  다른 방향으로 이동 시도..
            {

                // z방향으로 움직이려 한다면..
                Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z).normalized;   // 정규화, 물체 충돌시 느려짐 방지
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);


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

    public bool IsWalking()
    {
        return isWalking;
    }

}
