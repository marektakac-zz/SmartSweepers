﻿using SlimDX;
using SmartSweepersSlimDX.UI;

namespace SmartSweepersSlimDX.Rendering.UI
{
    /// <summary>
    /// Defines the interface required to specify an element's visual representation.
    /// </summary>
    interface IElementVisual
    {
        /// <summary>
        /// Measures the element, returning the size (in pixels) it would occupy if
        /// rendered with the specified renderer.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        /// <param name="element">The element.</param>
        /// <returns>The size of the element (in pixels).</returns>
        Vector2 Measure(UserInterfaceRenderer renderer, Element element);

        /// <summary>
        /// Renders the element using the specified renderer.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        /// <param name="element">The element.</param>
        /// <param name="x">The X coordinate (in pixels) of the upper left corner of the region the element should be rendered to.</param>
        /// <param name="y">The Y coordinate (in pixels) of the upper left corner of the region the element should be rendered to.</param>
        /// <param name="width">The width (in pixels) of the region the element should be rendered to.</param>
        /// <param name="height">The height (in pixels) of the region the element should be rendered to.</param>
        void Render(UserInterfaceRenderer renderer, Element element, int x, int y, int width, int height);
    }
}
