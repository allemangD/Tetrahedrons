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
   public class TetrahedronWindow : GameWindow
   {
      private Matrix4 _proj3d;
      private Matrix4 _view;

      Vector3d a = Vector3d.UnitX;
      Vector3d b = Vector3d.UnitY;

      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);

         X = (DisplayDevice.Default.Width - Width) / 2;
         Y = (DisplayDevice.Default.Height - Height) / 2;

         _view = Matrix4.LookAt(Vector3.Zero, -new Vector3(1, .6f, 1f), Vector3.UnitZ);
      }

      protected override void OnRenderFrame(FrameEventArgs e)
      {
         base.OnRenderFrame(e);

         GL.Viewport(ClientRectangle);

         GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
         GL.PointSize(10f);

         GL.PushMatrix();

         GL.MatrixMode(MatrixMode.Projection);
         GL.LoadMatrix(ref _proj3d);
         GL.MatrixMode(MatrixMode.Modelview);
         GL.LoadMatrix(ref _view);

         GL.Begin(PrimitiveType.TriangleStrip);
         GL.Vertex3(-a - b);
         GL.Vertex3(-a + b);
         GL.Vertex3(a - b);
         GL.Vertex3(a + b);
         GL.End();

         SwapBuffers();
      }

      protected override void OnUpdateFrame(FrameEventArgs e)
      {
         base.OnUpdateFrame(e);

         _proj3d = Matrix4.CreateOrthographic(6, 6f * Height / Width, -2, 2);
      }
   }
}