using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MapTileGenerator map = new MapTileGenerator(5, 5, 10);
    }

}
