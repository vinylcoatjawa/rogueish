using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class DLA : MonoBehaviour
{
    int size;
    float tileSize = 1f;
    GameObject map;
    public static Dictionary<int, Vector3> cardinalDirections = new Dictionary<int, Vector3>() // static dict to hold cardinal direction
    {
        { 0, Vector3.forward },
        { 1, Vector3.back },
        { 2, Vector3.left },
        { 3, Vector3.right }
    };
    
    
    PlayerInputActions inputAction;


    private void Awake()
    {
        inputAction = new PlayerInputActions();
        inputAction.Mouse.Enable();
        inputAction.Mouse.LeftClick.started += LeftClicked;
        inputAction.Mouse.RightClick.started += RightClicked;
        map = new GameObject("Map");
    }

    private void RightClicked(InputAction.CallbackContext obj)
    {
        SmoothTheMap();
    }

    //private void Start()
    //{

    //    size = 100;

    //    GameboardInit();

    //    GameObject init = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //    init.transform.position = Vector3.zero;
    //    init.GetComponent<Renderer>().material.color = Color.blue;
    //    GameObject init_1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //    init_1.transform.position = Vector3.zero + new Vector3(1, 0, 0);
    //    init_1.GetComponent<Renderer>().material.color = Color.blue;
    //    GameObject init_2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //    init_2.transform.position = Vector3.zero + new Vector3(0, 0, 1);
    //    init_2.GetComponent<Renderer>().material.color = Color.blue;
    //}

    private void GameboardInit()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject init = GameObject.CreatePrimitive(PrimitiveType.Cube);
        }
    }

    private void LeftClicked(InputAction.CallbackContext obj)
    {
        //SpawnParticle();
        StartCoroutine(DLAAnimation());
    }


    IEnumerator DLAAnimation()
    {
        for (int i = 0; i < 6000; i++)
        {
            //SpawnParticle();
            ScanFromBorder(80);
            yield return new WaitForSeconds(0.001f);

        }
    }

    private Vector3  GetRandomCardinalDirection()
    {
        int choice = Mathf.RoundToInt(Random.Range(0, 3));
        return cardinalDirections[choice];
    }

    private Vector3 SpawnAtRandom(int width, int height)
    {
        int x, z;
        x = Mathf.RoundToInt(Random.Range(-width, width + 1));
        z = Mathf.RoundToInt(Random.Range(-height, height + 1));

        return new Vector3(x, 0, z);

    }

    private void SpawGameTile(Vector3 spawnPosition)
    {
        GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        particle.GetComponent<Renderer>().material.color = Color.black;
        particle.transform.position = spawnPosition;
        particle.transform.SetParent(map.transform);

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
        Vector3 minmaxX = Vector3.zero;
        Vector3 minmaxZ = Vector3.zero;
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Floor tile");

        //foreach (var tile in tiles)
        //{
        //    {
        //        if (tile.transform.position.x <= minmaxX.x) { minmaxX.x = (int)tile.transform.position.x; }
        //        if (tile.transform.position.z <= minmaxZ.z) { minmaxZ.z = (int)tile.transform.position.z; }

        //        if (tile.transform.position.x > minmaxX.x) { minmaxX.x = (int)tile.transform.position.x; }
        //        if (tile.transform.position.z > minmaxZ.z) { minmaxZ.z = (int)tile.transform.position.z; }
        //    }
        //}

        Debug.Log(minmaxX + " as x and " + minmaxZ + " as z");

        
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




}




