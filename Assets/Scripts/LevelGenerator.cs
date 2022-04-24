using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoiseUtils;
//using System;

public class LevelGenerator : MonoBehaviour
{

    /* 
    TERMINOLOGY:
            - Corridor: straight bit of path
    */


    Noise noise = new Noise();
    GameObject map;
    GameObject path;
    int[,] mapGrid;
    int gridMinX, gridMinZ, gridMaxX, gridMaxZ;

    private void Awake()
    {
        map = new GameObject("The Map");
        path = new GameObject("The Path");
        path.transform.SetParent(map.transform);
        mapGrid = new int[100, 100]; // may not be needed

    }

    private void LayPathBranch(Vector3 pathStartPos, uint minCorridorCount, uint maxCorridorCountDeviation, uint seed)  // parameters may be superflous but this function may be used for sidebranches as well
    {
        uint startX = (uint)pathStartPos.x; // these will be used in the noise map
        uint startZ = (uint)pathStartPos.z; // these will be used in the noise map
        Vector3 corridorStartPos = pathStartPos;
        uint numberOfCorridors = minCorridorCount + (noise.Get2DNoiseUint(startX, startZ, seed) % maxCorridorCountDeviation); // calculate number of straight corridors to include in the path
        for (int corridor = 0; corridor < numberOfCorridors; corridor++) // looping through the number of corridors to create them
        {
            Vector3 direction = ChooseDirection(corridorStartPos, corridor, seed); // getting random direction for next corridor bit
            corridorStartPos = LayCorridor(corridorStartPos, direction, corridor, seed); // LayCorridor takes in the postition of the start tile and returns the position of the end tile         
        }
        
    }

    private Vector3 ChooseDirection(Vector3 corridorStartPos, int corridor, uint seed)
    {
        uint startX = (uint)corridorStartPos.x; // these will be used in the noise map
        uint startZ = (uint)corridorStartPos.z; // these will be used in the noise map

        uint probe = noise.Get1DNoiseUint((uint)corridor, seed); // we need to adjust the direction calculation in each while loop try
        int directionX = 0; // setting initial direction
        int directionZ = 0; // setting initial direction this will satisfy the while loop so it iterates through once at least

        while (directionX == directionZ)
        {
            uint xComponent = startX + (uint)corridor * (uint)corridor + (uint)probe;
            uint zComponent = startZ + (uint)corridor * (uint)corridor + ((uint)probe * 2);
        
            directionX = (int)noise.ZeroOrOne(xComponent, seed);
            directionZ = (int)noise.ZeroOrOne(zComponent, seed);

            probe += probe; // probing value is shifted so next iteration of while will have different values
        }
        return new Vector3(directionX, 0, directionZ);

    }

    private Vector3 LayCorridor(Vector3 corridorStartPos, Vector3 corridorDirection, int corridor, uint seed)
    {
        Vector3 nextTile = corridorStartPos;
        uint startX = (uint)nextTile.x;
        uint startZ = (uint)nextTile.z;
        int directionX = (int)corridorDirection.x;
        int directionZ = (int)corridorDirection.z;
        uint numberOfCorridorTiles = 2 + (noise.Get2DNoiseUint(startX + 1 +(uint)corridor * (uint)corridor, startZ + 1 + (uint)corridor * (uint)corridor, seed) % 8); // minsteps is 2 so we at least make 2 steps and 8 + 2 at most
        //Debug.Log("Handling corridr: " + corridor + " which will have " + numberOfCorridorTiles + " floor tiles");
        for (int step = 0; step < numberOfCorridorTiles; step++)
        {
            int gridX = (int)startX + step * directionX;
            int gridZ = (int)startZ + step * directionZ;

            //Debug.Log("Putting tile on (" + gridX + ", " + gridZ + " )");

            Vector3 tilePos = new Vector3(gridX, 0, gridZ);

            SpawnObjectAt(tilePos, Color.red, path.transform);

            //mapGrid[gridX, gridZ] = 1; // updating gridvalue to be the "floor value" which is 1

            /* saving the grids lover left and upper right corner */
            //if (gridX > gridMaxX) { gridMaxX = gridX; }
            //if (gridX < gridMinX) { gridMinX = gridX; }
            //if (gridZ > gridMaxZ) { gridMaxZ = gridZ; }
            //if (gridZ < gridMinZ) { gridMinZ = gridZ; } 

        }
        corridorStartPos = nextTile + numberOfCorridorTiles * new Vector3(directionX, 0, directionZ);
        return corridorStartPos;
    }


