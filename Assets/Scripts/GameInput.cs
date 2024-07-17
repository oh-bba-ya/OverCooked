using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnInterAction;
    public event EventHandler OnInteractAlternate;
    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();

        // New Input �ý��� Ȱ��ȭ..
        playerInputActions.Player.Enable();

        // Input Action Controller���� ������ interact Ű�� ���� �� ���� �Ϸ�..
        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
    }

    private void InteractAlternate_performed(InputAction.CallbackContext obj)
    {
        OnInteractAlternate?.Invoke(this,EventArgs.Empty);
    }

    // InputAction.CallbackContext <- �Ķ���Ͱ� �־�� �̺�Ʈ ��� ��
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
