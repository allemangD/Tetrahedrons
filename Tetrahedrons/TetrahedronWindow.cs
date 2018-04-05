using System;
using System.Diagnostics.PerformanceData;
using System.Drawing;
using System.Drawing.Printing;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Xml.XPath;
using OpenTK;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTK.Platform.Windows;

namespace Tetrahedrons
{
   public class TetrahedronWindow : GameWindow
   {
      private Matrix4 _proj3d;
      private Matrix4 _view;

      private MVec4D _a = MVec4D.UnitX;
      private MVec4D _b = MVec4D.UnitY;

      private Simplex _sim;
      private Simplex _int;

      private double _t;
      private bool _pause;
      private bool _wire;

      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);

         X = (DisplayDevice.Default.Width - Width) / 2;
         Y = (DisplayDevice.Default.Height - Height) / 2;

         _view = Matrix4.LookAt(Vector3.Zero, -new Vector3(.86f, .5f, 1), Vector3.UnitZ);

         _sim = new Simplex( // not-quite-regular pentatope
            new Vector4d(+.5, -.5, -.5, -.5),
            new Vector4d(-.5, +.5, -.5, -.5),
            new Vector4d(-.5, -.5, +.5, -.5),
            new Vector4d(-.5, -.5, -.5, +.5),
            new Vector4d(+.5, +.5, +.5, +.5));
         _int = new Simplex();
      }

      protected override void OnRenderFrame(FrameEventArgs e)
      {
         base.OnRenderFrame(e);

         GL.Viewport(ClientRectangle);

         GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
         GL.PointSize(10f);
         GL.LineWidth(3f);

         GL.PushMatrix();

         GL.MatrixMode(MatrixMode.Projection);
         GL.LoadMatrix(ref _proj3d);
         GL.MatrixMode(MatrixMode.Modelview);
         GL.LoadMatrix(ref _view);

         GL.Enable(EnableCap.DepthTest);

         GL.Begin(PrimitiveType.Lines);
         GL.Color3(0f, 0, 0);
         foreach (var f in _int.Edjes)
            Util.Vertex3(_int.Verts[f]);
         GL.End();

         GL.Begin(PrimitiveType.Triangles);
         foreach (var f in _int.Faces)
         {
            Util.Color3(_int.Verts[f]);
            Util.Vertex3(_int.Verts[f]);
         }

         GL.End();
         GL.Disable(EnableCap.DepthTest);

         if (_wire)
         {
            GL.Begin(PrimitiveType.Lines);
            foreach (var f in _sim.Edjes)
               Util.Vertex4(_sim.Verts[f]);

            GL.End();
         }

         SwapBuffers();
      }

      protected override void OnUpdateFrame(FrameEventArgs e)
      {
         base.OnUpdateFrame(e);

         var w = 2f;
         var d = w;
         _proj3d = Matrix4.CreateOrthographic(w, w * Height / Width, -d / 2, d / 2);

         if (_pause) return;

         _t += e.Time;

         var plane = MVec4D.UnitXw-2*MVec4D.UnitXy;
         var blade = MVec4D.UnitXyz;
         var pivot = .1 * MVec4D.UnitW;

         var r = MVec4D.Rotor(e.Time / 10, plane.Normalized);
         for (var i = 0; i < _sim.Verts.Length; i++)
            _sim.Verts[i] |= r;

         _int = _sim.Intersect(blade, pivot);
      }

      protected override void OnKeyDown(KeyboardKeyEventArgs e)
      {
         base.OnKeyDown(e);

         if (e.Key == Key.Space)
            _pause = !_pause;

         if (e.Key == Key.Comma)
            _wire = !_wire;
      }
   }
}