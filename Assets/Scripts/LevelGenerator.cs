using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            int x_abs = x;// Mathf.Abs(x);
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
            Vector3 tilePosition = startPos + direction * i * 10;
            GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
            tile.transform.position = tilePosition;
            tile.name = "Floor tile at " + tilePosition.ToString();
            tile.GetComponent<Renderer>().material.color = tileColor;
            tile.transform.SetParent(map.transform);
            tile.tag = "Floor tile";
        }
    }

    private void PathGenerator(int mainPathCorridorCount, int maxMainPathCorridorLength, int numberOfOffshootBranches, int branchCorridorCount)
    {
        List<int> mainPathCorridors = CorridorLister(mainPathCorridorCount, maxMainPathCorridorLength);
        GameObject theMap = new GameObject("The Map");
        Vector3 startPos = Vector3.zero;
        Vector3 absoluteStartPos = startPos;

        /* CREATE PATH */

        while (mainPathCorridors.Count != 0)
        {

            Debug.Log(startPos + "  " +absoluteStartPos);
            int corridorLength = ListPop(mainPathCorridors);
            Vector3 corridorDirection = RandomCardinalDirection();
            LayCorridorTiles(startPos, corridorDirection, corridorLength, theMap, Color.gray);
            startPos += corridorDirection * corridorLength * 10;
        }

        /* creating doors */

        //for (int doors = 0; doors < 10; doors++)
        //{
        //    int index = Random.Range(0, GameObject.FindGameObjectsWithTag("Floor tile").Length - 1);
        //    GameObject door = theMap.transform.GetChild(index).gameObject;
        //    door.GetComponent<Renderer>().material.color = Color.red;
        //    door.gameObject.name = "door";
        //    door.transform.position += new Vector3(0, .5f, 0);
        //}
        
    }

    
  








    private void Start()
    {

        PathGenerator(20,12, 0, 0);

        //for (int i = 0; i < 15; i++)
        //{
        //    Debug.Log(i + ". step is: " + NorthOrWest());
        //}


    }

}
