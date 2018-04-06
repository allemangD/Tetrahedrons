using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.Windows;

namespace Tetrahedrons
{
   internal class Gl4Window : GameWindow
   {
      private float _t;

      private int _pgmRend;

      private int _shVert;
      private int _shFrag;

      private int _bufPentVerts;
      private int _bufPentInds;

      private int _bufHullInds;
      private int _bufHullVerts;

      private int _bufTransView;
      private int _bufTransPent;

      private Vector4[] _datPentVerts;
      private uint[] _datPentInds;

      private ViewTransform _transView;
      private PenTransform _transPent;

      private int _shComp;
      private int _pgmComp;

      private int _hullIndsCount;
      private int _hullVertsCount;
      private int _penIndsCount;
      private int _penVertsCount;
      private int _invocationCount;

      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);

         X = (DisplayDevice.Default.Width - Width) / 2;
         Y = (DisplayDevice.Default.Height - Height) / 2;

         #region Shaders

         _pgmRend = GL.CreateProgram();

         _shVert = GL.CreateShader(ShaderType.VertexShader);
         GL.ShaderSource(_shVert, File.ReadAllText("Shaders/simple.vert"));
         GL.CompileShader(_shVert);
         GL.AttachShader(_pgmRend, _shVert);
         Console.WriteLine(GL.GetShaderInfoLog(_shVert));

         _shFrag = GL.CreateShader(ShaderType.FragmentShader);
         GL.ShaderSource(_shFrag, File.ReadAllText("Shaders/simple.frag"));
         GL.CompileShader(_shFrag);
         GL.AttachShader(_pgmRend, _shFrag);
         Console.WriteLine(GL.GetShaderInfoLog(_shFrag));

         GL.LinkProgram(_pgmRend);
         Console.WriteLine(GL.GetProgramInfoLog(_pgmRend));

         _pgmComp = GL.CreateProgram();

         _shComp = GL.CreateShader(ShaderType.ComputeShader);
         GL.ShaderSource(_shComp, File.ReadAllText("Shaders/intersect.comp"));
         GL.CompileShader(_shComp);
         GL.AttachShader(_pgmComp, _shComp);
         Console.Out.WriteLine(GL.GetShaderInfoLog(_shComp));

         GL.LinkProgram(_pgmComp);
         Console.Out.WriteLine(GL.GetProgramInfoLog(_pgmComp));

         #endregion

         _datPentVerts = new[]
         {
            new Vector4(-1, -1, -1, +1) / 2,
            new Vector4(+1, -1, -1, -1) / 2,
            new Vector4(-1, +1, -1, -1) / 2,
            new Vector4(-1, -1, +1, -1) / 2,
            new Vector4(+1, +1, +1, +1) / 2,
            new Vector4(-1, -1, -1, -1) / 2,
         };
         _datPentInds = new uint[]
         {
            0, 1, 2, 3, 4,
//                0, 1, 2, 3, 5,
         };

         _transView = ViewTransform.Identity;
         _transPent = PenTransform.Identity;

         _penVertsCount = _datPentVerts.Length;
         _penIndsCount = _datPentInds.Length;

         _invocationCount = _penIndsCount / 5;

         _hullVertsCount = _invocationCount * 6; // 6 hull verts per pent
         _hullIndsCount = _invocationCount * 24; // 24 hull inds per pent
         Console.Out.WriteLine("_hullIndsCount = {0}", _hullIndsCount);

