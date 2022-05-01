using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoiseUtils;
using UnityEngine.InputSystem;
using System;
//using System;

public class LevelGenerator
{

    /* 
    TERMINOLOGY:
            - Corridor: straight bit of path
    */
    uint levelSeed;
    uint complexity;
    Noise noise = new Noise();
    GameObject map;
    GameObject path;
    GameObject walls;

    public static Dictionary<string, Vector3> cardinalDirections = new Dictionary<string, Vector3>() // static dict to hold cardinal direction
    {
        { "North", Vector3.forward },
        { "South", Vector3.back },
        { "West", Vector3.left },
        { "East", Vector3.right }
    };

    public static Dictionary<string, Vector3> diagonalDirections = new Dictionary<string, Vector3>() // static dict to hold cardinal direction
    {
        { "North-East" , new Vector3(1, 0, 1) },
        { "South-East", new Vector3(1, 0, -1) },
        { "South-West", new Vector3(-1, 0, -1) },
        { "North-West", new Vector3(-1, 0, 1) }
    };

    float tileSize = 1f;



    public LevelGenerator(uint levelSeed, uint complexity)
    {
        this.levelSeed = levelSeed;
        this.complexity = complexity;

        map = new GameObject("The Map");
        path = new GameObject("The Path");
        walls = new GameObject("The Wall");
        path.transform.SetParent(map.transform);
        walls.transform.SetParent(map.transform);

        GenerateLevel();
        
    }
    

    public void GenerateLevel()
    {
        Physics.autoSimulation = false;
        CreatePathOnLevel(levelSeed);
        BuildWalls();
        Physics.autoSimulation = true;
    }

    #region GENERAL UTIL

    private void SpawnTileAt(int tilePosX, int tilePosZ, string tag, string name, Transform parent, PrimitiveType type, Color objectColor)
    {
        Physics.Simulate(1f);
        if (!Physics.CheckSphere(GridToWorld(tilePosX, tilePosZ), 0.1f))
        {
            GameObject tile = GameObject.CreatePrimitive(type);
            tile.name = name + " tile at " + GridToWorld(tilePosX, tilePosZ).ToString();
            tile.tag = tag;
            tile.transform.position = GridToWorld(tilePosX, tilePosZ);
            tile.GetComponent<Renderer>().material.color = objectColor;
            tile.transform.SetParent(parent);
        }
        else { Debug.Log("Object collision at: " + GridToWorld(tilePosX, tilePosZ)); }
    }

    private Vector3 GridToWorld(int gridX, int gridZ)
    {
        return new Vector3(gridX, 0, gridZ) * tileSize;
    }

    private void ReactivateTiles(List<GameObject> gameObjectsToActivate)
    {
        for (int i = 0; i < gameObjectsToActivate.Count; i++)
        {
            gameObjectsToActivate[i].SetActive(true);
        }
    }



    #endregion


    #region PATH

    private void CreatePathOnLevel(uint seed)
    {
        LayMainPathBranch(Vector3.zero, 10, 10); // currently these are hardcoded, since they seem to work well and give a good main path

        int numberOfOffshootBranches = 6; // this is also hardcoded for now

        for (int branch = 0; branch < numberOfOffshootBranches; branch++)
        {
            LayOffshootBranch(branch);
        }
    }



    private void LayMainPathBranch(Vector3 pathStartPos, uint minCorridorCount, uint maxCorridorCountDeviation)  // parameters may be superflous but this function may be used for sidebranches as well
    {
        uint startX = (uint)pathStartPos.x; // these will be used in the noise map
        uint startZ = (uint)pathStartPos.z; // these will be used in the noise map
        Vector3 corridorStartPos = pathStartPos;
        uint numberOfCorridors = minCorridorCount + (noise.Get2DNoiseUint(startX, startZ, levelSeed) % maxCorridorCountDeviation); // calculate number of straight corridors to include in the path
        for (int corridor = 0; corridor < numberOfCorridors; corridor++) // looping through the number of corridors to create them
        {
            Vector3 direction = ChooseDirection(corridorStartPos, corridor); // getting random direction for next corridor bit
            corridorStartPos = LayCorridor(corridorStartPos, direction, corridor, 2); // LayCorridor takes in the postition of the start tile and returns the position of the end tile         
        }
    }

