using System;
using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Tetrahedrons
{
   internal class Gl4Window : GameWindow
   {
      private int _pgm;

      private int _shVs;
      private int _shFs;

      private int _bufVerts;
      private int _bufInds;

      private Vector4[] _datVerts;
      private int[] _datInds;

      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);

         X = (DisplayDevice.Default.Width - Width) / 2;
         Y = (DisplayDevice.Default.Height - Height) / 2;

         _pgm = GL.CreateProgram();

         _shVs = GL.CreateShader(ShaderType.VertexShader);
         GL.ShaderSource(_shVs, File.ReadAllText("Shaders/simple.vert"));
         GL.CompileShader(_shVs);
         GL.AttachShader(_pgm, _shVs);
         Console.WriteLine(GL.GetShaderInfoLog(_shVs));

         _shFs = GL.CreateShader(ShaderType.FragmentShader);
         GL.ShaderSource(_shFs, File.ReadAllText("Shaders/simple.frag"));
         GL.CompileShader(_shFs);
         GL.AttachShader(_pgm, _shFs);
         Console.WriteLine(GL.GetShaderInfoLog(_shFs));

         GL.LinkProgram(_pgm);
         Console.WriteLine(GL.GetProgramInfoLog(_pgm));

         _bufVerts = GL.GenBuffer();
         _bufInds = GL.GenBuffer();

         _datVerts = new[]
         {
            new Vector4(+1, -1, -1, -1) / 2,
            new Vector4(-1, +1, -1, -1) / 2,
            new Vector4(-1, -1, +1, -1) / 2,
            new Vector4(-1, -1, -1, +1) / 2,
            new Vector4(+1, +1, +1, +1) / 2
         };

         _datInds = new[]
         {
            0, 1, 2,
            0, 1, 3,
            0, 2, 3,
            1, 2, 3
         };
      }


      protected override void OnRenderFrame(FrameEventArgs e)
      {
         base.OnRenderFrame(e);
         GL.Viewport(0, 0, Width, Height);
         GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
         GL.Enable(EnableCap.DepthTest);

         GL.EnableVertexAttribArray(0);

         GL.DrawElements(BeginMode.Triangles, _datInds.Length, DrawElementsType.UnsignedInt, 0);

         GL.DisableVertexAttribArray(0);

         GL.Flush();
         SwapBuffers();
      }

      protected override void OnUpdateFrame(FrameEventArgs e)
      {
         base.OnUpdateFrame(e);

         GL.BindBuffer(BufferTarget.ArrayBuffer, _bufVerts);
         GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (_datVerts.Length * Vector4.SizeInBytes), _datVerts, BufferUsageHint.StaticDraw);
         GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 0, 0);

         GL.UseProgram(_pgm);

         GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

         GL.BindBuffer(BufferTarget.ElementArrayBuffer, _bufInds);
         GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr) (_datInds.Length * sizeof(int)), _datInds, BufferUsageHint.StaticDraw);
      }
   }
}