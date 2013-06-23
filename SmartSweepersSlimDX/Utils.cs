using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSweepersSlimDX
{
    internal static class Utils
    {
        /// <summary>
        /// The random instance.
        /// </summary>
        private static Random random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

        /// <summary>
        /// Returns a random integer between x and y.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public static int RandomInt(int x, int y)
        {
            return random.Next(x, y);
        }

        /// <summary>
        /// Returns a random double between zero and 1.
        /// </summary>
        /// <returns></returns>
        public static double RandomDouble()
        {
            return random.NextDouble();
        }

        /// <summary>Returns a random bool.</summary>
        /// <returns></returns>
        public static bool RandBool()
        {
            return random.Next(0, 1) == 1 ? true : false;
        }

        /// <summary>Returns a random double in the range -1 &lt; n &lt; 1.</summary>
        /// <returns></returns>
        public static double RandomClamped()
        {
            return random.NextDouble() - random.NextDouble();
        }

        /// <summary>
        /// Clamps the first argument between the second two.
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        public static void Clamp(ref double arg, double min, double max)
        {
            if (min > max)
            {
                throw new ArgumentException(string.Format("MIN={0} is greater then MAX={1}", min, max));
            }

            if (arg < min)
            {
                arg = min;
            }
            else if (arg > max)
            {
                arg = max;
            }
        }
    }
}