    private int[,] WorldPosToMapgrid(Vector3 worldPosition)
    {
        int x = (int)worldPosition.x;
        int z = (int)worldPosition.z;
        int[,] res = new int[x, z];
        return res;
    }


    private Vector3 SelectRandomPathTile(uint seed)
    {
        GameObject[] pathTiles = GameObject.FindGameObjectsWithTag("Path tile");
        int numberOfPathTiles = pathTiles.Length;
        uint noiseBasedIndex = noise.Get1DNoiseUint((uint)numberOfPathTiles, seed) % (uint)numberOfPathTiles;
        pathTiles[noiseBasedIndex].GetComponent<Renderer>().material.color = Color.black;
        return pathTiles[noiseBasedIndex].transform.position;
    }

    private void SelectViableOffshootDirection(Vector3 tilePosition)
    {
        Dictionary <string , Vector3> directions = new Dictionary<string, Vector3>();
        directions.Add("Forward", Vector3.forward);
        directions.Add("Back", Vector3.back);
        directions.Add("Left", Vector3.left);
        directions.Add("Right", Vector3.right);

        foreach (var direction in directions)
        {
            if(Physics.Raycast(tilePosition, direction.Value))
            {
                Debug.Log("hit");
            }
        }

        //for (int x = - 1; x < 2; x++)
        //{
        //    for (int z = - 1; z < 2; z++)
        //    {
        //        Vector3 direction = new Vector3(x, 0, z);
        //        //Debug.Log("Casting in the direction: " + direction + " from " + tilePosition);
        //        Ray ray = new Ray(tilePosition, direction);
        //        //Debug.DrawRay(tilePosition, direction, Color.blue, 10);
        //        //Debug.Log(Physics.Raycast(tilePosition, direction, out RaycastHit hitinfo));

        //        if (Physics.Raycast(ray, out RaycastHit hit))
        //        {
        //            Debug.Log("hit");
        //            Debug.Log(hit.transform.gameObject.name + " was hit in direction: " + direction);
        //            Debug.DrawLine(ray.origin, hit.point, Color.blue, 10f);
        //        }
        //        else
        //        {
        //            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100, Color.green);
        //        }
        //    }  
        //}
        
        
    }
    Vector3 test;
    private void Start()
    {
        Debug.Log("Forward: " + Vector3.forward);
        Debug.Log("Back: " + Vector3.back);
        Debug.Log("Left: " + Vector3.left);
        Debug.Log("Right: " + Vector3.right);

        uint seed = (uint)Random.Range(1, uint.MaxValue);
        
        Debug.Log(seed);

        LayPathBranch(Vector3.zero, 10, 10, seed);
        test = SelectRandomPathTile(seed);
        SelectViableOffshootDirection(test);

        //CreatePathOnLevel(seed);

    }

    private void Update()
    {
        //SelectViableOffshootDirection(test);
    }

    private void CreatePathOnLevel(uint seed)
    {
        /* generate main path */
        LayPathBranch(Vector3.zero, 10, 10, seed);
        // get offshoot branch number using noise and the seed
        int numberOfOffshootBranches = 1;
        for (int branch = 0; branch < numberOfOffshootBranches; branch++)
        {
            //SelectRandomPathTile();
            //SelectViableOffshootDirection();
            //LayPathBranch();
        }


    }

    private void SpawnObjectAt(Vector3 objectPosition, Color objectColor, Transform parent)
    {
        GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tile.name = "Path tile at " + objectPosition.ToString();
        tile.tag = "Path tile";
        tile.transform.position = objectPosition;
        tile.GetComponent<Renderer>().material.color = objectColor;
        tile.transform.SetParent(parent);
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
