using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromHeightMap(Grid<float> heightMap)
    {
        int width = heightMap.GetWitdth();
        int height = heightMap.GetHeight();

        Texture2D texture = new Texture2D(width, height);

        Color[] colourMap = new Color[height * width];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap.GetGridObject(x, y));
            }
        }

        return TextureFromColourMap(colourMap, width, height);
    }
    
}