         _bufPentVerts = GL.GenBuffer();
         GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _bufPentVerts);
         GL.BufferData(BufferTarget.ShaderStorageBuffer, (IntPtr) (_penVertsCount * Vector4.SizeInBytes), _datPentVerts, BufferUsageHint.DynamicDraw);
         GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);

         _bufPentInds = GL.GenBuffer();
         GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _bufPentInds);
         GL.BufferData(BufferTarget.ShaderStorageBuffer, (IntPtr) (_penIndsCount * sizeof(int)), _datPentInds, BufferUsageHint.DynamicDraw);
         GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);

         _bufHullVerts = GL.GenBuffer();
         GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _bufHullVerts);
         GL.BufferData(BufferTarget.ShaderStorageBuffer, (IntPtr) (_hullVertsCount * Vector4.SizeInBytes), IntPtr.Zero, BufferUsageHint.DynamicDraw);
         GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);

         _bufHullInds = GL.GenBuffer();
         GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _bufHullInds);
         GL.BufferData(BufferTarget.ShaderStorageBuffer, (IntPtr) (_hullIndsCount * sizeof(int)), IntPtr.Zero, BufferUsageHint.DynamicDraw);
         GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);

         _bufTransView = GL.GenBuffer();
         GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _bufTransView);
         GL.BufferData(BufferTarget.ShaderStorageBuffer, (IntPtr) ViewTransform.SizeInBytes, ref _transView, BufferUsageHint.DynamicDraw);
         GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);

         _bufTransPent = GL.GenBuffer();
         GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _bufTransPent);
         GL.BufferData(BufferTarget.ShaderStorageBuffer, (IntPtr) ViewTransform.SizeInBytes, ref _transPent, BufferUsageHint.DynamicDraw);
         GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);

         GL.UniformBlockBinding(_pgmRend, 0, 0);
      }


      protected override void OnRenderFrame(FrameEventArgs e)
      {
         base.OnRenderFrame(e);
         GL.Viewport(0, 0, Width, Height);
         GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
         GL.Enable(EnableCap.DepthTest);

         GL.UseProgram(_pgmComp);
         GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, _bufPentVerts);
         GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 2, _bufPentInds);
         GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 3, _bufHullVerts);
         GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 4, _bufHullInds);
         GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 5, _bufTransPent);
         GL.DispatchCompute(_invocationCount, 1, 1);
         GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);

         GL.UseProgram(_pgmRend);
         GL.PointSize(10f);
         GL.BindBuffer(BufferTarget.ElementArrayBuffer, _bufHullInds);
         GL.BindBuffer(BufferTarget.ArrayBuffer, _bufHullVerts);
         GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 0, 0);
         GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
         GL.EnableVertexAttribArray(0);
         
         GL.Enable(EnableCap.DepthTest);
         GL.DrawElements(BeginMode.Triangles, _hullIndsCount, DrawElementsType.UnsignedInt, 0);
         GL.Disable(EnableCap.DepthTest);
         
         GL.DrawArrays(PrimitiveType.Points, 0, _hullVertsCount);
         
         GL.DisableVertexAttribArray(0);

         GL.Flush();
         SwapBuffers();
      }

      protected override void OnUpdateFrame(FrameEventArgs e)
      {
         base.OnUpdateFrame(e);
         _t += (float) e.Time;

         float w = 3;

         _transView.Projection = Matrix4.CreateOrthographic(w, w * Height / Width, -w, w);
         _transView.View = Matrix4.LookAt(Vector3.Zero, -new Vector3(1, -1, 1), Vector3.UnitZ);
         _transView.Model = Matrix4.CreateRotationZ(_t / 10);

         _bufTransView = GL.GenBuffer();
         GL.BindBuffer(BufferTarget.UniformBuffer, _bufTransView);
         GL.BufferData(BufferTarget.UniformBuffer, (IntPtr) ViewTransform.SizeInBytes, ref _transView, BufferUsageHint.StaticDraw);
         GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, _bufTransView);
         GL.BindBuffer(BufferTarget.UniformBuffer, 0);

         _transPent.Pivot.W = .3f;
//         _transPent.Pivot.W = (float) (Math.Sin(_t) * .6);
         _transPent.Rotate = new Matrix4(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, (float) Math.Sin(_t / 5), (float) -Math.Cos(_t / 5),
            0, 0, (float) Math.Cos(_t / 5), (float) Math.Sin(_t / 5));

         _bufTransPent = GL.GenBuffer();
         GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _bufTransPent);
         GL.BufferData(BufferTarget.ShaderStorageBuffer, (IntPtr) ViewTransform.SizeInBytes, ref _transPent, BufferUsageHint.DynamicDraw);
         GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
      }
   }
}