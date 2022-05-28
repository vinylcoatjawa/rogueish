using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager : MonoBehaviour
{
    // read from dung dict and DLA generate the level

    int gridWidth = 50;
    int gridHeight = 50;
    float cellSize = 2f;
    Vector3 startPos = Vector3.zero;
    Grid<bool> levelGrid;
    DLA dla = new DLA();

    /* got from dungeon manager */
    uint seed;
    int index;

    //public LevelManager(int index, uint seed)
    //{
    //    this.seed = seed;
    //    this.index = index;
    //}

    public Grid<bool> GenerateLevel()
    {
        levelGrid = new Grid<bool>(gridWidth, gridHeight, cellSize, startPos, () => false);
        dla.SetGrid(levelGrid);
        dla.GenerateWalkable();
        return levelGrid;
    }


    public void LayTiles(Grid<bool> levelGrid)
    {
        GameObject tile = Resources.Load<GameObject>("Assets/Resources/Dungeon/Walkable Floor.prefab");
        for (int x = 0; x < levelGrid.GetWitdth(); x++)
        {
            for (int y = 0; y < levelGrid.GetHeight(); y++)
            {
                if (levelGrid.GetGridObject(x, y))
                {

                }
            }
        }
    }

}
