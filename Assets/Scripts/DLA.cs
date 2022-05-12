using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DLA
{
    private Grid<bool> grid;

    public Grid<bool> GenerateDLAGrid(int width, int height, float cellSize, Vector3 originPosition)
    {
        grid = new Grid<bool>(width, height, cellSize, Vector3.zero, () => false);

        grid.SetGridObject(width / 2, height / 2, true);

        Debug.Log(grid.GetGridObject(5, 5));

        return grid;

    }
}
