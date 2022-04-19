using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    // 1. generate a grid of size X, Z

    // 2. select two far away points and connect them with a "random walk"

    // 3. create of branches from this walk

    // 4. place rectangles so that the path winds through them

    // 5. spice up room geometri with noise, probably Perlin noise


    int maxStepsize = 10;
    
    private void PathGenerator(int forward, int right)
    {
        List<int> forwardList = StepLister(forward);
        List<int> rightList = StepLister(right);
        GameObject theMap = new GameObject("The Map");


        Vector3 startPos = Vector3.zero;
        int totalListCount = forwardList.Count + rightList.Count;

        while(totalListCount != 0)
        {
            int choice = Random.Range(0, 2);
            if (choice == 1)
            {
                int stepsToTake = ListPop(forwardList);
                Vector3 stepsTaken = Vector3.forward * stepsToTake * 10;
                Tiler(startPos, Vector3.forward, stepsToTake, theMap);
                startPos += stepsTaken;
                totalListCount = forwardList.Count + rightList.Count;
            }
            else if (choice == 0)
            {
                int stepsToTake = ListPop(rightList);
                Vector3 stepsTaken = Vector3.right * stepsToTake * 10;
                Tiler(startPos, Vector3.right, stepsToTake, theMap);
                startPos += stepsTaken;
                totalListCount = forwardList.Count + rightList.Count;
            }
            Debug.Log("Left are: " + totalListCount + " steps and the current choice was " + choice);
        }
    }
    
    private void Tiler(Vector3 startPos, Vector3 direction, int steps, GameObject map)
    {
        
        for (int i = 0; i < steps; i++)
        {
            GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
            tile.transform.position = startPos + direction * i * 10;
            tile.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            tile.transform.SetParent(map.transform);
        }
    }

    private List<int> StepLister(int steps)
    {
        List<int> stepList = new List<int>();
        for (int i = 0; i < steps; i++)
        {
            int curr = Random.Range(1, maxStepsize);
            stepList.Add(curr);
        }

        return stepList;
    }

    private int ListPop(List<int> stepList)
    {
        int popped;
        int listLength = stepList.Count;
        if (listLength != 0)
        {
            popped = stepList[listLength - 1];
            stepList.RemoveAt(listLength - 1);
            return popped;
        }
        else return 0;
    }
    
    
    
   


    private void Start()
    {
        //IntegerPrtition(80, 4);

        PathGenerator(5,5);

   

        //Tiler(Vector3.zero, Vector3.right, 10);

        //Debug.Log(Vector3.forward);
        //Debug.Log(Vector3.right);
    }

}
