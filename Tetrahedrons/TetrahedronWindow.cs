﻿using System;
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

      private Simplex _pent;
      private Simplex _intr;

      private double _t;

      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);

         X = (DisplayDevice.Default.Width - Width) / 2;
         Y = (DisplayDevice.Default.Height - Height) / 2;

         _view = Matrix4.LookAt(Vector3.Zero, -new Vector3(.86f, .5f, 1), Vector3.UnitZ);

         _pent = new Simplex( // not-quite-regular pentatope
            new Vector4d(1, -1, -1, -1),
            new Vector4d(-1, 1, -1, -1),
            new Vector4d(-1, -1, 1, -1),
            new Vector4d(-1, -1, -1, 1),
            new Vector4d(1, 1, 1, 1));
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

         GL.Begin(PrimitiveType.TriangleStrip);
         GL.Color3(1d, 1, 1);
         Util.Vertex3(-_a - _b);
         Util.Vertex3(-_a + _b);
         Util.Vertex3(_a - _b);
         Util.Vertex3(_a + _b);
         GL.End();

         GL.Enable(EnableCap.DepthTest);
         GL.Begin(PrimitiveType.Triangles);
         foreach (var f in _intr.Faces())
         {
            Util.Color3(_intr.Verts[f]);
            Util.Vertex3(_intr.Verts[f]);
         }
         GL.End();
         GL.Disable(EnableCap.DepthTest);

         GL.Begin(PrimitiveType.Points);
         foreach (var m in _pent.Verts)
            Util.Vertex4(m);
         GL.End();

         GL.Begin(PrimitiveType.Lines);
         for (var i = 0; i < _pent.Verts.Length; i++)
         for (var j = i + 1; j < _pent.Verts.Length; j++)
         {
            Util.Vertex4(_pent.Verts[i]);
            Util.Vertex4(_pent.Verts[j]);
         }

         GL.End();

         GL.Begin(PrimitiveType.Points);
         foreach (var m in _intr.Verts)
            Util.Vertex4(m);
         GL.End();

         SwapBuffers();
      }

      protected override void OnUpdateFrame(FrameEventArgs e)
      {
         base.OnUpdateFrame(e);

         _proj3d = Matrix4.CreateOrthographic(6, 6f * Height / Width, -4, 4);

         _t += e.Time;

         var pln = MVec4D.UnitXy + MVec4D.UnitZw;

         var r = MVec4D.Rotor(e.Time / 10, pln.Normalized);
         for (var i = 0; i < _pent.Verts.Length; i++)
         {
            var m = _pent.Verts[i];
            m = r * m * !r;
            _pent.Verts[i] = m;
         }

         var blade = MVec4D.UnitXyz;
//         var pivot = .5 * MVec4D.UnitW;
         var pivot = 1.1 * Math.Sin(_t) * MVec4D.UnitW;
         _intr = _pent.Intersect(blade, pivot);
      }
   }
}