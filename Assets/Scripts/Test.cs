using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    float tileSize = .5f;

    int[,]  tileGrid;

    // Start is called before the first frame update
    void Start()
    {
        uint randomSeed = GetRandomSeed();//2896306688;// GetRandomSeed();4073934336,1617799936,2814833152
        Debug.Log(randomSeed);
        LevelGenerator level = new LevelGenerator(randomSeed, 10);

        //GameObject test = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //test.transform.position = new Vector3(3, 0, 0);
        //test.name = "testing overwrite";
        //test.GetComponent<Renderer>().material.color = Color.blue;
        //Physics.autoSimulation = false;
        //Physics.Simulate(10f);
        //for (int i = 0; i < 6; i++)
        //{
            
        //    Vector3 pos = new Vector3(i, 0, 0);
        //    if(Physics.CheckSphere(pos, 0.1f))
        //    {
        //        Debug.Log("Hitting something at " + pos);
        //    }
        //    else
        //    {
        //    GameObject path = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    path.transform.position = new Vector3(i, 0, 0);
        //    path.GetComponent<Renderer>().material.color = Color.red;
        //    path.name = i.ToString();
        //    }
        //}
        //Physics.autoSimulation = true;
    }

    
    private uint GetRandomSeed()
    {
        return (uint)Random.Range(1, uint.MaxValue); // 3363111936; 3687977984;//
    }

    


    
}
