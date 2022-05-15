using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoiseUtils;

public class DungeonManager 
{
    // get dungeon seed created from world seed
    // decide the number of levels and their respective seeds the dungeon will have
    // sore these in a dictionary

    //uint worldSeed = 123456789;
    Noise noise = new Noise();
    private Dictionary<int, uint> dungeonDict = new Dictionary<int, uint>();

    public Dictionary<int, uint> GenerateDungeonDict(uint seed)
    {
        // decide the number of levels
        int numberOfLevels = (int)noise.NoiseRandomRange(10, 20, 1987, seed);
        for (int level = 0; level < numberOfLevels; level++)
        {
            uint levelSeed = noise.Get1DNoiseUint((uint)level, seed);

            dungeonDict.Add(level, levelSeed);
        }

        return dungeonDict;
    }



}
