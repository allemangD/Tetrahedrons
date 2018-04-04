using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Net.Mail;
using System.Security;
using OpenTK;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Tetrahedrons
{
   public class TetrahedronWindow : GameWindow
   {
      private Matrix4 _proj3d;
      private Matrix4 _view;

      MVec4D a = MVec4D.UnitX;
      MVec4D b = MVec4D.UnitY;
      MVec4D v = MVec4D.Vec1(1, 1, 0, -1);
      MVec4D t = MVec4D.Vec1(0, 0, 0, 1);

      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);

         X = (DisplayDevice.Default.Width - Width) / 2;
         Y = (DisplayDevice.Default.Height - Height) / 2;

         _view = Matrix4.LookAt(Vector3.Zero, -new Vector3(1, 1, 1), Vector3.UnitZ);

      }

      protected override void OnRenderFrame(FrameEventArgs e)
      {
         base.OnRenderFrame(e);

         GL.Viewport(ClientRectangle);

         GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
         GL.Enable(EnableCap.DepthTest);
         GL.PointSize(10f);
         GL.LineWidth(3f);

         GL.PushMatrix();

         GL.MatrixMode(MatrixMode.Projection);
         GL.LoadMatrix(ref _proj3d);
         GL.MatrixMode(MatrixMode.Modelview);
         GL.LoadMatrix(ref _view);

         GL.Disable(EnableCap.DepthTest);
         GL.Begin(PrimitiveType.TriangleStrip);
         GL.Color3(1d, 1, 1);
         GL.Vertex3((-a - b).V3);
         GL.Vertex3((-a + b).V3);
         GL.Vertex3((a - b).V3);
         GL.Vertex3((a + b).V3);
         GL.End();
         GL.Enable(EnableCap.DepthTest);
         
         GL.Begin(PrimitiveType.Lines);
         GL.Color3(1d,0,0); GL.Vertex3(0,0,0); GL.Vertex3(1,0,0);
         GL.Color3(0,1d,0); GL.Vertex3(0,0,0); GL.Vertex3(0,1,0);
         GL.Color3(0,0,1d); GL.Vertex3(0,0,0); GL.Vertex3(0,0,1);
         GL.End();

         var B = MVec4D.UnitXyz;
         var bt = B ^ t;
         var bv = B ^ v;
         var alph = bt / bv ?? MVec4D.Zero;
         var dt = alph * v;
         var p = t - dt;

         Console.Out.WriteLine("p = {0}", p);

         GL.Begin(PrimitiveType.Lines);
         GL.Color3(.5 + t.W/4, 0, .5 - t.W/4);
         GL.Vertex3(t.V3);
         GL.Color3(.5 + (t + v).W/4, 0, .5 - (t + v).W/4);
         GL.Vertex3((t + v).V3);
         GL.End();

         GL.Begin(PrimitiveType.Points);
         GL.Color3(0d, 0, 1);
         GL.Vertex3(p.V3);
         GL.End();

         SwapBuffers();
      }

      protected override void OnUpdateFrame(FrameEventArgs e)
      {
         base.OnUpdateFrame(e);

         _proj3d = Matrix4.CreateOrthographic(6, 6f * Height / Width, -2, 2);

         var rv = MVec4D.Rotor(e.Time, MVec4D.UnitXy);
         v = rv * v * !rv;

         var rt = MVec4D.Rotor(e.Time / 4, MVec4D.UnitZw);
         t = rt * t * !rt;
      }
   }
}