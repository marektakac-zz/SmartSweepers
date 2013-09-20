using SlimDX.Direct2D;
using SmartSweepersSlimDX.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSweepersSlimDX
{
    internal class SweeperDraw
    {
        private float scale = 1;
        private float rotation = 0;
        private float posX = 100;
        private float posY = 100;
        private List<System.Drawing.Point> points = new List<System.Drawing.Point>
        { 
            new System.Drawing.Point(-4, -4),
            new System.Drawing.Point(-4, 4),
	        new System.Drawing.Point(-2, 4),
            new System.Drawing.Point(-2, -4),
            
            new System.Drawing.Point(2, -4),
            new System.Drawing.Point(4, -4),
            new System.Drawing.Point(4, 4),
            new System.Drawing.Point(2, 4),

            new System.Drawing.Point(2, -2),
            new System.Drawing.Point(-2, -2),

            new System.Drawing.Point(-2, 2),
            new System.Drawing.Point(-1, 2),
            new System.Drawing.Point(-1, 7),
            new System.Drawing.Point(1, 7),
            new System.Drawing.Point(1, 2),
            new System.Drawing.Point(2, 2)
        };
        private PointF[] vertices = null;
        private DeviceContext2D context2D;
        private Guid id;

        public PathGeometry LeftTrack
        {
            get
            {
                if (vertices == null || vertices.Length == 0)
                {
                    return null;
                }

                var geometry = new PathGeometry(context2D.RenderTarget.Factory);
                using (GeometrySink sink = geometry.Open())
                {
                    sink.BeginFigure(vertices[0], FigureBegin.Filled);
                    sink.AddLines(vertices.Take(4).ToArray());
                    sink.AddLine(vertices[0]);
                    sink.EndFigure(FigureEnd.Closed);
                    sink.Close();
                }

                return geometry;
            }
        }

        public PathGeometry RightTrack
        {
            get
            {
                if (vertices == null || vertices.Length == 0)
                {
                    return null;
                }

                var geometry = new PathGeometry(context2D.RenderTarget.Factory);
                using (GeometrySink sink = geometry.Open())
                {
                    sink.BeginFigure(vertices[4], FigureBegin.Filled);
                    sink.AddLines(vertices.Skip(4).Take(4).ToArray());
                    sink.AddLine(vertices[4]);
                    sink.EndFigure(FigureEnd.Closed);
                    sink.Close();
                }

                return geometry;
            }
        }

        public PathGeometry Body
        {
            get
            {
                if (vertices == null || vertices.Length == 0)
                {
                    return null;
                }

                var geometry = new PathGeometry(context2D.RenderTarget.Factory);
                using (GeometrySink sink = geometry.Open())
                {
                    sink.BeginFigure(vertices[8], FigureBegin.Filled);
                    sink.AddLines(vertices.Skip(7).ToArray());
                    sink.EndFigure(FigureEnd.Closed);
                    sink.Close();
                }

                return geometry;
            }
        }

        public SweeperDraw(DeviceContext2D context2D)
        {
            this.context2D = context2D;

            id = Guid.NewGuid();

            posX = Utils.RandomInt(400);
            posY = Utils.RandomInt(400);
            rotation = Utils.RandomInt(360);

            Debug.WriteLine(string.Format("Sweeper initializes with params rot: {0} pos: [{1},{2}]", rotation, posX, posY));
        }

        public void Update()
        {
            rotation += (Utils.RandomInt(3) - 1) * (int)(Utils.RandomDouble() * 3);
            posX += (Utils.RandomInt(3) - 1) * (int)(Utils.RandomDouble() * 2);
            posY += (Utils.RandomInt(3) - 1) * (int)(Utils.RandomDouble() * 2);

            while (rotation < 0) { rotation += 360; }
            while (rotation > 360) { rotation -= 360; }
            Utils.Clamp(ref posX, 10, 400);
            Utils.Clamp(ref posY, 10, 400);

            System.Drawing.Drawing2D.Matrix matTransform = new System.Drawing.Drawing2D.Matrix();
            matTransform.Scale(scale, scale, System.Drawing.Drawing2D.MatrixOrder.Append);
            matTransform.Rotate(rotation, System.Drawing.Drawing2D.MatrixOrder.Append);
            matTransform.Translate(posX, posY, System.Drawing.Drawing2D.MatrixOrder.Append);

            var temp = points.ToArray();
            matTransform.TransformPoints(temp);
            vertices = temp.Select(p => new PointF(p.X, p.Y)).ToArray();
        }
    }
}
