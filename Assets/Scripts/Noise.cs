using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseUtils
{
    public class Noise
    {
        const uint noise1 = 0b11010111010101010010011111101011;
        const uint noise2 = 0b11101101101010101100000101101101;
        const uint noise3 = 0b11000101010100111101000110001011;

        public uint Get1DNoiseUint (uint position, uint seed)
        {
            uint mangled = position;
            mangled *= noise1;
            mangled += seed;
            mangled ^= (mangled << 8);
            mangled += noise2;
            mangled ^= (mangled >> 8);
            mangled *= noise3;
            mangled ^= (mangled << 8);

            return mangled;
        }

        public float Get1DNoiseZeroToOne (uint position, uint seed)
        {
            uint mangled = position;
            mangled *= noise1;
            mangled += seed;
            mangled ^= (mangled << 8);
            mangled += noise2;
            mangled ^= (mangled >> 8);
            mangled *= noise3;
            mangled ^= (mangled << 8);

            return (float)mangled / uint.MaxValue;
        }

        public uint ZeroOrOne(uint position, uint seed)
        {
            uint mangled = position;
            mangled *= noise1;
            mangled += seed;
            mangled ^= (mangled << 8);
            mangled += noise2;
            mangled ^= (mangled >> 8);
            mangled *= noise3;
            mangled ^= (mangled << 8);

            return (uint)Mathf.RoundToInt((float)mangled / uint.MaxValue);
        }


    }
}