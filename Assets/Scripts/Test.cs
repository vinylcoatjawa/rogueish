using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NoiseUtils;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    PlayerInputActions playerInputActions;
    //Grid<int> grid;
    [SerializeField] private LevelVisual levelVisual;
    DLA dla = new DLA();
    Grid<bool> dlaGrid;
    Noise noise = new Noise();
    Dictionary<int, uint> dungeonDict;
    public LevelManager currentLevel;
    DungeonManager dungeonManager;
    WorldMapGenerator worldMapGen;
    Scene level1;

    Grid<float> noiseMapGrid;

    Grid<bool> structureOverlay = new Grid<bool>(20, 20, 200, Vector3.zero, () => false);



    int gridWidth = 50;
    int gridHeight = 50;
    float cellSize = 2f;
    Vector3 startPos = Vector3.zero;


    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Mouse.Enable();
        playerInputActions.Mouse.LeftClick.started += LeftCLicked;
        playerInputActions.Mouse.RightClick.started += RightClicked;

        worldMapGen = GetComponent<WorldMapGenerator>();


        //dungeonDict = dungeonManager.GenerateDungeonDict((uint)1);

    }

    void Start()
    {
        //grid = new Grid<int>(gridWidth, gridHeight, cellSize, startPos, () => 0);
        //dlaGrid = new Grid<bool>(gridWidth, gridHeight, cellSize, startPos, () => false);
        //dla.SetGrid(dlaGrid);

        noiseMapGrid = worldMapGen.GenerateMap();



    }

    private Vector3 GetMouseWorldPosition(Camera worldCamera, Vector3 screenPoint)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPoint);
        return worldPosition;
    }
    private void RightClicked(InputAction.CallbackContext obj)
    {
        for (int x = 0; x < structureOverlay.GetWitdth(); x++)
        {
            for (int y = 0; y < structureOverlay.GetHeight(); y++)
            {
                //debugTestArray[x, y] = CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + middleOffset, 20, Color.black, TextAnchor.MiddleCenter);
                //debugTestArray[x, y].gameObject.transform.SetParent(debugGrid.transform);

                Debug.DrawLine(structureOverlay.GetWorldPosition(x, y), structureOverlay.GetWorldPosition(x, y + 1), Color.black, 100f);
                Debug.DrawLine(structureOverlay.GetWorldPosition(x, y), structureOverlay.GetWorldPosition(x + 1, y), Color.black, 100f);
            }
        }

        Debug.DrawLine(structureOverlay.GetWorldPosition(0, structureOverlay.GetHeight()), structureOverlay.GetWorldPosition(structureOverlay.GetWitdth(), structureOverlay.GetHeight()), Color.black, 100f);
        Debug.DrawLine(structureOverlay.GetWorldPosition(structureOverlay.GetWitdth(), 0), structureOverlay.GetWorldPosition(structureOverlay.GetWitdth(), structureOverlay.GetHeight()), Color.black, 100f);

        FindSuitableDungeonLocation();
    }


    /* this entire bit should be a method probably */
    private void FindSuitableDungeonLocation()
    {
        
        
        Vector3 middleOffset = new Vector3(structureOverlay.GetCellSize(), structureOverlay.GetCellSize(), 0) * 0.5f;
        Vector3 middleOffset_1 = new Vector3(noiseMapGrid.GetCellSize(), noiseMapGrid.GetCellSize(), 0) * 0.5f;
        for (int x = 0; x < structureOverlay.GetWitdth(); x++)
        {
            for (int y = 0; y < structureOverlay.GetHeight(); y++)
            {
                //CreateWorldText(structureOverlay.GetGridObject(x, y).ToString(), null, structureOverlay.GetWorldPosition(x, y) + middleOffset, 300, Color.black, TextAnchor.MiddleCenter);
                //CreateWorldText(($"({x}, {y})").ToString(), null, structureOverlay.GetWorldPosition(x, y) + middleOffset, 300, Color.black, TextAnchor.MiddleCenter);

                //Debug.Log($"x is: {x} and y is: {y}");

                bool hasWaterSoFar = false;

                for (int i = x * 20; i < (x + 1) * 20; i++)
                {
                    for (int j = y * 20; j < (y + 1) * 20; j++)
                    {
                        //CreateWorldText(noiseMapGrid.GetGridObject(i, j).ToString(), null, noiseMapGrid.GetWorldPosition(i, j) + middleOffset_1, 10, Color.black, TextAnchor.MiddleCenter);
                        //Debug.Log(noiseMapGrid.GetGridObject(i, j) + $" for i: {i} and j: {j} whilw we are inside {x} , {y} and the boolean is {hasWaterSoFar}");
                        //Debug.Log(noiseMapGrid.GetGridObject(i, j) <= 0.35f);
                        

                        if (noiseMapGrid.GetGridObject(i, j) <= 0.35f)
                        {
                            //CreateWorldText(("Water").ToString(), null, structureOverlay.GetWorldPosition(x, y) + middleOffset, 300, Color.black, TextAnchor.MiddleCenter);
                            //structureOverlay.SetGridObject(x, y, 1);
                            //CreateWorldText(structureOverlay.GetGridObject(x, y).ToString(), null, structureOverlay.GetWorldPosition(x, y) + middleOffset, 300, Color.black, TextAnchor.MiddleCenter);
                            hasWaterSoFar = true;
                            //break;
                        }
                        //else { structureOverlay.SetGridObject(x, y, 1); }

                    
                    }
                    
                    if (hasWaterSoFar) { structureOverlay.SetGridObject(x, y, true); /*break;*/ }
                    //else { structureOverlay.SetGridObject(x, y, 1); }

                }
                CreateWorldText(structureOverlay.GetGridObject(x, y).ToString(), null, structureOverlay.GetWorldPosition(x, y) + middleOffset, 300, Color.black, TextAnchor.MiddleCenter);

                
            }
            
        }

        //structureOverlay.SetGridObject((int)CreateRandomDungeonLocation().x, (int)CreateRandomDungeonLocation().y, 99);
        //CreateWorldText("99", null, structureOverlay.GetWorldPosition((int)CreateRandomDungeonLocation().x, (int)CreateRandomDungeonLocation().y) + middleOffset, 300, Color.black, TextAnchor.MiddleCenter);
    }


    private Vector3 CreateRandomDungeonLocation()
    {
        int randX, randY;
        randX = Random.Range(0, structureOverlay.GetWitdth());
        randY = Random.Range(0, structureOverlay.GetHeight());

        return new Vector3(randX, randY, 0);

        while (true)
        {

        }

    }

    private void LeftCLicked(InputAction.CallbackContext obj)
    {
        int x, y;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 screenPoint = new Vector3(mousePos.x, mousePos.y, 2666); // 60 depends on the cams Z position
        Vector3 worldPos = GetMouseWorldPosition(Camera.main, screenPoint);
        noiseMapGrid.GetXY(worldPos, out x, out y);
        Debug.Log($"Mouse is at: {mousePos} and our worldpos is: {worldPos} and we are looking at {x} , {y}" );
        Debug.Log(noiseMapGrid.GetGridObject(worldPos));
        //grid.SetGridObject(worldPos, 43);

        level1 = SceneManager.CreateScene("level 1");

        //dlaGrid = dla.GenerateWalkable();       
    }




    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        return textMesh;
    }
    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left)
    {
        if (color == null) color = Color.white;
        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment);
    }





}
