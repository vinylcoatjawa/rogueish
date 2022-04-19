using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;




/*public class LevelGenerator
{ 
    
    private int levelWidth, levelHeight, seed, roomSideLength, roomX, roomZ;
    public List<MapTile> mapTiles = new List<MapTile>(); // tiles will be collected into this to enable easy loop through
    private HashSet<Vector2> rooms = new HashSet<Vector2>();
    public LevelGenerator(int levelWidth, int levelHeight, int seed, int roomSideLength)
    {
        this.levelWidth = levelWidth;
        this.levelHeight = levelHeight;
        this.seed = seed;
        this.roomSideLength = roomSideLength;
    }

    public void CreateFloorTiles()
    {
        MapTile mapTile;

        for (int x = -this.levelWidth; x < this.levelWidth; x++)
        {
            for (int z = -this.levelHeight; z < this.levelHeight; z++)
            {

                float cubeDistFromOrigo = MathF.Sqrt(x * x + z * z) / 150; // the farther away the cube is the less likely for it to be walkable, should be dynamic ie dependant on this.levelHeight and this.roomsdidelength
                bool isWall = GetPerlinDepth(x, z, this.seed) > (.68f - cubeDistFromOrigo); // these numbers are not dynamic and will work properly with map size 40,40 and room size 9
                Vector3 tilePostition = new Vector3(x, isWall ? 1 :0, z); //raise up the ones having isWall flag
                mapTile = new MapTile(tilePostition);

                mapTile.isWall = isWall;
                mapTiles.Add(mapTile);
            }
        }
    }
    private float GetPerlinDepth(int x, int z, int seed)
    {
        float xNoise = (float)x * Mathf.PI + seed;
        float zNoise = (float)z * Mathf.PI + seed;
        return Mathf.PerlinNoise(xNoise, zNoise);
    }

    public void AssignRooms()
    {
        
        foreach (var mapTile in mapTiles)
        {
            float tileXCoord = mapTile.tilePosition.x;
            float tileZCoord = mapTile.tilePosition.z;

            tileXCoord += this.levelWidth;
            tileZCoord += this.levelHeight;

            int roomXCoord = (int)Mathf.Floor(tileXCoord / this.roomSideLength);
            int roomZCoord = (int)Mathf.Floor(tileZCoord / this.roomSideLength);

            mapTile.roomNumber = new Vector2(roomXCoord, roomZCoord); // room named
            rooms.Add(mapTile.roomNumber); // room enlisted
        }
    }



    public void GetOpenings()
    {
        foreach (var mapTile in mapTiles)
        {
            MapTile east = GetTileByPosition(mapTile.tilePosition + new Vector3(1, 0, 0));
            MapTile north = GetTileByPosition(mapTile.tilePosition + new Vector3(0, 0, 1));

            if (north != null && mapTile.roomNumber != north.roomNumber && !mapTile.isWall|| east != null && mapTile.roomNumber != east.roomNumber && !mapTile.isWall) 
            {
                mapTile.isOnRoomBorder = true;
            }
        }
    }
    public MapTile GetTileByPosition(Vector3 position)
    {
        MapTile theTile;

        foreach (var i in mapTiles)
        {
            if (i.tilePosition == position)
            {
                theTile = i;
                return theTile;
            }
            
        }
        return null;
    }

    public void RemoveBadRooms()
    {
        GetFloorWallProportionPerRoom();
        
        
    }

    private void GetFloorWallProportionPerRoom()
    {
        int numberOfFloors;
        int numberOfWalls;
        float proportion;
        foreach (var room in rooms)
        {
            numberOfFloors = 0;
            numberOfWalls = 0;
            proportion = 0;
            foreach (var mapTile in mapTiles)
            {
                if (mapTile.isWall && mapTile.roomNumber == room) { numberOfWalls++; } else if (!mapTile.isWall && mapTile.roomNumber == room) numberOfFloors++;
            }
            proportion = (float)numberOfWalls / numberOfFloors;
            foreach (var mapTile in mapTiles)
            {
                if (proportion > 1f && mapTile.roomNumber == room && mapTile.tilePosition.y == 0)
                {
                    mapTile.tilePosition.y += 1;
                }
            }
        }
    }
}



public class MapTile
{
    public Vector3 tilePosition;
    public Vector2 roomNumber;
    public bool isWall;
    public bool isOnRoomBorder;
    public bool badRoom;
    public MapTile(Vector3 tilePosition)
    {
        this.tilePosition = tilePosition;
    }

}*/



















