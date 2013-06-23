﻿using System;
using SlimDX;
using SmartSweepersSlimDX.UI;

namespace SmartSweepersSlimDX.Rendering.UI
{
    /// <summary>
    /// Provides baseline functionality for rendering a user interface.
    /// </summary>
    public abstract class UserInterfaceRenderer : IDisposable
    {
        private bool disposed = false;

        /// <summary>
        /// Performs object finalization.
        /// </summary>
        ~UserInterfaceRenderer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Renders the specified user interface.
        /// </summary>
        /// <param name="userInterface">The user interface to render.</param>
        public void Render(UserInterface userInterface)
        {
            if (userInterface == null)
                throw new ArgumentNullException("userInterface");

            int y = 0;
            foreach (Element element in userInterface.Container)
            {
                IElementVisual visual = new DefaultVisual();
                Vector2 size = visual.Measure(this, element);
                visual.Render(this, element, 0, y, (int)size.X, (int)size.Y);
                y += (int)size.Y;
            }

            Flush();
        }

        /// <summary>
        /// Disposes of object resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of object resources.
        /// </summary>
        /// <param name="disposeManagedResources">If true, managed resources should be
        /// disposed of in addition to unmanaged resources.</param>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (!disposed)
            {
                disposed = true;
            }
        }

        /// <summary>
        /// In a derived class, implements logic to flush all pending rendering commands.
        /// </summary>
        protected abstract void Flush();

        /// <summary>
        /// Computes the metrics for a string if it were to be rendered with this renderer.
        /// </summary>
        /// <param name="text">The string.</param>
        /// <returns>The size metrics for the string.</returns>
        internal abstract Vector2 MeasureString(string text);

        /// <summary>
        /// Renders a string.
        /// </summary>
        /// <param name="text">The string.</param>
        /// <param name="x">The X coordinate of the upper left corner of the text.</param>
        /// <param name="y">The Y coordinate of the upper left corner of the text.</param>
        /// <param name="color">The color of the text.</param>
        internal abstract void RenderString(string text, int x, int y, Color4 color);

        /// <summary>
        /// Renders a line.
        /// </summary>
        /// <param name="x0">The X coordinate of the first point.</param>
        /// <param name="y0">The Y coordinate of the first point.</param>
        /// <param name="color0">The color of the first point.</param>
        /// <param name="x1">The X coordinate of the second point.</param>
        /// <param name="y1">The Y coordinate of the second point.</param>
        /// <param name="color1">The color of the second point.</param>
        internal abstract void RenderLine(int x0, int y0, Color4 color0, int x1, int y1, Color4 color1);

        /// <summary>
        /// Renders a rectangle.
        /// </summary>
        /// <param name="x">The X coordinate of the upper left corner of the rectangle.</param>
        /// <param name="y">The Y coordinate of the upper left corner of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="color">The color of the rectangle.</param>
        internal void RenderRectangle(int x, int y, int width, int height, Color4 color)
        {
            RenderLine(x, y, color, x + width, y, color);
            RenderLine(x + width, y, color, x + width, y + height, color);
            RenderLine(x + width, y + height, color, x, y + height, color);
            RenderLine(x, y + height, color, x, y, color);
        }
    }
}
