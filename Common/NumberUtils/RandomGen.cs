using System;

namespace Common.NumberUtils
{
    public static class RandomGen
    {
        private const int NUM_RANDOMS = 100000;

        private static float[] randomNormalizedFloats = new float[NUM_RANDOMS];
        private static int curIndex = 0;
        private static bool initialized = false;
        
        private static void initialize()
        {
            Random rng = new Random();
            for (int i = 0; i < randomNormalizedFloats.Length; i++)
            {
                randomNormalizedFloats[i] = (float)rng.NextDouble();
            }
            initialized = true;
        }

        public static float NextNormalizedFloat()
        {
            if (!initialized)
            {
                initialize();
            }
            else if (curIndex >= NUM_RANDOMS)
            {
                curIndex = 0;
            }
            return randomNormalizedFloats[curIndex++];
        }

        public static float GetFloat()
        {
            return NextNormalizedFloat() * float.MaxValue;
        }

        public static float GetFloatRange(float minInclusive, float maxInclusive)
        {
            return minInclusive + NextNormalizedFloat() * (maxInclusive - minInclusive);
        }

        public static int GetInt()
        {
            return (int)(NextNormalizedFloat() * int.MaxValue + .5f);
        }

        public static int GetIntRange(int minInclusive, int maxInclusive)
        {
            return minInclusive + (int)(NextNormalizedFloat() * (maxInclusive - minInclusive) + .5f);
        }

    }
}
