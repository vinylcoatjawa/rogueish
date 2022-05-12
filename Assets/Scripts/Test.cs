using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NoiseUtils;

public class Test : MonoBehaviour
{
    PlayerInputActions playerInputActions;
    Grid<int> grid;
    [SerializeField] private LevelVisual levelVisual;
    Grid<bool> dlaGrid;
    Noise noise = new Noise();


    int gridWidth = 10;
    int gridHeight = 10;
    float cellSize = 5f;
    Vector3 startPos = Vector3.zero;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Mouse.Enable();
        playerInputActions.Mouse.LeftClick.started += LeftCLicked;
        playerInputActions.Mouse.RightClick.started += RightClicked;


    }





    void Start()
    {
        //grid = new Grid<int>(50, 50, 2f, Vector3.zero, () => 0);

        //levelVisual.SetGrid(grid);

        dlaGrid = new Grid<bool>(gridWidth, gridHeight, cellSize, startPos, () => false);




    }

    private Vector3 GetMouseWorldPosition(Camera worldCamera, Vector3 screenPoint)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPoint);
        return worldPosition;
    }
    private void RightClicked(InputAction.CallbackContext obj)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 screenPoint = new Vector3(mousePos.x, mousePos.y, 60); // 60 depends on the cams Z position
        Vector3 worldPos = GetMouseWorldPosition(Camera.main, screenPoint);
        Debug.Log(grid.GetGridObject(worldPos));
    }
    private void LeftCLicked(InputAction.CallbackContext obj)
    {


        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 screenPoint = new Vector3(mousePos.x, mousePos.y, 60); // 60 depends on the cams Z position
        Vector3 worldPos = GetMouseWorldPosition(Camera.main, screenPoint);
        grid.SetGridObject(worldPos, 43);
    }

}
