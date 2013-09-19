using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using SlimDX.DXGI;
using SlimDX.Windows;
using SmartSweepersSlimDX.Rendering;
using SmartSweepersSlimDX.Rendering.UI;
using SmartSweepersSlimDX.UI;
using SmartSweepersSlimDX.UI.Bindings;
using SmartSweepersSlimDX.Utility;

namespace SmartSweepersSlimDX
{
    /// <summary>
    /// Implements core application logic of a SlimDX.
    /// </summary>
    class SmartSweepers : IDisposable
    {
        private const int WINDOW_WIDTH = 800;
        private const int WINDOW_HEIGHT = 600;
        private const string TITLE = "Smart Sweepers";

        private bool disposed = false;
        private readonly Clock clock = new Clock();
        private readonly Bindable<float> framesPerSecond = new Bindable<float>();
        private IDisposable apiContext;
        private FormWindowState currentFormWindowState;
        private Form _form;
        private float frameAccumulator;
        private int frameCount;
        private bool deviceLost = false;
        private UserInterface userInterface;
        private UserInterfaceRenderer userInterfaceRenderer;
        private bool isFullScreen = false;

        /// <summary>
        /// Gets the width of the renderable area of the window.
        /// </summary>
        public int WindowWidth
        {
            get { return WINDOW_WIDTH; }
        }

        /// <summary>
        /// Gets the number of seconds passed since the last frame.
        /// </summary>
        public float FrameDelta { get; private set; }

        /// <summary>
        /// Gets the height of the renderable area of the window.
        /// </summary>
        public int WindowHeight
        {
            get { return WINDOW_HEIGHT; }
        }

        /// <summary>Gets the user interface.</summary>
        /// <value>The user interface.</value>
        public UserInterface UserInterface
        {
            get { return userInterface; }
        }

        /// <summary>
        /// Represents a Direct3D10 Context, only valid after calling InitializeDevice(DeviceSettings10)
        /// </summary>
        public DeviceContext10 Context10 { get; private set; }

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
            _form = CreateForm();

            currentFormWindowState = _form.WindowState;

            bool isFormClosed = false;
            bool formIsResizing = false;

            _form.MouseClick += HandleMouseClick;
            _form.KeyDown += HandleKeyDown;
            _form.KeyUp += HandleKeyUp;
            _form.Closed += (o, args) => { isFormClosed = true; };
            _form.Resize += (o, args) =>
            {
                if (_form.WindowState != currentFormWindowState)
                {
                    HandleResize(o, args);
                }

                currentFormWindowState = _form.WindowState;
            };
            _form.ResizeBegin += (o, args) => { formIsResizing = true; };
            _form.ResizeEnd += (o, args) =>
            {
                formIsResizing = false;
                HandleResize(o, args);
            };

            userInterface = new UserInterface();
            var stats = new Element();
            stats.SetBinding("Label", framesPerSecond);
            userInterface.Container.Add(stats);

            OnInitialize();
            OnResourceLoad();

            clock.Start();

            MessagePump.Run(_form, () =>
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

            OnResourceUnload();
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
                    if (userInterfaceRenderer != null)
                    {
                        userInterfaceRenderer.Dispose();
                    }

                    apiContext.Dispose();
                    _form.Dispose();
                }