/*public class MapTileGenerator
{
    private int tileHeigth, tileWidth, roomX, roomZ;
    private float cellSize, gridOffset;
    private Vector3 originPosition;


    public MapTileGenerator(int width, int heigth, float cellSize, Vector3 originPosition)
    {
        this.tileWidth = width;
        this.tileHeigth = heigth;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        gridOffset = cellSize * .95f;
        int roomSize = 10;



        

        for (int x = -width; x < width + 1; x++)
        {
            for (int z = -heigth; z < heigth + 1; z++)
            {
                int blockYValue;
                //if ( -1 < x  ) Debug.Log("if i: " + x + " and j: " + z + " then the noise is : " + GetPerlinDepth(x, z, 0.66f));
                if (GetPerlinDepth(x, z, 159654) > .5f) { blockYValue = 1; } else blockYValue = 0; // needs logic to smooth this, also add logic to make it harsher the farther we are from origin
                Vector3 tilePosition = GetWorldCoords(x , blockYValue, z) + new Vector3(cellSize / 2, 0, cellSize / 2);
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tile.name = "Cube at " + "(" + x + ", " + z + " )";
                tile.transform.localScale = new Vector3(gridOffset, gridOffset, gridOffset);
                tile.transform.position = tilePosition;
                roomX = ((x + width) / roomSize);
                roomZ = ((z + heigth) / roomSize);
                Debug.Log("Tile: (" + x + ", " + z + ") is in room " + "(" + roomX + ", " + roomZ + ")");
            }
        }

        for (int x = -(width + roomSize / 2); x < width; x += roomSize)
        {
            for (int z = -(heigth + roomSize / 2); z < heigth; z += roomSize)
            {
                // gets to middle of the room
                

                GameObject northWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                northWall.name = "North Wall";
                northWall.transform.localScale = new Vector3(roomSize, 0.01f, 3) * cellSize;
                Vector3 northWallPos = GetWorldCoords(x, 0, z + roomSize / 2);
                Vector3 northWallRot = new Vector3(90, 0, 0);
                northWall.transform.position = northWallPos;
                northWall.transform.eulerAngles = northWallRot;
                northWall.GetComponent<Renderer>().material.color = Color.blue;

                //Debug.Log(x + " and " + z);
                //Vector3 northWallPos = GetWorldCoords(x, 5, z + roomSize / 2);
                //Vector3 northWallRot = new Vector3(0, 0, 90);
                //GameObject northWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //northWall.transform.position = northWallPos;
                //northWall.transform.eulerAngles = northWallRot;
                //northWall.GetComponent<Renderer>().material.color = Color.blue;

                //GameObject middle = new GameObject("Middle of: (" + x + ", " + z + " )");
                //middle.transform.position = roomMiddle;


                
                //Vector3 tilePosition = GetWorldCoords(x, 5, z);
                //Vector3 tileRotation = new Vector3(90, 0, 0);
                //GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //tile.transform.localScale = new Vector3(gridOffset, gridOffset, gridOffset);
                //tile.transform.position = tilePosition;
                //tile.transform.eulerAngles = tileRotation;
                //tile.GetComponent<Renderer>().material.color = Color.red;
                break;
            } break;
        } 


    }

    private float GetPerlinDepth(int x, int z, int seed)
    {
        
        float xNoise = (float)x * Mathf.PI + seed;
        float zNoise = (float)z * Mathf.PI + seed;
        Debug.Log("For (" + x + ", " + z + ") we get the noise input x: " + xNoise + " z: " + zNoise + " and perlin noise for these is " + Mathf.PerlinNoise(xNoise, zNoise));


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

    private void 

}*/
