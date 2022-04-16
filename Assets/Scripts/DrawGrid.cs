using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DrawGrid : MonoBehaviour
{
    int[,] cells = new int[20, 20];
    void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                GameObject grid = new GameObject("Game Object at: (" + i + ",  " + j + " )", typeof(TextMesh));
                TextMesh text = grid.GetComponent<TextMesh>();
                text.color = Color.red;
                text.text = "hmm";
                
       
            }
        }
    }

    
}
