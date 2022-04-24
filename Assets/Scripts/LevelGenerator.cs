using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoiseUtils;
//using System;

public class LevelGenerator : MonoBehaviour
{
    // 1. generate a walk

    // 3. create of branches from this walk NO NEED 

    // 4. place rectangles so that the path winds through them

    // 5. spice up room geometri with noise, probably Perlin noise

    /* TERMINOLOGY:
            - Corridor: straight bit of path
    */
    //public uint seed = 12890;
    Noise noise = new Noise();
    GameObject map;
    int[,] mapGrid;
    int gridMinX, gridMinZ, gridMaxX, gridMaxZ;

    private void Awake()
    {
        map = new GameObject("The Map");
        mapGrid = new int[100, 100];

    }

    private void LayPathBranch(Vector3 pathStartPos, uint minCorridorCount, uint maxCorridorCountDeviation, uint seed)  // parameters may be superflous but this function may be used for sidebranches as well
    {
        uint startX = (uint)pathStartPos.x;
        uint startZ = (uint)pathStartPos.z;
        Vector3 corridorStartPos = pathStartPos;
        uint numberOfCorridors = minCorridorCount + (noise.Get2DNoiseUint(startX, startZ, seed) % maxCorridorCountDeviation); // calculate number of straight corridors to include in the path
        for (int corridor = 0; corridor < numberOfCorridors; corridor++) // looping through the number of corridors to create them
        {
            Vector3 direction = ChooseDirection(corridorStartPos, corridor, seed);
            //Debug.Log("Number of corridors: " + numberOfCorridors + " we are currently on corridor nr: " + (corridor + 1) + " and the chose direstion is: " + direction);
            //Debug.Log(direction);
            //Vector3 direction = new Vector3(1, 0, 0);
            LayCorridor(corridorStartPos, direction, corridor, seed);
            
        }
        
    }

    private Vector3 ChooseDirection(Vector3 corridorStartPos, int corridor, uint seed)
    {
        uint startX = (uint)corridorStartPos.x; 
        uint startZ = (uint)corridorStartPos.z;
        //Vector3 nextTile = corridorStartPos;

        
        uint probe = noise.Get1DNoiseUint((uint)corridor, seed); // we need to adjust the direction calculation in each while loop try
        int directionX = 0; // setting initial direction
        int directionZ = 0;

        while (directionX == directionZ)
        {
            uint xComponent = startX + (uint)corridor * (uint)corridor + (uint)probe;
            uint zComponent = startZ + (uint)corridor * (uint)corridor + ((uint)probe * 2);
        
            directionX = (int)noise.ZeroOrOne(xComponent, seed);
            directionZ = (int)noise.ZeroOrOne(zComponent, seed);

            probe += probe;
        }
        return new Vector3(directionX, 0, directionZ);

    }

    private void LayCorridor(Vector3 corridorStartPos, Vector3 corridorDirection, int corridor, uint seed)
    {
        Vector3 nextTile = corridorStartPos;
        int startX = (int)nextTile.x;
        int startZ = (int)nextTile.z;
        int directionX = (int)corridorDirection.x;
        int directionZ = (int)corridorDirection.z;
        uint numberOfCorridorTiles = 2 + (noise.Get2DNoiseUint((uint)startX + (uint)corridor * (uint)corridor, (uint)startZ + (uint)corridor * (uint)corridor, seed) % 8); // minsteps is 2 so we at least make 2 steps and 8 + 2 at most
        Debug.Log("Handling corridr: " + corridor + " which will have " + numberOfCorridorTiles + " floor tiles");
        for (int step = 0; step < numberOfCorridorTiles; step++)
        {
            int gridX = startX + step * directionX;
            int gridZ = startZ + step * directionZ;
            mapGrid[gridX, gridZ] = 0;
            if (gridX > gridMaxX) { gridMaxX = gridX; }
            if (gridX < gridMinX) { gridMinX = gridX; }
            if (gridZ > gridMaxZ) { gridMaxZ = gridZ; }
            if (gridZ < gridMinZ) { gridMinZ = gridZ; }
            Debug.Log(gridMaxX + " " + gridMinX + " " + gridMaxZ + " " + gridMinZ + " value " + mapGrid[gridX, gridZ]);
        }
        //corridorStartPos = nextTile + numberOfCorridorTiles * new Vector3(directionX, 0, directionZ); 

    }


