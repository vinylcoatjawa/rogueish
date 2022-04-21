using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoiseUtils;

public class LevelGenerator : MonoBehaviour
{
    // 1. generate a walk

    // 3. create of branches from this walk NO NEED 

    // 4. place rectangles so that the path winds through them

    // 5. spice up room geometri with noise, probably Perlin noise



    /* returns a list of integers(x) to be used as x length straight corridor segments of a path */
    private List<int> CorridorLister(int numberOfCorridors, int maxCorridorLength)
    {
        List<int> stepList = new List<int>();
        for (int i = 0; i < numberOfCorridors; i++)
        {
            int curr = Random.Range(2, maxCorridorLength);
            stepList.Add(curr);
        }
        return stepList;
    }
    /* standard list popper, takes list pops last element */
    private int ListPop(List<int> list)
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
    }
    /* returns a unit vector pointing north or west*/
    private Vector3 RandomCardinalDirection()
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
    }
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
    private void LayCorridorTiles(Vector3 startPos, Vector3 direction, int steps, GameObject map, Color tileColor)
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
    }
    /* find the lower-left- and the upper-right-most tile  */
    private void GetMaxCorner(out Vector3 minCorner, out Vector3 maxCorner)
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

    }

    private void PathGenerator(int mainPathCorridorCount, int maxMainPathCorridorLength, int numberOfOffshootBranches, int branchCorridorCount)
    {
        List<int> mainPathCorridors = CorridorLister(mainPathCorridorCount, maxMainPathCorridorLength);
        GameObject theMap = new GameObject("The Map");
        Vector3 maxCorner = Vector3.zero;
        Vector3 minCorner = Vector3.zero;
        Vector3 startPos = Vector3.zero;
        //Vector3 absoluteStartPos = startPos; // may not be needed

        /* CREATE PATH */

        while (mainPathCorridors.Count != 0)
        {

            int corridorLength = ListPop(mainPathCorridors);
            Vector3 corridorDirection = RandomCardinalDirection();
            LayCorridorTiles(startPos, corridorDirection, corridorLength, theMap, Color.red);
            startPos += corridorDirection * corridorLength;
        }

        GetMaxCorner(out minCorner, out maxCorner);
        LayOuterWall(minCorner, maxCorner);

        //GameObject rand = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //rand.transform.position = GetRandomRoomCorner(minCorner, maxCorner);



    }



    private void LayOuterWall(Vector3 minCorner, Vector3 maxCorner)
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

    }




    /* MAY BE NEEDED */
    //private Vector3 GetWorldPosition(int x, int z, float cellSize)
    //{
    //    return new Vector3(x, 0, z) * cellSize;
    //}




    private void Start()
    {

        Noise noise = new Noise();

        //Debug.Log((uint)Mathf.RoundToInt((float)2 / 3));

        Debug.Log(uint.MaxValue);

        for (int i = 0; i < 10000; i++)
        {

            Debug.Log(noise.ZeroOrOne((uint)i, 36));

            //if(noise.Get1DNoiseZeroToOne((uint)i, 256) >= .5f)
            //{
            //Debug.Log(true);
            //}
            //else
            //{
            // Debug.Log(false);
            //}
        }

        
        PathGenerator(25,10, 0, 0);
        //GetMaxCorner();

        //for (int i = 0; i < 15; i++)
        //{
        //    Debug.Log(i + ". step is: " + NorthOrWest());
        //}


    }

}
