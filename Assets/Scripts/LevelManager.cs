using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager
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

    public LevelManager(int index, uint seed)
    {
        this.seed = seed;
        this.index = index;
    }

    public Grid<bool> GenerateLevel()
    {
        levelGrid = new Grid<bool>(gridWidth, gridHeight, cellSize, startPos, () => false);
        dla.SetGrid(levelGrid);
        dla.GenerateWalkable();

        return levelGrid;
    }


}
