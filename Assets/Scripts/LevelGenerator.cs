using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoiseUtils;
using UnityEngine.InputSystem;
using System;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    int size = 50;
    float tileSize = 1f;
    GameObject map;
    Vector3 lowerLeftCorner, upperRightCorner;

    public static Dictionary<int, Vector3> cardinalDirections = new Dictionary<int, Vector3>() // static dict to hold cardinal direction
    {
        { 0, Vector3.forward },
        { 1, Vector3.back },
        { 2, Vector3.left },
        { 3, Vector3.right }
    };
    private void Awake()
    {
        map = new GameObject("Map");
        
    }

    private void Start()
    {
        DLALevel();
        Debug.Log("dla done");
        Vector3 lowerLeftCorner, upperRightCorner;
        GetCornerGridTiles(out lowerLeftCorner, out upperRightCorner);
        Debug.Log(lowerLeftCorner + " as lowe left and " + upperRightCorner + " as upper right");
    }


    IEnumerator DLAAnimation()
    {
        for (int i = 0; i < 4500; i++)
        {
            ScanFromBorder(size);
            //yield return new WaitForSeconds(0.000001f);
            yield return null;
        }
        SmoothTheMap();
        GetCornerGridTiles(out lowerLeftCorner, out upperRightCorner);
    }
    private void DLALevel()
    {
        InitCubes();
        //StartCoroutine(DLAAnimation());
        GameObject[] floorTiles = GameObject.FindGameObjectsWithTag("Floor tile");
        while (floorTiles.Length < 1000)
        {
            ScanFromBorder(size);
            floorTiles = GameObject.FindGameObjectsWithTag("Floor tile");
        }

    }
    private void InitCubes()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject init = GameObject.CreatePrimitive(PrimitiveType.Cube); // may change to generic object
            init.transform.position = new Vector3(0, 0, i);
            init.GetComponent<Renderer>().material.color = Color.black;
        }
    }
    private void ScanFromBorder(int size)
    {
        //pick direction to choose boder to cast from
        int floatingCoordinate = Mathf.RoundToInt(Random.Range(-size, size));
        Vector3 chosenPosition;
        Vector3 chosenDirection;

        int choice = Mathf.RoundToInt(Random.Range(0, 4));

        if (choice == 0)
        {
            chosenPosition = new Vector3(floatingCoordinate, 0, -size);
            chosenDirection = cardinalDirections[choice];
            TryToPlace(chosenPosition, chosenDirection);
        }
        else if (choice == 1)
        {
            chosenPosition = new Vector3(floatingCoordinate, 0, size);
            chosenDirection = cardinalDirections[choice];
            TryToPlace(chosenPosition, chosenDirection);
        }
        else if (choice == 2)
        {
            chosenPosition = new Vector3(size, 0, floatingCoordinate);
            chosenDirection = cardinalDirections[choice];
            TryToPlace(chosenPosition, chosenDirection);
        }
        else if (choice == 3)
        {
            chosenPosition = new Vector3(-size, 0, floatingCoordinate);
            chosenDirection = cardinalDirections[choice];
            TryToPlace(chosenPosition, chosenDirection);
        }
    }
    private void TryToPlace(Vector3 startPos, Vector3 direction)
    {

        Ray ray = new Ray(startPos, direction);

        Vector3 stickLocation;

        if (Physics.Raycast(ray, out RaycastHit hitinfo))
        {
            //Debug.DrawRay(startPos, direction * 150, Color.green, 0.1f);
            stickLocation = hitinfo.collider.transform.position + (startPos - hitinfo.point).normalized * tileSize;
            //Debug.Log("Hit at " + hitinfo.collider.transform.position + " from " + startPos + " and stick location is " + stickLocation);

            GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            particle.GetComponent<Renderer>().material.color = Color.black;
            particle.transform.position = stickLocation;
            particle.transform.SetParent(map.transform);
            particle.tag = "Floor tile";
        }
        //else { Debug.DrawRay(startPos, direction * 150, Color.red,0.1f); }
    }
    private int GetNeighbourCount(Vector3 position)
    {
        int neighbourCount = 0;
        foreach (var direction in cardinalDirections)
        {
            Ray ray = new Ray(position, direction.Value);
            if (Physics.Raycast(ray, out RaycastHit hitinfo, 1f))
            {
                neighbourCount++;
            }
        }
        return neighbourCount;
    }
    private void SmoothTheMap()
    {
        for (int x = -size; x < size; x++)
        {
            for (int z = -size; z < size; z++)
            {
                Vector3 position = new Vector3(x, 0, z);
                if (!Physics.CheckSphere(position, 0.01f))
                {
                    if (GetNeighbourCount(position) > 2)
                    {
                        GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        particle.GetComponent<Renderer>().material.color = Color.black;
                        particle.transform.position = position;
                        particle.transform.SetParent(map.transform);
                        particle.tag = "Floor tile";
                    }

                }
            }
        }
    }
    private void GetCornerGridTiles( out Vector3 lowerLeftCorner, out Vector3 upperRightCorner)
    {
        Vector3 minCorner = Vector3.zero;
        Vector3 maxCorner = Vector3.zero;
        GameObject[] floorTiles = GameObject.FindGameObjectsWithTag("Floor tile");
        foreach (var floorTile in floorTiles)
        {
            if (floorTile.transform.position.x <= minCorner.x) { minCorner.x = (int)floorTile.transform.position.x; }
            if (floorTile.transform.position.z <= minCorner.z) { minCorner.z = (int)floorTile.transform.position.z; }

            if (floorTile.transform.position.x > maxCorner.x) { maxCorner.x = (int)floorTile.transform.position.x; }
            if (floorTile.transform.position.z > maxCorner.z) { maxCorner.z = (int)floorTile.transform.position.z; }
        }
        lowerLeftCorner = new Vector3(minCorner.x, 0, minCorner.z);
        upperRightCorner = new Vector3(maxCorner.x, 0, maxCorner.z);
        
    }
}
