using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoiseUtils;
using System;

public static class PerlinMap
{
    public static Grid<float> GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity, uint seed, Vector2 offset)
    {
        Grid<float> noiseMap = new Grid<float>(mapWidth, mapHeight, 10, Vector3.zero, () => 0f);
        Noise noise = new Noise();

        Vector2[] octavesOffsets = new Vector2[octaves];
        uint noiseDisturbance = 123;
        for (int octave = 0; octave < octaves; octave++)
        {
            int offsetX = (int)noise.NoiseRandomRange(0, 20000, (uint)octave * noiseDisturbance + noiseDisturbance, seed) - 10000 + (int)offset.x;
            int offsetY = (int)noise.NoiseRandomRange(0, 20000, (uint)octave * noiseDisturbance + 2 * noiseDisturbance, seed) - 10000 + (int)offset.y;
            octavesOffsets[octave] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0) { scale = 0.0001f; }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                
                for (int octave = 0; octave < octaves; octave++)
                {
                    float sampleX = (x - halfWidth)/ scale * frequency + octavesOffsets[octave].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octavesOffsets[octave].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if (noiseHeight > maxNoiseHeight) { maxNoiseHeight = noiseHeight; }
                else if (noiseHeight < minNoiseHeight) { minNoiseHeight = noiseHeight; }
                noiseMap.SetGridObject(x, y, noiseHeight);
            }
        }
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                noiseMap.SetGridObject(x, y, Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap.GetGridObject(x, y)));
            }
        }

        noiseMap =  SmoothTheEdges(noiseMap, mapWidth, mapHeight);

        return noiseMap;
    }

    private static Grid<float> SmoothTheEdges(Grid<float> inputNoiseMap, int mapWidth, int mapHeight)
    {
        Vector3 mapMiddle = new Vector3(mapWidth / 2, mapHeight / 2, 0) * 10;
        float circleRadius = (mapWidth / 2) * .8f * 10;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3 currentCell = inputNoiseMap.GetWorldPosition(x, y);
                float distance = Vector3.Distance(mapMiddle, currentCell);
                if (distance >= circleRadius)
                {
                    
                    
                    //if (x >= (mapWidth / 2) && y >= (mapHeight / 2))
                    //{
                    //    float mindist =  Mathf.Min(Mathf.Abs(x - mapHeight), Mathf.Abs(y - mapHeight));
                    //    float curr = inputNoiseMap.GetGridObject(x, y);
                    //    //Debug.Log($"at {x}, {y} horisontal distance is {Mathf.Abs(x - mapHeight)} and vertical {Mathf.Abs(y - mapHeight)} and the min is {Mathf.Min(Mathf.Abs(x - mapHeight), Mathf.Abs(y - mapHeight))}");
                    //    inputNoiseMap.SetGridObject(x, y, curr * (mindist / 75));

                    //}
                    
                    //float smoothed = inputNoiseMap.GetGridObject(x, y) * (1000 / distance);
                    //Debug.Log($"at {x}, {y} we have distance {distance} and smoothing factor {(1000 / distance)}");
                }
                // if cell distance from the middle is outside circle
                // then find out the
            }
        }
        return inputNoiseMap;
    }
}
