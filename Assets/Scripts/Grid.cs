using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Grid
{
    int width;
    int height;
    float cellSize;
    int[,] gridArray;
    TextMesh[,] debugTestArray;

    public Grid(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridArray = new int[width, height];
        debugTestArray = new TextMesh[width, height];
        Vector3 middleOffset = new Vector3(cellSize, cellSize, 0) * 0.5f;

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                debugTestArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y].ToString(), null, GetWorldPosition(x, y) + middleOffset, 20, Color.black, TextAnchor.MiddleCenter);

                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.black, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.black, 100f);
            }
        }

        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.black, 100f);

       
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y, 0) * cellSize;
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPosition.x / cellSize);
        y = Mathf.FloorToInt(worldPosition.y / cellSize);
    }


    public void SetValue(int x, int y, int value)
    {
        if(x >= 0 && y>= 0 && x< width && y< height)
        {
            gridArray[x, y] = value;
            debugTestArray[x, y].text = gridArray[x, y].ToString();
        }
    }

    public void SetValue(Vector3 wordPosition, int value)
    {
        int x, y;
        GetXY(wordPosition, out x, out y);
        SetValue(x, y, value);
    }

    public int GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return 0;
        }
    }

    public int GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return GetValue(x, y);
        }
        else
        {
            return 0;
        }
    }

}
