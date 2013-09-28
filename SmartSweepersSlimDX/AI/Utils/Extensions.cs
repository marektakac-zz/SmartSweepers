using System;

namespace SmartSweepersSlimDX.AI.Utils
{
    internal static class Extensions
    {
        /// <summary>
        /// Clamps the first argument between the second two.
        /// </summary>
        /// <param name="number">The arg.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        internal static void Clamp(this double number, double min, double max)
        {
            if (min > max)
            {
                throw new ArgumentException(string.Format("MIN={0} is greater then MAX={1}", min, max));
            }

            if (number < min)
            {
                number = min;
            }
            else if (number > max)
            {
                number = max;
            }
        }
    }
}