    private int[,] WorldPosToMapgrid(Vector3 worldPosition)
    {
        int x = (int)worldPosition.x;
        int z = (int)worldPosition.z;
        int[,] res = new int[x, z];
        return res;
    }


    void OnDrawGizmos()
    {
        if (mapGrid != null)
        {
            for (int x = gridMinX; x < gridMaxX; x++)
            {
                for (int y = gridMinZ; y < gridMaxZ; y++)
                {
                    Gizmos.color = (mapGrid[x, y] == 1) ? Color.black : Color.blue;
                    Vector3 pos = new Vector3(x, 0, y);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }






    }

    private void Start()
    {
        LayPathBranch(Vector3.zero, 5, 5, (uint)Random.Range(1, uint.MaxValue));
        //ChooseDirection(Vector3.zero, 6);
    }




    /* returns a list of integers(x) to be used as x length straight corridor segments of a path */
    /*private List<int> CorridorLister(int numberOfCorridors, int maxCorridorLength)
    {
        List<int> stepList = new List<int>();
        for (int i = 0; i < numberOfCorridors; i++)
        {
            int curr = Random.Range(2, maxCorridorLength);
            stepList.Add(curr);
        }
        return stepList;
    }*/
    /* standard list popper, takes list pops last element */
    /*private int ListPop(List<int> list)
    {
        int popped;
        int listLength = list.Count;
        if (listLength != 0)
        {
            popped = list[listLength - 1];
            list.RemoveAt(listLength - 1);
            return popped;
        }
        else return 0;
    }*/
    /* returns a unit vector pointing north or west*/
    /*private Vector3 RandomCardinalDirection()
    {
        while (true)
        {
            float yRot = Random.rotationUniform.eulerAngles.y;
            int x = Mathf.RoundToInt(Mathf.Cos(yRot));
            int z = Mathf.RoundToInt(Mathf.Sin(yRot));

            int x_abs = x;// Mathf.Abs(x); uncomment for movement only in north-east direction
            int z_abs = z;// Mathf.Abs(z);

            if (Mathf.Abs(x + z) == 1)
            {
                return new Vector3(x_abs, 0, z_abs);
            }
        }
    }*/
    /* returns a unit vector pointing south or east MAY NOT BE NEEDED*/
    /*private Vector3 SouthOrEast()
    {
        while (true)
        {
            float yRot = Random.rotationUniform.eulerAngles.y;
            int x = Mathf.RoundToInt(Mathf.Cos(yRot));
            int z = Mathf.RoundToInt(Mathf.Sin(yRot));

            int x_abs = -Mathf.Abs(x);
            int z_abs = -Mathf.Abs(z);

            if (Mathf.Abs(x + z) == 1)
            {
                return new Vector3(x_abs, 0, z_abs);
            }
        }
    }*/
    /* lays floor tiles for a straight corridor section */
    /*private void LayCorridorTiles(Vector3 startPos, Vector3 direction, int steps, GameObject map, Color tileColor)
    {
        for (int i = 0; i < steps; i++)
        {
            Vector3 tilePosition = startPos + direction * i;
            GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
            tile.transform.position = tilePosition;
            tile.name = "Floor tile at " + tilePosition.ToString();
            tile.GetComponent<Renderer>().material.color = tileColor;
            tile.transform.localScale = new Vector3(0.1f, 0, 0.1f);
            tile.transform.SetParent(map.transform);
            tile.tag = "Floor tile";
            // maybe destroy the ones existing the position where we wann insert before inserting
        }
    }*/
    /* find the lower-left- and the upper-right-most tile  */
    /*private void GetMaxCorner(out Vector3 minCorner, out Vector3 maxCorner)
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Floor tile");
        Vector3 max = Vector3.zero;
        Vector3 min = Vector3.zero;
        foreach (var tile in tiles)
        {
            if (tile.transform.position.x >= max.x) { max.x = tile.transform.position.x; }
            if (tile.transform.position.z >= max.z) { max.z = tile.transform.position.z; }
            if (tile.transform.position.x <= min.x) { min.x = tile.transform.position.x; }
            if (tile.transform.position.z <= min.z) { min.z = tile.transform.position.z; }
        }
        maxCorner = max;
        minCorner = min;

    }*/

    /*private void PathGenerator(int mainPathCorridorCount, int maxMainPathCorridorLength, int numberOfOffshootBranches, int branchCorridorCount)
    {
        List<int> mainPathCorridors = CorridorLister(mainPathCorridorCount, maxMainPathCorridorLength);
        GameObject theMap = new GameObject("The Map");
        Vector3 maxCorner = Vector3.zero;
        Vector3 minCorner = Vector3.zero;
        Vector3 startPos = Vector3.zero;
        //Vector3 absoluteStartPos = startPos; // may not be needed*/

    /* CREATE PATH */

    /*while (mainPathCorridors.Count != 0)
    {

        int corridorLength = ListPop(mainPathCorridors);
        Vector3 corridorDirection = RandomCardinalDirection();
        LayCorridorTiles(startPos, corridorDirection, corridorLength, theMap, Color.red);
        startPos += corridorDirection * corridorLength;
    }

    GetMaxCorner(out minCorner, out maxCorner);
    LayOuterWall(minCorner, maxCorner);*/

    //GameObject rand = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //rand.transform.position = GetRandomRoomCorner(minCorner, maxCorner);







    /*private void LayOuterWall(Vector3 minCorner, Vector3 maxCorner)
    {
        int offset = 5;
        int verticalLength = (int)Mathf.Abs(minCorner.z - maxCorner.z);
        int horizontalLength = (int)Mathf.Abs(minCorner.x - maxCorner.x);
        GameObject walls = new GameObject("Walls");

        Vector3 wallMinCorner = minCorner - new Vector3(offset, 0, offset);
        Vector3 wallMaxCorner = maxCorner + new Vector3(offset, 0, offset);

        for (int i = 0; i < (verticalLength + 2 * offset) + 1; i++)
        {
            GameObject wallSegment_1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject wallSegment_2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallSegment_1.GetComponent<Renderer>().material.color = Color.red;
            wallSegment_2.GetComponent<Renderer>().material.color = Color.red;
            wallSegment_1.transform.position = new Vector3(wallMinCorner.x, 0, wallMinCorner.z) + i * (Vector3.forward);
            wallSegment_2.transform.position = new Vector3(wallMaxCorner.x, 0, wallMinCorner.z) + i * (Vector3.forward);
            wallSegment_1.transform.SetParent(walls.transform);
            wallSegment_2.transform.SetParent(walls.transform);
        }

        for (int i = 1; i < (horizontalLength + 2 * offset); i++)
        {
            GameObject wallSegment_1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject wallSegment_2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallSegment_1.GetComponent<Renderer>().material.color = Color.red;
            wallSegment_2.GetComponent<Renderer>().material.color = Color.red;
            wallSegment_1.transform.position = new Vector3(wallMinCorner.x, 0, wallMinCorner.z) + i * (Vector3.right);
            wallSegment_2.transform.position = new Vector3(wallMinCorner.x, 0, wallMaxCorner.z) + i * (Vector3.right);
            wallSegment_1.transform.SetParent(walls.transform);
            wallSegment_2.transform.SetParent(walls.transform);
        }
    }

    private Vector3 GetRandomRoomCorner(Vector3 minCorner, Vector3 maxCorner)
    {
        int randX = Random.Range((int)minCorner.x, (int)maxCorner.x);
        int randZ = Random.Range((int)minCorner.z, (int)maxCorner.z);

        return new Vector3(randX, 0, randZ);
    }

    private void RayCastInCardinalDirection(Vector3 objectPosition, Vector3 cardinalDirection)
    {

    }*/





}
