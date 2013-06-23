using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SmartSweepersSlimDX.Rendering
{
    /// <summary>
    /// The base class for an automatically-resizing buffer of primitive data.
    /// </summary>
    public abstract class DynamicPrimitiveBuffer<T> : IDisposable where T : struct
    {
        private const int initialBufferSize = 32;

        private int bufferSize;
        private bool needsCommit = false;
        private bool disposed = false;
        private List<T> vertices = new List<T>();

        /// <summary>
        /// Gets the number of vertices in the buffer.
        /// </summary>
        public int Count { get { return vertices.Count; } }

        /// <summary>
        /// Gets the size (in bytes) of a single buffer element.
        /// </summary>
        public int ElementSize { get { return Marshal.SizeOf(typeof(T)); } }

        /// <summary>
        /// Performs object finalization.
        /// </summary>
        ~DynamicPrimitiveBuffer()
        {
            Dispose(false);
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
        /// Adds a vertex to the buffer.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        public void Add(T vertex)
        {
            vertices.Add(vertex);
            if (vertices.Count > bufferSize)
            {
                bufferSize = bufferSize == 0 ? initialBufferSize : bufferSize * 2;
                ResizeBuffer(bufferSize * ElementSize);
            }

            needsCommit = true;
        }

        /// <summary>
        /// Clears the buffer of all primitive data.
        /// </summary>
        public void Clear()
        {
            // Note that we do not require a recommit here, since trying to render an
            // empty buffer will just no-op. It doesn't matter what's in the real buffer
            // on the card at this point, so there's no sense in locking it.
            vertices.Clear();
        }

        /// <summary>
        /// Commits the buffer changes in preparation for rendering.
        /// </summary>
        public void Commit()
        {
            if (needsCommit)
            {
                FillBuffer(vertices);
                needsCommit = false;
            }
        }

        /// <summary>
        /// In a derived class, implements logic to resize the buffer.
        /// During resize, the existing buffer contents need not be preserved.
        /// </summary>
        /// <param name="sizeInBytes">The new size, in bytes.</param>
        protected abstract void ResizeBuffer(int sizeInBytes);

        /// <summary>
        /// In a derived class, implements logic to fill the buffer with vertex data.
        /// </summary>
        /// <param name="vertices">The vertex data.</param>
        protected abstract void FillBuffer(List<T> vertices);
    }
}
