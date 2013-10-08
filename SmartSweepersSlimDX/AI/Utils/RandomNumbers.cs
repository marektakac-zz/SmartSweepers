using System;

namespace SmartSweepersSlimDX.AI.Utils
{
    internal static class RandomNumbers
    {
        #region Private Variables

        /// <summary>
        /// The random instance.
        /// </summary>
        private static Random random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a random integer between x and y.
        /// </summary>
        /// <param name="max">The maximum.</param>
        /// <returns></returns>
        public static int Int(int max)
        {
            return random.Next(max);
        }

        /// <summary>
        /// Returns a random double between zero and 1.
        /// </summary>
        /// <returns></returns>
        public static double Double()
        {
            return random.NextDouble();
        }

        /// <summary>Returns a random bool.</summary>
        /// <returns></returns>
        public static bool Bool()
        {
            return random.Next(0, 1) == 1 ? true : false;
        }

        /// <summary>Returns a random double in the range -1 &lt; n &lt; 1.</summary>
        /// <returns></returns>
        public static double Clamped()
        {
            return random.NextDouble() - random.NextDouble();
        }

        #endregion
    }
}
