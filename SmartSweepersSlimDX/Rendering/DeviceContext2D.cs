using System;
using System.Drawing;
using SlimDX.Direct2D;

namespace SmartSweepersSlimDX.Rendering
{
    /// <summary>
    /// Provides creation and management functionality for a Direct2D rendering context.
    /// </summary>
    class DeviceContext2D : IDisposable
    {
        #region Private Variables

        private bool disposed = false;
        private DeviceSettings2D settings;
        private Factory factory;

        #endregion

        #region Public Properties

        /// <summary>Gets the underlying Direct3D render target.</summary>
        /// <value>The render target.</value>
        public WindowRenderTarget RenderTarget { get; private set; }

        #endregion

        #region Constructor / Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceContext2D" /> class.
        /// </summary>
        /// <param name="handle">The window handle to associate with the device.</param>
        /// <param name="settings">The settings used to configure the device.</param>
        /// <exception cref="System.ArgumentException">Value must be a valid window handle.;handle</exception>
        /// <exception cref="System.ArgumentNullException">Settings value must be set.</exception>
        public DeviceContext2D(IntPtr handle, DeviceSettings2D settings)
        {
            if (handle == IntPtr.Zero)
            {
                throw new ArgumentException("Value must be a valid window handle.", "handle");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            this.settings = settings;

            factory = new Factory();

            RenderTarget = new WindowRenderTarget(factory, new WindowRenderTargetProperties
            {
                Handle = handle,
                PixelSize = new Size(settings.Width, settings.Height)
            });
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="DeviceContext2D"/> class.
        /// </summary>
        ~DeviceContext2D()
        {
            Dispose(false);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Disposes of object resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Disposes of object resources.
        /// </summary>
        /// <param name="disposeManagedResources">If true, managed resources should be
        /// disposed of in addition to unmanaged resources.</param>
        protected void Dispose(bool disposeManagedResources)
        {
            if (!this.disposed)
            {
                if (disposeManagedResources)
                {
                    RenderTarget.Dispose();
                    factory.Dispose();
                }

                disposed = true;
            }
        }

        #endregion
    }
}
