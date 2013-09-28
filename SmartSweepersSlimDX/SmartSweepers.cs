using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using SlimDX.Windows;
using SmartSweepersSlimDX.AI.Utils;
using SmartSweepersSlimDX.Rendering;

namespace SmartSweepersSlimDX
{
    /// <summary>
    /// Implements core application logic of a SlimDX.
    /// </summary>
    internal class SmartSweepers : IDisposable
    {
        private const string TITLE = "Smart Sweepers";

        private bool disposed = false;
        private IDisposable apiContext;
        private FormWindowState currentFormWindowState;
        private Form form;
        private bool isFullScreen = false;
        private System.Threading.Thread fastUpdate;

        protected Controller controller;
        protected SlimDX.Color4 brushColor = new SlimDX.Color4(0.93f, 0.40f, 0.08f);

        /// <summary>
        /// Gets the width of the renderable area of the window.
        /// </summary>
        public int WindowWidth
        {
            get { return Params.Instance.WindowWidth; }
        }

        /// <summary>
        /// Gets the height of the renderable area of the window.
        /// </summary>
        public int WindowHeight
        {
            get { return Params.Instance.WindowHeight; }
        }

        /// <summary>
        /// Represents a Direct2D Context, only valid after calling InitializeDevice(DeviceSettings2D)
        /// </summary>
        public DeviceContext2D Context2D { get; private set; }

        /// <summary>
        /// Performs object finalization.
        /// </summary>
        ~SmartSweepers()
        {
            Dispose(false);
        }

        /// <summary>Runs this instance.</summary>
        public void Run()
        {
            form = CreateForm();

            currentFormWindowState = form.WindowState;

            bool isFormClosed = false;
            bool formIsResizing = false;

            form.KeyDown += HandleKeyDown;
            form.KeyUp += HandleKeyUp;
            form.Closed += (o, args) => { isFormClosed = true; TerminateFastUpdate(); };
            form.Resize += (o, args) =>
            {
                if (form.WindowState != currentFormWindowState)
                {
                    HandleResize(o, args);
                }

                currentFormWindowState = form.WindowState;
            };
            form.ResizeBegin += (o, args) => { formIsResizing = true; };
            form.ResizeEnd += (o, args) => { formIsResizing = false; HandleResize(o, args); };

            OnInitialize();

            controller = new Controller(Context2D.RenderTarget);

            MessagePump.Run(form, () =>
            {
                if (isFormClosed)
                {
                    return;
                }

                Update();

                if (!formIsResizing)
                {
                    Render();
                }
            });
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
                if (disposeManagedResources)
                {
                    apiContext.Dispose();
                    form.Dispose();
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Creates the form.
        /// </summary>
        /// <returns></returns>
        protected Form CreateForm()
        {
            return new RenderForm()
            {
                Icon = Icon.ExtractAssociatedIcon("SmartSweepersSlimDX.exe"),
                Text = TITLE,
                ClientSize = new Size(WindowWidth, WindowHeight)
            };
        }

        /// <summary>
        /// In a derived class, implements logic to initialize the instance.
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        /// In a derived class, implements logic to update any relevant state.
        /// </summary>
        protected virtual void OnUpdate() { }

        /// <summary>
        /// In a derived class, implements logic to render the instance.
        /// </summary>
        protected virtual void OnRender() { }

        /// <summary>
        /// In a derived class, implements logic that should occur before all
        /// other rendering.
        /// </summary>
        protected virtual void OnRenderBegin() { }

        /// <summary>
        /// In a derived class, implements logic that should occur after all
        /// other rendering.
        /// </summary>
        protected virtual void OnRenderEnd() { }

        /// <summary>
        /// Initializes a <see cref="DeviceContext2D">Direct2D device context</see> according to the specified settings.
        /// The base class retains ownership of the context and will dispose of it when appropriate.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>The initialized device context.</returns>
        protected void InitializeDevice(DeviceSettings2D settings)
        {
            var result = new DeviceContext2D(form.Handle, settings);
            //userInterfaceRenderer = new UserInterfaceRenderer9( result.Device, settings.Width, settings.Height );
            apiContext = result;
            Context2D = result;
        }

        /// <summary>Quits this instance.</summary>
        protected void Quit()
        {
            form.Close();
            TerminateFastUpdate();
        }

        private void TerminateFastUpdate()
        {
            if (fastUpdate != null && fastUpdate.ThreadState != System.Threading.ThreadState.Stopped)
            {
                Debug.WriteLine("Terminating fast-update thread.");
                fastUpdate.Abort();
            }
        }

        /// <summary>Updates state.</summary>
        private void Update()
        {
            OnUpdate();
        }

        /// <summary>Renders this instance.</summary>
        private void Render()
        {
            OnRenderBegin();
            OnRender();
            OnRenderEnd();
        }

        /// <summary>
        /// Handles a key down event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Left)
            {
                int direction = e.KeyCode == Keys.Right ? 1 : -1;
                float power = 30;

                double r = brushColor.Red + (float)RandomNumbers.Double() / power * direction;
                double g = brushColor.Green + (float)RandomNumbers.Double() / power * direction;
                double b = brushColor.Blue + (float)RandomNumbers.Double() / power * direction;

                r.Clamp(0, 1);
                g.Clamp(0, 1);
                b.Clamp(0, 1);

                brushColor = new SlimDX.Color4((float)r, (float)g, (float)b);

                Debug.WriteLine(string.Format("New color: {0,6:N2} {1,4:N2} {2,4:N2}", brushColor.Red, brushColor.Green, brushColor.Blue));
            }
        }

        /// <summary>
        /// Handles a key up event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void HandleKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Quit();
            }
            else if (e.Alt && e.KeyCode == Keys.Enter)
            {
                isFullScreen = !isFullScreen;
            }
            else if (e.KeyCode == Keys.F && controller != null)
            {
                controller.FastRenderToggle();
                if (controller.FastRender())
                {
                    fastUpdate = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
                    {
                        while (true)
                        {
                            controller.Update();
                        }
                    }));

                    fastUpdate.Start();
                }
                else
                {
                    fastUpdate.Abort();
                }
            }
        }

        private void HandleResize(object sender, EventArgs e)
        {
            if (form.WindowState == FormWindowState.Minimized)
            {
                return;
            }
        }
    }
}