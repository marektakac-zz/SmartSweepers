using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SlimDX;
using SlimDX.Direct2D;
using SmartSweepersSlimDX.Rendering;
using System.Diagnostics;

namespace SmartSweepersSlimDX
{
    internal class SmartSweepers2D : SmartSweepers
    {
        private PathGeometry triangle;
        private GeometryGroup sweeperGG;
        private SolidColorBrush brush;

        /// <summary>Disposes of object resources.</summary>
        /// <param name="disposeManagedResources">If true, managed resources should be
        /// disposed of in addition to unmanaged resources.</param>
        protected override void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                brush.Dispose();
                triangle.Dispose();
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

            triangle = new PathGeometry(Context2D.RenderTarget.Factory);

            using (GeometrySink sink = triangle.Open())
            {
                PointF p0 = new PointF(0.50f * WindowWidth, 0.25f * WindowHeight);
                PointF p1 = new PointF(0.75f * WindowWidth, 0.75f * WindowHeight);
                PointF p2 = new PointF(0.25f * WindowWidth, 0.75f * WindowHeight);

                sink.BeginFigure(p0, FigureBegin.Filled);
                sink.AddLine(p1);
                sink.AddLine(p2);
                sink.EndFigure(FigureEnd.Closed);

                // Note that Close() and Dispose() are not equivalent like they are for
                // some other IDisposable() objects.
                sink.Close();
            }

            List<System.Drawing.Point> sweeper = new List<System.Drawing.Point>
            { 
                new System.Drawing.Point(-4, -4),
                new System.Drawing.Point(-4, 4),
	            new System.Drawing.Point(-2, 4),
                new System.Drawing.Point(-2, -4),
            
                new System.Drawing.Point(2, -4),
                new System.Drawing.Point(4, -4),
                new System.Drawing.Point(4, 4),
                new System.Drawing.Point(2, 4),

                new System.Drawing.Point(-2, -2),
                new System.Drawing.Point(2, -2),

                new System.Drawing.Point(-2, 2),
                new System.Drawing.Point(-1, 2),
                new System.Drawing.Point(-1, 7),
                new System.Drawing.Point(1, 7),
                new System.Drawing.Point(1, 2),
                new System.Drawing.Point(2, 2)
            };

            float scale = 10;
            float rotation = 0;
            float posX = 10;
            float posY = 10;

            System.Drawing.Drawing2D.Matrix matTransform = new System.Drawing.Drawing2D.Matrix();
            matTransform.Scale(scale, scale);
            matTransform.Rotate(rotation);
            matTransform.Translate(posX, posY);

            var sweeperTrans = sweeper.ToArray();
            matTransform.TransformPoints(sweeperTrans);
            var sweeperVB = sweeperTrans.Select(p => new PointF(p.X, p.Y)).ToArray();

            var leftTrack = new PathGeometry(Context2D.RenderTarget.Factory);
            using (GeometrySink sink = leftTrack.Open())
            {
                sink.BeginFigure(sweeperVB[0], FigureBegin.Filled);
                sink.AddLines(sweeperVB.Take(4).ToArray());
                sink.AddLine(sweeperVB[0]);
                sink.EndFigure(FigureEnd.Closed);
                sink.Close();
            }

            var rightTrack = new PathGeometry(Context2D.RenderTarget.Factory);
            using (GeometrySink sink = rightTrack.Open())
            {
                sink.BeginFigure(sweeperVB[4], FigureBegin.Filled);
                sink.AddLines(sweeperVB.Skip(4).Take(4).ToArray());
                sink.AddLine(sweeperVB[4]);
                sink.EndFigure(FigureEnd.Closed);
                sink.Close();
            }

            var body = new PathGeometry(Context2D.RenderTarget.Factory);
            using (GeometrySink sink = body.Open())
            {
                sink.BeginFigure(sweeperVB[8], FigureBegin.Filled);
                sink.AddLines(sweeperVB.Skip(8).ToArray());
                sink.EndFigure(FigureEnd.Closed);
                sink.Close();
            }

            sweeperGG = new GeometryGroup(Context2D.RenderTarget.Factory, FillMode.Alternate, new[] { leftTrack, rightTrack, body });

            brush = new SolidColorBrush(Context2D.RenderTarget, brushColor);
        }

        /// <summary>
        /// In a derived class, implements logic that should occur before all
        /// other rendering.
        /// </summary>
        protected override void OnRenderBegin()
        {
            brush = new SolidColorBrush(Context2D.RenderTarget, brushColor);

            Context2D.RenderTarget.BeginDraw();
            Context2D.RenderTarget.Transform = Matrix3x2.Identity;
            Context2D.RenderTarget.Clear(new Color4(0.3f, 0.3f, 0.3f));
        }

        /// <summary>In a derived class, implements logic to render the instance.</summary>
        protected override void OnRender()
        {
            //Context2D.RenderTarget.FillGeometry(triangle, brush);
            
            foreach (var geometry in sweeperGG.GetSourceGeometry())
            {
                Context2D.RenderTarget.FillGeometry(geometry, brush);
            }
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
