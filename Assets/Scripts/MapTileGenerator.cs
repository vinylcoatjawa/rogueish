using UnityEngine;
using TMPro;
using System;

public class MapTileGenerator
{
    private int tileHeigth, tileWidth;
    private float cellSize, gridOffset;
    private Vector3 originPosition;
    //int[,] cells;


    public MapTileGenerator(int width, int heigth, float cellSize, Vector3 originPosition)
    {
        this.tileWidth = width;
        this.tileHeigth = heigth;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        gridOffset = cellSize - 1 / cellSize;
        int roomSize = 10;

        //cells = new int[width, heigth];

        for (int x = -width; x < width + 1; x++)
        {
            for (int z = -heigth; z < heigth + 1; z++)
            {
                int blockYValue;
                if (GetPerlinDepth(x, z, 0.66f) > .6f) { blockYValue = 1; } else blockYValue = 0; // needs logic to smooth this, also add logic to make it harsher the farther we are from origin
                Vector3 tilePosition = GetWorldCoords(x, blockYValue, z);
                Vector3 tileRotation = new Vector3(90, 0, 0);
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tile.transform.localScale = new Vector3(gridOffset, gridOffset, gridOffset);
                tile.transform.position = tilePosition;
                tile.transform.eulerAngles = tileRotation;

                Debug.Log("if i: " + x + " and j: " + z + " then the noise is : " + GetPerlinDepth(x, z, 0.66f));
            }
        }

        for (int x = -width; x < width; x += roomSize)
        {
            for (int z = -heigth; z < heigth; z += roomSize)
            {
                Vector3 tilePosition = GetWorldCoords(x, 5, z);
                Vector3 tileRotation = new Vector3(90, 0, 0);
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tile.transform.localScale = new Vector3(gridOffset, gridOffset, gridOffset);
                tile.transform.position = tilePosition;
                tile.transform.eulerAngles = tileRotation;
                tile.GetComponent<Renderer>().material.color = Color.red;
            }
        }


    }

    private float GetPerlinDepth(int x, int z, float detailScale)
    {
        float xNoise = x / detailScale;
        float zNoise = z / detailScale;
        return Mathf.PerlinNoise(xNoise, zNoise);
    }

    private Vector3 GetWorldCoords(int x, int y, int z)
    {
        return new Vector3(x, y, z) * cellSize + originPosition;
    }

    private void GetXY(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    public void GenerateWalls(int width, int height, int roomSize, Vector3 originPosition)
    {
        int levelWidth = 2 * width + 1;
        int levelHeight = 2 * height + 1;

        for (int x = -width; x < width; x += roomSize)
        {
            for (int z = -height; z < height; z += roomSize)
            {
                Vector3 tilePosition = GetWorldCoords(x, 5, z);
                Vector3 tileRotation = new Vector3(90, 0, 0);
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tile.transform.localScale = new Vector3(gridOffset, gridOffset, gridOffset);
                tile.transform.position = tilePosition;
                tile.transform.eulerAngles = tileRotation;
                tile.GetComponent<Renderer>().material.color = Color.red;
            }
        }



    }

}
