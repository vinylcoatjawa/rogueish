using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRender;

    public void DrawNoiseMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int heigth = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, heigth);

        Color[] colourMap = new Color[width * heigth];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < heigth; z++)
            {
                float ize = z * width + x;

                //Debug.Log("hi");
                //Debug.Log("For x: " + x + " and z : " + z + " we get PN of : " + noiseMap[x,z] + " and ize : " + ize);

                Color col;
                if (noiseMap[x, z] > 0.5) { col = Color.black; } else col = Color.white;
                colourMap[z * width + x] = col;
                //colourMap[z * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, z]);
                //int[,] coords = new int[x, z]; 
                //Debug.Log("For " + coords + " the PN is " + noiseMap[x, z]);


            }
        }
        texture.SetPixels(colourMap);
        texture.Apply();

        textureRender.sharedMaterial.mainTexture = texture;

        textureRender.transform.localScale = new Vector3(width, 1, heigth);
    }

    
    
}
