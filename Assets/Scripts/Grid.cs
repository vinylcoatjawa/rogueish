using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;


public class Grid<TGridObject>
{
    int width;
    int height;
    float cellSize;
    Vector3 originPosition;
    TGridObject[,] gridArray;
    TextMesh[,] debugTestArray;
    

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<TGridObject> createGridObject)
    {
        /* setting instance parameters */
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        Vector3 middleOffset = new Vector3(cellSize, cellSize, 0) * 0.5f;
        
        /* initializing array with default objects */
        gridArray = new TGridObject[width, height];
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObject();
            }
        }

    #region DEBUG-ARRAY
        debugTestArray = new TextMesh[width, height];
        bool allowDebug = true;
        if (allowDebug)
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    debugTestArray[x, y] = CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + middleOffset, 20, Color.black, TextAnchor.MiddleCenter);

                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.black, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.black, 100f);
                }
            }

            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.black, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.black, 100f);
        }

    }
    #endregion


    public Vector3 GetWorldPosition(int x, int y) // convert from grid space to world space
    {
        return new Vector3(x, y, 0) * cellSize + originPosition;
    }
    public void SetGridObject(int x, int y, TGridObject value) // setting object on grid position
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            debugTestArray[x, y].text = gridArray[x, y].ToString();
        }
    }
    public TGridObject GetGridObject(int x, int y) // getting object from grid position
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(TGridObject);
        }
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y) // convert from world space to grid space
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }
    public void SetGridObject(Vector3 wordPosition, TGridObject value) // setting object in world space
    {
        int x, y;
        GetXY(wordPosition, out x, out y);
        SetGridObject(x, y, value);
    }
    public TGridObject GetGridObject(Vector3 worldPosition) // getting grid object from world position
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return GetGridObject(x, y);
        }
        else
        {
            return default(TGridObject);
        }
    }

    /* various grid related util functions */
    public int GetWitdth()
    {
        return gridArray.GetLength(0);
    }
    public int GetHeight()
    {
        return gridArray.GetLength(1);
    }
    public float GetCellSize()
    {
        return cellSize;
    }




    #region UTILS


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
    #endregion

}