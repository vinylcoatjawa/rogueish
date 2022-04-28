using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoiseUtils;
using UnityEngine.InputSystem;
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
        path.transform.SetParent(map.transform);

        GenerateLevel();
        //GenerateLevel_2();
    }
    public void GenerateLevel_2()
    {
        // put startin tile at 0,0
        SpawnTileAt(0, 0, "Path tile", "Path", path.transform, PrimitiveType.Cube, Color.red);
        // noise RNG number of corridors and number of tiles in each create dict maybe
        uint numberOfCorridors = 10 + (noise.Get2DNoiseUint(0, 0, levelSeed) % complexity);
        Dictionary<GameObject, Vector3> corridorDict = new Dictionary<GameObject, Vector3>();
        Debug.Log(numberOfCorridors);
        Physics.autoSimulation = false;
        for (int i = 0; i < 12; i++)
        {
            
            GameObject branchStartTile;
            Vector3 branchDirection;
            SelectRandomPathTileAndDirection(out branchStartTile, out branchDirection);
            Debug.Log("Star pos is: " + branchStartTile.transform.position + " and direction is " + branchDirection);
            LayCorridor(branchStartTile.transform.position, branchDirection, i, 10);
        }
        // SelectRandomPathTileAndDirection for these
        Physics.autoSimulation = true;
    }

    public void GenerateLevel()
    {
        Debug.Log(levelSeed);
        Physics.autoSimulation = false;
        CreatePathOnLevel(levelSeed);
        //LayMainPathBranch(Vector3.zero, 10, 10);
        Physics.autoSimulation = true;
    }

    private void CreatePathOnLevel(uint seed)
    {
        LayMainPathBranch(Vector3.zero, 10, 10);

        int numberOfOffshootBranches = 5;
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
        uint noiseBasedIndex;
        branchStartTile = pathTiles[0];
        branchDirection = Vector3.zero;
        uint probe = 0;
        bool goodEnough = true;
        
        List<string> cardinalDirectionWhitelist = new List<string>() { "North", "South", "West", "East" };

        List<string> diagonalDirectionWhitelist = new List<string>();

        while (goodEnough)
        {
            noiseBasedIndex = noise.Get1DNoiseUint((uint)numberOfPathTiles, levelSeed) % (uint)numberOfPathTiles;
            branchStartTile = pathTiles[noiseBasedIndex];

            foreach (var direction in cardinalDirections)
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
            foreach (var direction in diagonalDirections)
            {
                Ray ray = new Ray(branchStartTile.transform.position, direction.Value);
                Physics.Simulate(1f); // need to simulate physics to be able to raycast
                if (Physics.Raycast(branchStartTile.transform.position, direction.Value, out RaycastHit hitinfo, 1f))
                {
                    diagonalDirectionWhitelist.Add(direction.Key);
                }
            }
            for (int i = 0; i < cardinalDirectionWhitelist.Count; i++)
            {
                for (int j = 0; j < diagonalDirectionWhitelist.Count; j++)
                {
                    if (Vector3.Dot(cardinalDirections[cardinalDirectionWhitelist[i]], diagonalDirections[diagonalDirectionWhitelist[j]]) > 0)
                    {
                        cardinalDirectionWhitelist.Remove(cardinalDirectionWhitelist[i]);
                    }
                }
            }
            if (cardinalDirectionWhitelist.Count == 0)
            {
                probe += probe;
            }
            else
            {
                goodEnough = false;
            }
        }
        for (int i = 0; i < toActivateAgain.Count; i++)
        {
            toActivateAgain[i].SetActive(true);
        }
        Debug.Log(cardinalDirectionWhitelist[0]);
        branchDirection = cardinalDirections[cardinalDirectionWhitelist[0]];
        branchStartTile.GetComponent<Renderer>().material.color = Color.black;
    }




    private void LayOffshootBranch(int branch)
    {
        GameObject branchStartTile;
        Vector3 branchStartDirection;
        SelectRandomPathTileAndDirection(out branchStartTile, out branchStartDirection);
        Vector3 branchStartPos = branchStartTile.transform.position + branchStartDirection;
        LayCorridor(branchStartPos, branchStartDirection, branch, 8);
    }



    private void SpawnTileAt(int tilePosX, int tilePosZ, string tag, string name, Transform parent, PrimitiveType type, Color objectColor)
    {
        //if(!Physics.CheckBox(GridToWorld(tilePosX, tilePosZ), Vector3.one * tileSize))
        //{
        GameObject tile = GameObject.CreatePrimitive(type);
        tile.name = name + " tile at " + GridToWorld(tilePosX, tilePosZ).ToString();
        tile.tag = tag;
        tile.transform.position = GridToWorld(tilePosX, tilePosZ);
        tile.GetComponent<Renderer>().material.color = objectColor;
        tile.transform.SetParent(parent);
        //}
        //else { Debug.Log("Object collision at: " + GridToWorld(tilePosX, tilePosZ)); }
    }

    private Vector3 GridToWorld(int gridX, int gridZ)
    {
        return new Vector3(gridX, 0, gridZ) * tileSize;
    }


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
