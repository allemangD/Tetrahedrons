using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Mail;
using System.Security;
using OpenTK;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Tetrahedrons
{
    public struct Polygon
    {
        public Vector3d[] Points;
        public Color Color;

        public Polygon(Vector3d[] points) : this(points, Color.DodgerBlue)
        {
        }

        public Polygon(Vector3d[] points, Color color)
        {
            Points = points;
            Color = color;
        }

        public void Draw()
        {
            GL.Begin(PrimitiveType.TriangleStrip);
            GL.Color3(Color);
            foreach (var v in Points)
                GL.Vertex3(v);
            GL.End();
        }

        public static Polygon operator +(Polygon p, MVec3D m)
        {
            var pts = new Vector3d[p.Points.Length];
            for (var i = 0; i < p.Points.Length; i++)
                pts[i] = (p.Points[i] + m).VectorPart;
            return new Polygon(pts, p.Color);
        }

        public static Polygon operator -(Polygon p, MVec3D m)
        {
            var pts = new Vector3d[p.Points.Length];
            for (var i = 0; i < p.Points.Length; i++)
                pts[i] = (p.Points[i] - m).VectorPart;
            return new Polygon(pts, p.Color);
        }

        public static Polygon operator *(Polygon p, MVec3D m)
        {
            var pts = new Vector3d[p.Points.Length];
            for (var i = 0; i < p.Points.Length; i++)
                pts[i] = (p.Points[i] * m).VectorPart;
            return new Polygon(pts, p.Color);
        }

        public static Polygon operator ^(Polygon p, MVec3D m)
        {
            var pts = new Vector3d[p.Points.Length];
            for (var i = 0; i < p.Points.Length; i++)
                pts[i] = (p.Points[i] ^ m).VectorPart;
            return new Polygon(pts, p.Color);
        }

        public static Polygon operator &(Polygon p, MVec3D m)
        {
            var pts = new Vector3d[p.Points.Length];
            for (var i = 0; i < p.Points.Length; i++)
                pts[i] = (p.Points[i] & m).VectorPart;
            return new Polygon(pts, p.Color);
        }
    }

    public class Tetrahedron
    {
        public readonly Vector3d[] Points;
        public Color Color;
        public Color WireColor;

        public Tetrahedron(Vector3d p1, Vector3d p2, Vector3d p3, Vector3d p4) : this(p1, p2, p3, p4, Color.GhostWhite)
        {
        }

        public Tetrahedron(Vector3d p1, Vector3d p2, Vector3d p3, Vector3d p4, Color color) : this(p1, p2, p3, p4, color, Color.Maroon)
        {
        }

        public Tetrahedron(Vector3d p1, Vector3d p2, Vector3d p3, Vector3d p4, Color color, Color wireColor)
        {
            Color = color;
            WireColor = wireColor;
            Points = new[] {p1, p2, p3, p4};
        }

        public void Draw(bool wire = true)
        {
            if (wire)
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Color3(WireColor);

                for (var i = 0; i < 4; i++)
                for (var j = i + 1; j < 4; j++)
                {
                    GL.Vertex3(Points[i]);
                    GL.Vertex3(Points[j]);
                }

                GL.End();
            }
            else
            {
                GL.Begin(PrimitiveType.Triangles);
                GL.Color3(Color);

                for (var i = 0; i < 4; i++)
                for (var j = i + 1; j < 4; j++)
                for (var k = j + 1; k < 4; k++)
                {
                    GL.Vertex3(Points[i]);
                    GL.Vertex3(Points[j]);
                    GL.Vertex3(Points[k]);
                }

                GL.End();
            }
        }

        public Polygon Intersection(MVec3D blade, Vector3d pivot)
        {
            var pts = new Vector3d[4];
            for (var i = 0; i < 4; i++)
            {
                pts[i] = Points[i] - pivot;
            }

            var a = blade * MVec3D.Unit123;
            var sides = new double[4];
            for (var i = 0; i < 4; i++)
            {
                sides[i] = (a & ((pts[i] ^ blade) * ~blade)).E; // normal dot rejection
            }

            var vecs = new List<Vector3d>(4);

            for (var i = 0; i < 4; i++)
            {
                for (var j = i + 1; j < 4; j++)
                {
                    if (sides[i] * sides[j] >= 0) continue; // both pts on same side

                    var t = ((blade ^ pts[i]) * ~(blade ^ (pts[j] - pts[i]))).E;
                    var point = pts[i] + t * (pts[j] - pts[i]);
                    vecs.Add(point + pivot);
                }
            }

            return new Polygon(vecs.ToArray());
        }
    }

    public class TetrahedronWindow : GameWindow
    {
        private Matrix4 _proj3d;
        private Matrix4 _proj2d;
        private Matrix4 _view;

        private double _t;
        private bool _pause;
        private bool _div;
        private Tetrahedron _tetra;
        private Polygon _polyg;

        private MVec3D _b1 = MVec3D.Unit1;
        private MVec3D _b2 = MVec3D.Unit3;
        private MVec3D _pivot = MVec3D.Zero;

        private Polygon _square = new Polygon(new[] {new Vector3d(-1, -1, 0), new Vector3d(-1, 1, 0), new Vector3d(1, -1, 0), new Vector3d(1, 1, 0),}, Color.Red);

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            X = (DisplayDevice.Default.Width - Width) / 2;
            Y = (DisplayDevice.Default.Height - Height) / 2;

            _tetra = new Tetrahedron(new Vector3d(1, 1, 1), new Vector3d(-1, -1, 1), new Vector3d(-1, 1, -1), new Vector3d(1, -1, -1));
            _polyg = new Polygon(new[] {new Vector3d(-1, -1, 0), new Vector3d(-1, 1, 0), new Vector3d(1, -1, 0), new Vector3d(1, 1, 0)});

            _view = Matrix4.LookAt(Vector3.Zero, -new Vector3(1, .6f, 1f), Vector3.UnitZ);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Viewport(ClientRectangle);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.PointSize(10f);

            GL.PushMatrix();

            if (_div)
            {
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadMatrix(ref _proj2d);

                _polyg.Draw();
            }
            else
            {
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadMatrix(ref _proj3d);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadMatrix(ref _view);


                GL.Enable(EnableCap.DepthTest);
                _tetra.Draw(wire: false);
                GL.Disable(EnableCap.DepthTest);
                _polyg.Draw();

                GL.Begin(PrimitiveType.Points);
                GL.Color3(Color.DarkBlue);
                GL.Vertex3(_pivot.VectorPart);
                GL.End();

                GL.Begin(PrimitiveType.Lines);
                GL.Vertex3(_pivot.VectorPart);
                GL.Vertex3((_pivot + (_b2 ^ _b1) * MVec3D.Unit123 * .5).VectorPart);
                GL.End();

                GL.Enable(EnableCap.DepthTest);
                _tetra.Draw(wire: true);
            }

            GL.PopMatrix();

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            _proj3d = Matrix4.CreateOrthographic(6, 6f * Height / Width, -2, 2);
            _proj2d = Matrix4.CreateOrthographic(4, 4f * Height / Width, -1, 1);

            _t += e.Time;

            if (Keyboard[Key.Left])
            {
                var rot = MVec3D.Rotor(-e.Time, MVec3D.Unit12);
                _b1 = rot * _b1 * ~rot; // todo make blade classes
                _b2 = rot * _b2 * ~rot;
            }

            if (Keyboard[Key.Right])
            {
                var rot = MVec3D.Rotor(e.Time, MVec3D.Unit12);
                _b1 = rot * _b1 * ~rot;
                _b2 = rot * _b2 * ~rot;
            }

            if (Keyboard[Key.Up])
            {
                _pivot -= (_b1 ^ _b2) * MVec3D.Unit123 * e.Time;
            }

            if (Keyboard[Key.Down])
            {
                _pivot += (_b1 ^ _b2) * MVec3D.Unit123 * e.Time;
            }

            if (_pause)
                return;

//            var b1 = MVec3d.Unit2;
//            var b2 = MVec3d.Unit3;
//            var pivot = new Vector3d(Math.Cos(_t), 0, 0);

//            var b1 = MVec3d.Vector(0, Math.Cos(_t * .2), Math.Sin(_t * .5));
//            var b2 = MVec3d.Vector(Math.Cos(_t * .6), 0, Math.Sin(_t * .4));
//            var pivot = Vector3d.Zero;

//            var b1 = MVec3d.Vector(Math.Cos(_t/2), Math.Sin(_t/2), 0);
//            var b2 = MVec3d.Unit3;
//            var pivot = Vector3d.Zero;

//            var blade = MVec3d.Outer(b1, b2);

//            var rot = MVec3d.Complex(_t / 2, MVec3d.Unit12);
//            var b1 = rot * MVec3d.Unit2 * ~rot;
//            var b2 = rot * MVec3d.Unit3 * ~rot;
//            var pln = MVec3d.Outer(b1, b2);
//            var blade = pln;
//            var pivot = Vector3d.Zero;

            _polyg = _tetra.Intersection(_b1 ^ _b2, _pivot.VectorPart);

            if (_div)
            {
                var px = ((_pivot & _b1) * ~_b1) & _b1;
                var py = ((_pivot ^ _b1) * ~_b1) & ((_b2 ^ _b1) * ~_b1);

                for (var i = 0; i < _polyg.Points.Length; i++)
                {
                    MVec3D p = _polyg.Points[i];
                    var x = ((p & _b1) * ~_b1) & _b1;
                    var y = ((p ^ _b1) * ~_b1) & ((_b2 ^ _b1) * ~_b1);
                    _polyg.Points[i] = new Vector3d(x.E - px.E, y.E - py.E, 0);
                }
            }
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.Key)
            {
                case Key.Space:
                    _pause = !_pause;
                    break;
                case Key.A:
                    _div = !_div;
                    break;
                case Key.R:
                    _b1 = MVec3D.Unit1;
                    _b2 = MVec3D.Unit3;
                    _pivot = MVec3D.Zero;
                    break;
            }
        }
    }
}