using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class WorldMapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColourMap };
    public DrawMode drawMode;
    
    public int mapWidth, mapHeight;
    public float noiseScale;
    public bool autoUpdate;

    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public uint seed;
    public Vector2 offset;

    public TerrainType[] regions;

    public void GenerateMap()
    {
        Grid<float> noiseMap = PerlinMap.GenerateNoiseMap(mapWidth, mapHeight, noiseScale, octaves, persistance, lacunarity, seed, offset);

        Color[] colorMap = new Color[mapHeight * mapWidth];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap.GetGridObject(x, y);
                for (int region = 0; region < regions.Length; region++)
                {
                    if(currentHeight <= regions[region].height)
                    {
                        colorMap[y * mapWidth + x] = regions[region].colour;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colorMap, mapWidth, mapHeight));
        }

    }

    private void OnValidate()
    {
        if (mapWidth < 1) mapWidth = 1;
        if (mapHeight < 1) mapHeight = 1;
        if (lacunarity < 1) lacunarity = 1;
        if (octaves < 0) octaves = 0;

    }
    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color colour;
    }
}

