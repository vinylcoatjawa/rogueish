using UnityEngine;
using TMPro;
using System;

public class MapTileGenerator
{
    private int tileHeigth, tileWidth;
    private float cellSize, gridOffset;
    int[,] cells;


    public MapTileGenerator(int width, int heigth, float cellSize)
    {
        this.tileWidth = width;
        this.tileHeigth = heigth;
        this.cellSize = cellSize;
        gridOffset = cellSize / 2;

        cells = new int[width, heigth];

        for (int i = -width; i < width; i++)
        {
            for (int j = -heigth; j < heigth; j++)
            {
                Vector3 tilePosition = GetWorldCoords(i, j);
                Vector3 tileRotation = new Vector3(90, 0, 0);
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tile.transform.localScale = new Vector3(cellSize - 1 / cellSize, cellSize - 1 / cellSize, cellSize - 1 / cellSize);
                tile.transform.position = tilePosition;
                tile.transform.eulerAngles = tileRotation;

                Debug.Log("if i: " + i + " and j: " + j + " then the noise is : " + GetPerlinDepth(i, j, 0.66f));

                // add text 
                //TextMesh tileText = tile.AddComponent<TextMesh>();
                //tileText.color = Color.red;
                //tileText.text = "(" + i + ", " + j + ")";

                //gridSquare.transform.position = GetWorldCoords(i, j);
                //gridSquare.transform.eulerAngles = new Vector3(90, 0, 0);

                //GameObject gridSquare = new GameObject("Grid square at: (" + i + ",  " + j + " )", typeof(TextMesh));

                //Vector3 gridSquareRot = gridSquare.transform.rotation.eulerAngles;

                //TextMesh text = gridSquare.GetComponent<TextMesh>();
                //text.color = Color.red;
                // text.text = "(" + i + ", " + j + ")";
                //Debug.DrawLine(GetWorldCoords(i, j) + new Vector3(cellSize, 0, cellSize) * .5f, GetWorldCoords(i, j + 1) + new Vector3(cellSize, 0, cellSize) * .5f, Color.blue, 100f);
                //Debug.DrawLine(GetWorldCoords(i, j) + new Vector3(cellSize, 0, cellSize) * .5f, GetWorldCoords(i + 1, j) + new Vector3(cellSize, 0, cellSize) * .5f, Color.blue, 100f);
            }
        }
        //Debug.DrawLine(GetWorldCoords(width, heigth) + new Vector3(cellSize, 0, cellSize) * .5f, GetWorldCoords(width, heigth) + new Vector3(cellSize, 0, cellSize) * .5f, Color.blue, 100f);
        //Debug.DrawLine(GetWorldCoords(width, 0) + new Vector3(cellSize, 0, cellSize) * .5f, GetWorldCoords(width, heigth) + new Vector3(cellSize, 0, cellSize) * .5f, Color.blue, 100f);

    }

    private float GetPerlinDepth(int x, int z, float detailScale)
    {
        float xNoise = x / detailScale;
        float zNoise = z / detailScale;
        Debug.Log(xNoise);
        return Mathf.PerlinNoise(xNoise, zNoise);
    }

    private Vector3 GetWorldCoords(int x, int z, int y = 0)
    {
        return new Vector3(x, y, z) * cellSize;
    }

}