    private Vector3 ChooseDirection(Vector3 corridorStartPos, int corridor)
    {
        uint startX = (uint)corridorStartPos.x; // these will be used in the noise map
        uint startZ = (uint)corridorStartPos.z; // these will be used in the noise map

        uint probe = noise.Get1DNoiseUint((uint)corridor, levelSeed); // we need to adjust the direction calculation in each while loop try

        int directionX = 0; // setting initial direction
        int directionZ = 0; // setting initial direction this will satisfy the while loop so it iterates through once at least

        while (directionX == directionZ)
        {
            uint xComponent = startX + (uint)corridor * (uint)corridor + (uint)probe;
            uint zComponent = startZ + (uint)corridor * (uint)corridor + ((uint)probe * 2);

            directionX = (int)noise.ZeroOrOne(xComponent, levelSeed);
            directionZ = (int)noise.ZeroOrOne(zComponent, levelSeed);

            probe += probe; // probing value is shifted so next iteration of while will have different values
        }
        return new Vector3(directionX, 0, directionZ);
    }

    private Vector3 LayCorridor(Vector3 corridorStartPos, Vector3 corridorDirection, int corridor, int minLength) // want to pass in min length so we can use this for main and offbranches with flexibility 
    {
        Vector3 nextTile = corridorStartPos;
        uint startX = (uint)nextTile.x;
        uint startZ = (uint)nextTile.z;
        int directionX = (int)corridorDirection.x;
        int directionZ = (int)corridorDirection.z;
        uint numberOfCorridorTiles = (uint)minLength + (noise.Get2DNoiseUint(startX + 1 + (uint)corridor * (uint)corridor, startZ + 1 + (uint)corridor * (uint)corridor, levelSeed) % 8); // minsteps is 2 so we at least make 2 steps and 8 + 2 at most
        for (int step = 0; step < numberOfCorridorTiles; step++)
        {
            int gridX = (int)startX + step * directionX;
            int gridZ = (int)startZ + step * directionZ;
            SpawnTileAt(gridX, gridZ, "Path tile", "Path", path.transform, PrimitiveType.Cube, Color.red);
        }
        corridorStartPos = nextTile + numberOfCorridorTiles * new Vector3(directionX, 0, directionZ);
        return corridorStartPos;
    }


