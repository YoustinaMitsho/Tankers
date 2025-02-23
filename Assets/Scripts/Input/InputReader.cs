using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

[CreateAssetMenu(fileName = "New Input Reader", menuName = "Input/Input Reaader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    private Controls controls;

    public event Action<bool> PrimaryFireEvent;
    public event Action<Vector2> MoveEvent;
    public Vector2 AimPosition {  get; private set; }


    private void OnEnable()
    {
        if(controls == null)
        {
            controls = new Controls();
            controls.Player.AddCallbacks(this);
        }

        controls.Player.Enable();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnPrimaryFire(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
        if (context.performed)
        {
            PrimaryFireEvent?.Invoke(true);
        }else if (context.canceled)
        {
            PrimaryFireEvent?.Invoke(false);
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
        AimPosition =  context.ReadValue<Vector2>();
    }

    private void OnDestroy()
    {
        controls.Player.Disable();
    }
}
