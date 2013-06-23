﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSweepersSlimDX.Utility
{
    /// <summary>
    /// A mechanism for tracking elapsed time.
    /// </summary>
    public class Clock
    {
        private bool isRunning;
        private readonly long frequency;
        private long count;

        /// <summary>
        /// Initializes a new instance of the <see cref="Clock"/> class.
        /// </summary>
        public Clock()
        {
            frequency = Stopwatch.Frequency;
        }

        public void Start()
        {
            count = Stopwatch.GetTimestamp();
            isRunning = true;
        }

        /// <summary>
        /// Updates the clock.
        /// </summary>
        /// <returns>The time, in seconds, that elapsed since the previous update.</returns>
        public float Update()
        {
            float result = 0.0f;
            if (isRunning)
            {
                long last = count;
                count = Stopwatch.GetTimestamp();
                result = (float)(count - last) / frequency;
            }

            return result;
        }
    }
}
