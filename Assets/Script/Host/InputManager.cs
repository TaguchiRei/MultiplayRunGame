using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Action<Vector2> OnMove;
    public Action OnJump;
    public Action OnJumpEnd;
    public Action OnDash;
    public Action OnDashEnd;
    
    MultiPlayerInputActions _inputActions;

    private void Start()
    {
        _inputActions = new MultiPlayerInputActions();
        
        _inputActions.Player.Move.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        _inputActions.Player.Jump.started += ctx => OnJump?.Invoke();
        _inputActions.Player.Jump.canceled += ctx => OnJumpEnd?.Invoke();
        _inputActions.Player.Sprint.started += ctx => OnDash?.Invoke();
        _inputActions.Player.Sprint.canceled += ctx => OnDashEnd?.Invoke();
        
        _inputActions.Enable();
    }
}
