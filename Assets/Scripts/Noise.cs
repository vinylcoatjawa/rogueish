using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseUtils
{
    public class Noise
    {
        //const uint noise1 = 0b11010111010101010010011111101011;
        //const uint noise2 = 0b11101101101010101100000101101101;
        //const uint noise3 = 0b11000101010100111101000110001011;

        const uint noise1 = 0xB5297A4D;
        const uint noise2 = 0x68E31DA4;
        const uint noise3 = 0x1B56C4E9;

        const uint prime = 0b11101110011010110010101101011101;




        /* 1D */
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

        public uint NoiseRandomRange(uint rangeFrom, uint rangeTo, uint position, uint seed)
        {
            uint mangled = Get1DNoiseUint(position, seed);
            return rangeFrom + mangled % (rangeTo - rangeFrom);
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

        public int ZeroOrOneOrMinusOne(uint position, uint seed)
        {
            uint mangled = position;
            mangled *= noise1;
            mangled += seed;
            mangled ^= (mangled << 8);
            mangled += noise2;
            mangled ^= (mangled >> 8);
            mangled *= noise3;
            mangled ^= (mangled << 8);

            return (int)(mangled % 3) - 1;
        }


        /* 2D */

        public uint Get2DNoiseUint(uint posX, uint posY, uint seed)
        {
            return Get1DNoiseUint(posX + (prime * posY), seed);
        }

        public float Get2DNoiseZeroToOne(uint posX, uint posY, uint seed)
        {
            return Get1DNoiseZeroToOne(posX + (prime * posY), seed);
        }
        


    }
}




