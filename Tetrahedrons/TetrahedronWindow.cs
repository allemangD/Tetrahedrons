﻿using System;
 using OpenTK;
 using OpenTK.Graphics.OpenGL;
 using OpenTK.Input;

namespace Tetrahedrons
{
   public class TetrahedronWindow : GameWindow
   {
      private Matrix4 _proj3D;
      private Matrix4 _view;

      private Simplex[] _simplexes;
      private Simplex[] _intersections;

      private double _t;
      private bool _pause;
      private bool _wire;

      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);

         X = (DisplayDevice.Default.Width - Width) / 2;
         Y = (DisplayDevice.Default.Height - Height) / 2;

         _view = Matrix4.LookAt(Vector3.Zero, -new Vector3(.86f, .5f, 1), Vector3.UnitZ);

         
         // build the pentatope lattice
         var n = 5;
         
         _simplexes = new Simplex[n * n * n * n];
         _intersections = new Simplex[_simplexes.Length];

         var off = (new Vector4d(n - 1, n - 1, n - 1, n - 1)) / 2;
         
         var i = 0;
         for (var x = 0; x < n; x++)
         for (var y = 0; y < n; y++)
         for (var z = 0; z < n; z++)
         for (var w = 0; w < n; w++)
         {
            var d = new Vector4d(x, y, z, w);
            _simplexes[i] = new Simplex( // not-quite-regular pentatope
               new Vector4d(+.5, -.5, -.5, -.5) + d - off,
               new Vector4d(-.5, +.5, -.5, -.5) + d - off,
               new Vector4d(-.5, -.5, +.5, -.5) + d - off,
               new Vector4d(-.5, -.5, -.5, +.5) + d - off,
               new Vector4d(+.5, +.5, +.5, +.5) + d - off);
            _intersections[i] = new Simplex();
            i++;
         }
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
         GL.LoadMatrix(ref _proj3D);
         GL.MatrixMode(MatrixMode.Modelview);
         GL.LoadMatrix(ref _view);

         GL.Enable(EnableCap.DepthTest);

         GL.Begin(PrimitiveType.Lines);
         // render intersection wireframes
         
         GL.Color3(0f, 0, 0);
         foreach (var intr in _intersections)
         foreach (var f in intr.Edjes)
            Util.Vertex3(intr.Verts[f]);
         
         GL.End();

         GL.Begin(PrimitiveType.Triangles);
         // render intersection faces
         
         foreach (var intr in _intersections)
         foreach (var f in intr.Faces)
         {
            Util.Color3(intr.Verts[f]);
            Util.Vertex3(intr.Verts[f]);
         }
         
         GL.End();
         
         GL.Disable(EnableCap.DepthTest);

         if (_wire)
         {
            GL.Begin(PrimitiveType.Lines);
            // render pentatope wireframes
            
            foreach (var sim in _simplexes)
            foreach (var f in sim.Edjes)
               Util.Vertex4(sim.Verts[f]);

            GL.End();
         }

         SwapBuffers();
      }

      protected override void OnUpdateFrame(FrameEventArgs e)
      {
         base.OnUpdateFrame(e);

         var w = 10f;
         var d = w;
         _proj3D = Matrix4.CreateOrthographic(w, w * Height / Width, -d / 2, d / 2);

         if (_pause) return;

         _t += e.Time;

         var pln = MVec4D.UnitXy + MVec4D.UnitXz + MVec4D.UnitYw;
         var r = MVec4D.Rotor(e.Time / 10, pln.Normalized);
         foreach (var pent in _simplexes)
            for (var i = 0; i < pent.Verts.Length; i++)
               pent.Verts[i] |= r;

         var blade = MVec4D.UnitXyz;
         var pivot = MVec4D.Zero;

         for (var i = 0; i < _simplexes.Length; i++)
         {
            _intersections[i] = _simplexes[i].Intersect(blade, pivot);
         }
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