using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoiseUtils;
using System;
using Random = UnityEngine.Random;

public class DLA
{
    private Grid<bool> grid;
    List<Vector2> walkableTiles = new List<Vector2>();
    Noise noise = new Noise();

    uint seed; 

    public void SetGrid(Grid<bool> grid)
    {
        this.grid = grid;
        //seed = (uint)Random.Range(0, uint.MaxValue);
    }
    public Grid<bool> GenerateWalkable()
    {        
        grid.SetGridObject(grid.GetWitdth() / 2, grid.GetHeight() / 2, true); // setting middle tile to walkable
        int xCoord = 0;
        int yCoord = 0;
        
        for (int i = 0; i < 400; i++)
        {
            Vector2 tileChoice;
            /* scanning the frid for tiles to add to walkables */
            for (int x = 0; x < grid.GetWitdth(); x++)
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {
                    if(grid.GetGridObject(x, y) == true && !walkableTiles.Contains(GridToVector2(x,y)) ) { walkableTiles.Add(GridToVector2(x, y)); } // adding to list of walkables so we can keep track of them
                }
            }
            tileChoice = Expand(i);
            Vector2ToGrid(tileChoice, out xCoord, out yCoord);
            grid.SetGridObject(xCoord, yCoord, true);
        }

        return grid;
    }

    private Vector2 Expand(int index)
    {
        Vector2 north, south, east, west;

        uint rollD4 = noise.NoiseRandomRange(0, 4, (uint)index * (uint)index, seed);

        Vector2 target = SelectRandomWalkableTile(index);
        Aggregate(target, out north, out south, out east, out west);

        if (rollD4 == 0) return north;
        else if (rollD4 == 1) return south;
        else if (rollD4 == 2) return east;
        else if (rollD4 == 3) return west;
        else return target;
    }

    private void Aggregate(Vector2 target, out Vector2 north, out Vector2 south, out Vector2 east, out Vector2 west)
    {       
        int targetX = (int)target.x;
        int targetY = (int)target.y;
        int minX = grid.GetWitdth();
        int minY = grid.GetHeight();
        int maxX = 0;
        int maxY = 0;
        // running through vertically ASSUMES GRID STARTS FROM ORIGO!!
        for (int y = 0; y < grid.GetHeight(); y++)
        {
            if (grid.GetGridObject(targetX, y) == true && y > maxY) { maxY = y; }
            if (grid.GetGridObject(targetX, y) == true && y < minY) { minY = y; }
        }
        // running through horisontally ASSUMES GRID STARTS FROM ORIGO!!
        for (int x = 0; x < grid.GetWitdth(); x++)
        {
            if (grid.GetGridObject(x, targetY) == true && x > maxX) { maxX = x; }
            if (grid.GetGridObject(x, targetY) == true && x < minX) { minX = x; }
        }
        if (maxY + 1 <= grid.GetHeight()) //  ASSUMES GRID STARTS FROM ORIGO!!
        {
            north = new Vector2(targetX, maxY + 1);
        } else north = new Vector2(targetX, maxY);

        if (minY - 1 >= 0) //  ASSUMES GRID STARTS FROM ORIGO!!
        {
            south = new Vector2(targetX, minY - 1);
        }
        else south = new Vector2(targetX, minY);

        if (maxX + 1 <= grid.GetWitdth()) //  ASSUMES GRID STARTS FROM ORIGO!!
        {
            east = new Vector2(maxX + 1, targetY);
        }
        else east = new Vector2(maxX, targetY);

        if (minX - 1 >= 0) //  ASSUMES GRID STARTS FROM ORIGO!!
        {
            west = new Vector2(minX - 1, targetY);
        }
        else west = new Vector2(minX, targetY);
    }
    private Vector2 SelectRandomWalkableTile(int index)
    {
        return walkableTiles[(int)noise.NoiseRandomRange(0, (uint)walkableTiles.Count, (uint)index, seed)];
    }

    private Vector2 GridToVector2(int x, int y)
    {
        return new Vector2(x, y);
    }

    private void Vector2ToGrid(Vector2 inputVector, out int x, out int y)
    {
        x = (int)inputVector.x;
        y = (int)inputVector.y;
    }

}
