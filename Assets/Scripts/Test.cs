using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    PlayerInputActions playerInputActions;
    Grid grid;


    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Mouse.Enable();
        playerInputActions.Mouse.LeftClick.started += LeftCLicked;
        playerInputActions.Mouse.RightClick.started += RightClicked;


    }

    private void RightClicked(InputAction.CallbackContext obj)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 screenPoint = new Vector3(mousePos.x, mousePos.y, 60); // 60 depends on the cams Z position
        Vector3 worldPos = GetMouseWorldPosition(Camera.main, screenPoint);
        Debug.Log(grid.GetValue(worldPos));
    }

    private void LeftCLicked(InputAction.CallbackContext obj)
    {

        
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 screenPoint = new Vector3(mousePos.x, mousePos.y, 60); // 60 depends on the cams Z position
        Vector3 worldPos = GetMouseWorldPosition(Camera.main, screenPoint);
        grid.SetValue(worldPos, 43);
    }

    void Start()
    {

        grid = new Grid(3, 4, 10f, new Vector3(20,0));
        
    }

    private Vector3 GetMouseWorldPosition(Camera worldCamera, Vector3 screenPoint)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPoint);
        return worldPosition;
    }


}
