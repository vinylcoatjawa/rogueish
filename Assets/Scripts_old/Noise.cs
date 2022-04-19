using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapWitdh, int mapHeight, float scale)
    {
        float[,] noiseMap = new float[mapWitdh, mapHeight];

        if (scale <= 0) { scale = 0.0001f; };

        for (int x = 0; x < mapWitdh; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                float sampleX = x / scale;
                float sampleZ = z / scale;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ);
                noiseMap[x, z] = perlinValue;
            }
        }

        return noiseMap;
    }

}
