using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoiseUtils;

public static class PerlinMap
{
    public static Grid<float> GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity, uint seed, Vector2 offset)
    {
        Grid<float> noiseMap = new Grid<float>(mapWidth, mapHeight, 1, Vector3.zero, () => 0f);
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
                    float sampleX = (x - halfWidth) / scale * frequency + octavesOffsets[octave].x;
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

                return noiseMap;
    }
}