    private void SelectRandomPathTileAndDirection(out GameObject branchStartTile, out Vector3 branchDirection) // really ugly but works well
    {
        GameObject[] pathTiles = GameObject.FindGameObjectsWithTag("Path tile");
        List<GameObject> toActivateAgain = new List<GameObject>();
        int numberOfPathTiles = pathTiles.Length;
        branchStartTile = pathTiles[0];
        branchDirection = Vector3.zero;
        uint probe = 0;
        bool goodEnough = false;
        
        
        List<string> cardinalDirectionlist = new List<string>() { "North", "South", "West", "East" };
        List<string> cardinalDirectionWhitelist = new List<string>();
        List<string> diagonalDirectionBlacklist = new List<string>();

        while (!goodEnough)
        {
            cardinalDirectionWhitelist = new List<string>(cardinalDirectionlist);
            diagonalDirectionBlacklist.Clear();
            uint noiseBasedIndex = noise.Get1DNoiseUint((uint)numberOfPathTiles + probe, levelSeed) % (uint)numberOfPathTiles;
            branchStartTile = pathTiles[noiseBasedIndex];
            foreach (var direction in cardinalDirections) // scanning in the caridnal directions and removing obstructed directions, also turning off hit pathtiles
            {
                Ray ray = new Ray(branchStartTile.transform.position, direction.Value);
                Physics.Simulate(1f); // need to simulate physics to be able to raycast
                if (Physics.Raycast(branchStartTile.transform.position, direction.Value, out RaycastHit hitinfo, 1f))
                {
                    cardinalDirectionWhitelist.Remove(direction.Key);
                    toActivateAgain.Add(hitinfo.collider.gameObject);
                    hitinfo.collider.gameObject.SetActive(false);
                }
            }
            foreach (var direction in diagonalDirections) // scanning in the diagonal directions
            {
                Ray ray = new Ray(branchStartTile.transform.position, direction.Value);
                Physics.Simulate(1f); // need to simulate physics to be able to raycast
                if (Physics.Raycast(branchStartTile.transform.position, direction.Value, out RaycastHit hitinfo, 1f))
                {
                    diagonalDirectionBlacklist.Add(direction.Key);
                }
            }
            if (diagonalDirectionBlacklist.Count  >= 3) // if we hit more than 2 diagonals then this tile is a bad chioce
            {  
                probe++;
                ReactivateTiles(toActivateAgain);
                continue;
            }
            else
            {
                for (int i = 0; i < cardinalDirectionWhitelist.Count; i++) // we loop through the
                {
                    for (int j = 0; j < diagonalDirectionBlacklist.Count; j++)
                    {
                        if (Vector3.Dot(cardinalDirections[cardinalDirectionWhitelist[i]], diagonalDirections[diagonalDirectionBlacklist[j]]) > 0)
                        {
                            cardinalDirectionWhitelist.Remove(cardinalDirectionWhitelist[i]);
                        }
                    }
                }
            }
            if (cardinalDirectionWhitelist.Count == 0)
            {              
                probe++;          
                ReactivateTiles(toActivateAgain);
                continue;
            }
            else
            {
                goodEnough = true;
            }
        }
        ReactivateTiles(toActivateAgain);
        Debug.Log(cardinalDirectionWhitelist[0]);
        branchDirection = cardinalDirections[cardinalDirectionWhitelist[0]];
        //branchStartTile.GetComponent<Renderer>().material.color = Color.black;
    }




    private void LayOffshootBranch(int branch)
    {
        GameObject branchStartTile;
        Vector3 branchStartDirection;
        SelectRandomPathTileAndDirection(out branchStartTile, out branchStartDirection);
        Vector3 branchStartPos = branchStartTile.transform.position + branchStartDirection;
        LayCorridor(branchStartPos, branchStartDirection, branch, 8);
    }

    #endregion


    #region WALLS

    Vector3 minCorner = Vector3.zero;
    Vector3 maxCorner = Vector3.zero;



    private void BuildWalls()
    {
        Vector3 pathMinCorner;
        Vector3 pathMaxCorner;
        GetMinMaxCorners(out pathMinCorner, out pathMaxCorner);
        LayOuterWall(pathMinCorner, pathMaxCorner);
        // get corners
        // build outer walls
        // pick random point at least 2 away from path
        // pick 3 or 4 random cardinal directions to eject walls towards nearest wall
    }

    private void GetMinMaxCorners(out Vector3 pathMinCorner, out Vector3 pathMaxCorner)
    {
        pathMinCorner = Vector3.zero;
        pathMaxCorner = Vector3.zero;
        GameObject[] pathTiles = GameObject.FindGameObjectsWithTag("Path tile");
        foreach (var pathTile in pathTiles)
        {
            if (pathTile.transform.position.x <= pathMinCorner.x) { pathMinCorner.x = (int)pathTile.transform.position.x; }
            if (pathTile.transform.position.z <= pathMinCorner.z) { pathMinCorner.z = (int)pathTile.transform.position.z; }

            if (pathTile.transform.position.x > pathMaxCorner.x) { pathMaxCorner.x = (int)pathTile.transform.position.x; }
            if (pathTile.transform.position.z > pathMaxCorner.z) { pathMaxCorner.z = (int)pathTile.transform.position.z; }
        }
        
        pathMinCorner = new Vector3(pathMinCorner.x, 0, pathMinCorner.z);
        pathMaxCorner = new Vector3(pathMaxCorner.x, 0, pathMaxCorner.z);
        Debug.Log("Min is: " + pathMinCorner + " max is: " + pathMaxCorner);
    }

