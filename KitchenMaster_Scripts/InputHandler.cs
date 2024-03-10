using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }
    private PlayerInputActions playerInputActions;

    public event Action OnInteractPerformed;
    public event Action OnInteractAlternatePerformed;
    public event Action OnPausePerformed;

    public enum Binding
    {
        Move_UP,
        Move_Down,
        Move_Left,
        Move_Right,
        Interact,
        InteractAlt,
        Pause
    }

    private void Awake()
    {
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();

        playerInputActions.PlayerAction.Interact.performed += Interact_performed;
        playerInputActions.PlayerAction.InteractAlternate.performed += InteractAlternate_performed;
        playerInputActions.PlayerAction.Pause.performed += Pause_performed;
    }

    private void OnDestroy()
    {
        playerInputActions.PlayerAction.Interact.performed -= Interact_performed;
        playerInputActions.PlayerAction.InteractAlternate.performed -= InteractAlternate_performed;
        playerInputActions.PlayerAction.Pause.performed -= Pause_performed;

        playerInputActions.Dispose();
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPausePerformed?.Invoke();
    }

    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAlternatePerformed?.Invoke();
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractPerformed?.Invoke();
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 movementInput = playerInputActions.PlayerAction.Movement.ReadValue<Vector2>();

        movementInput = movementInput.normalized;
        return movementInput;
    }

    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            default:
            case Binding.Move_UP:
                return playerInputActions.PlayerAction.Movement.bindings[1].ToDisplayString();

            case Binding.Move_Down:
                return playerInputActions.PlayerAction.Movement.bindings[2].ToDisplayString();

            case Binding.Move_Left:
                return playerInputActions.PlayerAction.Movement.bindings[3].ToDisplayString();

            case Binding.Move_Right:
                return playerInputActions.PlayerAction.Movement.bindings[4].ToDisplayString();

            case Binding.Interact:
                return playerInputActions.PlayerAction.Interact.bindings[0].ToDisplayString();;

            case Binding.InteractAlt:
                return playerInputActions.PlayerAction.InteractAlternate.bindings[0].ToDisplayString();

            case Binding.Pause:
                return playerInputActions.PlayerAction.Pause.bindings[0].ToDisplayString();
        }
    }
}
