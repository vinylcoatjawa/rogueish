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
        uint randomSeed = 1480729344;// GetRandomSeed();
        LevelGenerator level = new LevelGenerator(randomSeed);


        
    }

    
    private uint GetRandomSeed()
    {
        return (uint)Random.Range(1, uint.MaxValue); // 3363111936; 3687977984;//
    }

    //private void MarchTheNoiseMap(delegate )
    //{

    //}


    
}
