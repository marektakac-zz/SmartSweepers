using SlimDX;

namespace SmartSweepersSlimDX.Rendering.UI
{
    /// <summary>
    /// Defines the interface required to specify an element's visual representation.
    /// </summary>
    public class Text
    {
        /// <summary>Gets or sets the X.</summary>
        /// <value>The X.</value>
        public int X { get; set; }

        /// <summary>Gets or sets the Y.</summary>
        /// <value>The Y.</value>
        public int Y { get; set; }

        /// <summary>Gets or sets the string.</summary>
        /// <value>The string.</value>
        public string String { get; set; }

        /// <summary>Gets or sets the color.</summary>
        /// <value>The color.</value>
        public Color4 Color { get; set; }

        /// <summary>Initializes a new instance of the <see cref="Text"/> class.</summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        public Text(int x, int y, string text, Color4 color)
        {
            X = x; Y = y; String = text; Color = color;
        }
    }
}
