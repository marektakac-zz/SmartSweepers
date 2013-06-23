using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSweepersSlimDX.Rendering
{
    /// <summary>
    /// Settings used to initialize a Direct2D context.
    /// </summary>
    class DeviceSettings2D
    {
        /// <summary>
        /// Gets or sets the width of the renderable area.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the renderable area.
        /// </summary>
        public int Height { get; set; }
    }
}