    private void LayOuterWall(Vector3 minCorner, Vector3 maxCorner)
    {
        int offset = 5;
        int verticalLength = (int)Mathf.Abs(minCorner.z - maxCorner.z);
        int horizontalLength = (int)Mathf.Abs(minCorner.x - maxCorner.x);
        //GameObject walls = new GameObject("Walls");
        Vector3 wallMinCorner = minCorner - new Vector3(offset, 0, offset);
        Vector3 wallMaxCorner = maxCorner + new Vector3(offset, 0, offset);

        //horisontal

        LayWall(wallMinCorner, wallMinCorner + new Vector3(wallMaxCorner.x, 0, 0));
        Debug.Log("Start: " + wallMinCorner + " abd end " + (wallMinCorner + new Vector3(wallMaxCorner.x, 0, 0)));


        //for (int i = 0; i < (verticalLength + 2 * offset) + 1; i++)
        //{
        //    GameObject wallSegment_1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    GameObject wallSegment_2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    wallSegment_1.GetComponent<Renderer>().material.color = Color.red;
        //    wallSegment_2.GetComponent<Renderer>().material.color = Color.red;
        //    wallSegment_1.transform.position = new Vector3(wallMinCorner.x, 0, wallMinCorner.z) + i * (Vector3.forward);
        //    wallSegment_2.transform.position = new Vector3(wallMaxCorner.x, 0, wallMinCorner.z) + i * (Vector3.forward);
        //    wallSegment_1.transform.SetParent(walls.transform);
        //    wallSegment_2.transform.SetParent(walls.transform);
        //}
        //for (int i = 1; i < (horizontalLength + 2 * offset); i++)
        //{
        //    GameObject wallSegment_1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    GameObject wallSegment_2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    wallSegment_1.GetComponent<Renderer>().material.color = Color.red;
        //    wallSegment_2.GetComponent<Renderer>().material.color = Color.red;
        //    wallSegment_1.transform.position = new Vector3(wallMinCorner.x, 0, wallMinCorner.z) + i * (Vector3.right);
        //    wallSegment_2.transform.position = new Vector3(wallMinCorner.x, 0, wallMaxCorner.z) + i * (Vector3.right);
        //    wallSegment_1.transform.SetParent(walls.transform);
        //    wallSegment_2.transform.SetParent(walls.transform);
        //}
    }
   
    private void LayWall(Vector3 startPos, Vector3 endPos)
    {
        float wallLength;
        if (startPos.x != endPos.x && startPos.z != endPos.z) { Debug.Log("Start and end are not in line for wall building"); }
        else if (startPos.z != endPos.z) { Debug.Log("Only vertical wall is possible"); }
        else if (startPos.x != endPos.x) { Debug.Log("Only horisontal wall possible"); }
            
        if (startPos.x == endPos.x)
        {
            wallLength = startPos.z - endPos.z;
            for (int z = 0; z < wallLength; z++)
            {
                SpawnTileAt((int)startPos.x, (int)startPos.z, "Wall tile", "Wall", walls.transform, PrimitiveType.Cube, Color.grey);
            }
        }
        if (startPos.z == endPos.z)
        {
            wallLength = startPos.x - endPos.x;
            for (int x = 0; x < wallLength; x++)
            {
                SpawnTileAt((int)startPos.x, (int)startPos.z, "Wall tile", "Wall", walls.transform, PrimitiveType.Cube, Color.grey);
            }
        }
    }





    #endregion
























    /*  BAD SEEDS
     * 621337600 - has parallel corridors next to eachother
     * 3796206592 - this one too
     * 2208472320 has cross section
     */

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
