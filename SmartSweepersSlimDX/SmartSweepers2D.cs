using SlimDX;
using SmartSweepersSlimDX.Rendering;

namespace SmartSweepersSlimDX
{
    internal class SmartSweepers2D : SmartSweepers
    {
        /// <summary>Disposes of object resources.</summary>
        /// <param name="disposeManagedResources">If true, managed resources should be
        /// disposed of in addition to unmanaged resources.</param>
        protected override void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
            }

            base.Dispose(disposeManagedResources);
        }

        /// <summary>In a derived class, implements logic to initialize the instance.</summary>
        protected override void OnInitialize()
        {
            DeviceSettings2D settings = new DeviceSettings2D
            {
                Width = WindowWidth,
                Height = WindowHeight
            };

            InitializeDevice(settings);
        }

        /// <summary>
        /// In a derived class, implements logic that should occur before all
        /// other rendering.
        /// </summary>
        protected override void OnRenderBegin()
        {
            if (!controller.FastRender())
            {
                controller.Update();
            }

            Context2D.RenderTarget.BeginDraw();
            Context2D.RenderTarget.Transform = Matrix3x2.Identity;
            Context2D.RenderTarget.Clear(new Color4(System.Drawing.Color.Black));
        }

        /// <summary>In a derived class, implements logic to render the instance.</summary>
        protected override void OnRender()
        {
            controller.Render();
        }

        /// <summary>
        /// In a derived class, implements logic that should occur after all
        /// other rendering.
        /// </summary>
        protected override void OnRenderEnd()
        {
            Context2D.RenderTarget.EndDraw();
        }
    }
}
