using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class test : MonoBehaviour
{
    //MapTileGenerator map;
    LevelGenerator level;
    List<MapTile> mapTiles = new List<MapTile>();
    void Start()
    {
        level = new LevelGenerator(40, 40, 1695, 9);
        level.CreateFloorTiles();
        level.AssignRooms();
        level.GetOpenings();
        level.RemoveBadRooms();
        ShowLog(level.mapTiles);

    }

    private void ShowLog(List<MapTile> tilelist)
    {
        
        foreach (var maptile in tilelist)
        {

            GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tile.name = "Tile on: (" + maptile.tilePosition.x + ", " + maptile.tilePosition.z + " )";
            tile.transform.position = maptile.tilePosition;
            //Debug.Log(maptile.TilePosition + " is " + maptile.isWall);
            //tile.GetComponent<Renderer>().material.color = new Color(maptile.RoomNumber.x / 10, 0, maptile.RoomNumber.y / 10, 1);
            //if(maptile.TilePosition.y == 0) { tile.GetComponent<Renderer>().material.color = Color.gray; } else tile.GetComponent<Renderer>().material.color = Color.black;
            if(maptile.isOnRoomBorder && tile.transform.position.y == 0) { 
                tile.GetComponent<Renderer>().material.color = Color.red;
                tile.transform.position = new Vector3(tile.transform.position.x, 0, tile.transform.position.z);
            }
            else if (maptile.isOnRoomBorder && tile.transform.position.y != 0) { tile.GetComponent<Renderer>().material.color = Color.red; }
            //tile.GetComponent<Renderer>().material.color = new Color(maptile.RoomNumber.x / 10, 0, maptile.RoomNumber.y / 10, 1);
            //Debug.Log("Tile on: (" + maptile.TilePosition.x + ", " + maptile.TilePosition.z + " ) has room numbers: " + maptile.RoomNumber.x + ", " + maptile.RoomNumber.y + " )");

        }



    }
}
