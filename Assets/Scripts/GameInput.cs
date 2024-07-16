using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnInterAction;
    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();

        // New Input 시스템 활성화..
        playerInputActions.Player.Enable();

        // Input Action Controller에서 설정한 interact 키를 누른 후 실행 완료..
        playerInputActions.Player.Interact.performed += Interact_performed;
    }

    // InputAction.CallbackContext <- 파라미터가 있어야 이벤트 등록 됌
    private void Interact_performed(InputAction.CallbackContext obj)
    {
        OnInterAction?.Invoke(this,EventArgs.Empty); 
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
}