                disposed = true;
            }
        }

        protected Form CreateForm()
        {
            return new RenderForm()
            {
                Text = TITLE,
                ClientSize = new Size(WindowWidth, WindowHeight)
            };
        }

        /// <summary>
        /// In a derived class, implements logic to initialize the instance.
        /// </summary>
        protected virtual void OnInitialize() { }

        protected virtual void OnResourceLoad() { }

        protected virtual void OnResourceUnload() { }

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
            var result = new DeviceContext2D(_form.Handle, settings);
            //userInterfaceRenderer = new UserInterfaceRenderer9( result.Device, settings.Width, settings.Height );
            apiContext = result;
            Context2D = result;
        }

        /// <summary>
        /// Initializes a <see cref="DeviceContext10">Direct3D10 device context</see> according to the specified settings.
        /// The base class retains ownership of the context and will dispose of it when appropriate.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>The initialized device context.</returns>
        protected void InitializeDevice(DeviceSettings10 settings)
        {
            var result = new DeviceContext10(_form.Handle, settings);
            userInterfaceRenderer = new UserInterfaceRenderer10(result.Device, settings.Width, settings.Height);
            apiContext = result;
            Context10 = result;
        }

        /// <summary>Quits this instance.</summary>
        protected void Quit()
        {
            _form.Close();
        }

        /// <summary>Updates state.</summary>
        private void Update()
        {
            FrameDelta = clock.Update();
            userInterface.Container.Update();
            OnUpdate();
        }

        /// <summary>Renders this instance.</summary>
        private void Render()
        {
            if (deviceLost)
            {
                // This should only become true if we're using D3D9, so we can assume the
                // D3D9 context is valid at this point.
                /*
                if (Context9.Device.TestCooperativeLevel() == SlimDX.Direct3D9.ResultCode.DeviceNotReset)
                {
                    Context9.Device.Reset(Context9.PresentParameters);
                    deviceLost = false;
                    userInterfaceRenderer = new UserInterfaceRenderer9(Context9.Device, WindowWidth, WindowHeight);
                    OnResourceLoad();
                }
                else
                {
                    Thread.Sleep(100);
                    return;
                }
                */
            }

            frameAccumulator += FrameDelta;
            ++frameCount;
            if (frameAccumulator >= 1.0f)
            {
                framesPerSecond.Value = frameCount / frameAccumulator;

                //Debug.WriteLine(string.Format("FPS: {0:0}", framesPerSecond.Value));

                frameAccumulator = 0.0f;
                frameCount = 0;
            }

            try
            {
                OnRenderBegin();
                OnRender();
                if (userInterfaceRenderer != null)
                {
                    userInterfaceRenderer.Render(userInterface);
                }
                OnRenderEnd();
            }
            catch (SlimDX.Direct3D9.Direct3D9Exception e)
            {
                if (e.ResultCode == SlimDX.Direct3D9.ResultCode.DeviceLost)
                {
                    OnResourceUnload();
                    userInterfaceRenderer.Dispose();
                    deviceLost = true;
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Handles a mouse click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void HandleMouseClick(object sender, MouseEventArgs e) { }

        /// <summary>
        /// Handles a key down event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void HandleKeyDown(object sender, KeyEventArgs e) { }

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
                OnResourceUnload();

                isFullScreen = !isFullScreen;

                /*
                if (Context9 != null)
                {
                    userInterfaceRenderer.Dispose();

                    Context9.PresentParameters.BackBufferWidth = _configuration.WindowWidth;
                    Context9.PresentParameters.BackBufferHeight = _configuration.WindowHeight;
                    Context9.PresentParameters.Windowed = !isFullScreen;

                    if (!isFullScreen)
                        _form.MaximizeBox = true;

                    Context9.Device.Reset(Context9.PresentParameters);

                    userInterfaceRenderer = new UserInterfaceRenderer9(Context9.Device, _form.ClientSize.Width, _form.ClientSize.Height);
                }
                else 
                 */
                if (Context10 != null)
                {
                    userInterfaceRenderer.Dispose();

                    Context10.SwapChain.ResizeBuffers(1, WindowWidth, WindowHeight, Context10.SwapChain.Description.ModeDescription.Format, SwapChainFlags.AllowModeSwitch);
                    Context10.SwapChain.SetFullScreenState(isFullScreen, null);

                    userInterfaceRenderer = new UserInterfaceRenderer10(Context10.Device, WindowWidth, WindowHeight);
                }

                OnResourceLoad();
            }
        }

        private void HandleResize(object sender, EventArgs e)
        {
            if (_form.WindowState == FormWindowState.Minimized)
            {
                return;
            }

            OnResourceUnload();
            /*
            if (Context9 != null)
            {
                userInterfaceRenderer.Dispose();

                Context9.PresentParameters.BackBufferWidth = 0;
                Context9.PresentParameters.BackBufferHeight = 0;

                Context9.Device.Reset(Context9.PresentParameters);

                userInterfaceRenderer = new UserInterfaceRenderer9(Context9.Device, _form.ClientSize.Width, _form.ClientSize.Height);
            }
            else 
            */
            if (Context10 != null)
            {
                userInterfaceRenderer.Dispose();

                Context10.SwapChain.ResizeBuffers(1, WindowWidth, WindowHeight, Context10.SwapChain.Description.ModeDescription.Format, Context10.SwapChain.Description.Flags);


                userInterfaceRenderer = new UserInterfaceRenderer10(Context10.Device, _form.ClientSize.Width, _form.ClientSize.Height);
            }

            OnResourceLoad();
        }
    }
}