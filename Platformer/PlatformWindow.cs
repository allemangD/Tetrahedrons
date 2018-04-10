using System;
using System.Data.Common;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Platformer.Util;
using BeginMode = OpenTK.Graphics.OpenGL4.BeginMode;
using BufferRangeTarget = OpenTK.Graphics.OpenGL4.BufferRangeTarget;
using BufferTarget = OpenTK.Graphics.OpenGL4.BufferTarget;
using ClearBufferMask = OpenTK.Graphics.OpenGL4.ClearBufferMask;
using DrawElementsType = OpenTK.Graphics.OpenGL4.DrawElementsType;
using EnableCap = OpenTK.Graphics.OpenGL4.EnableCap;
using GL = OpenTK.Graphics.OpenGL4.GL;

namespace Platformer
{
   public struct ViewMats
   {
      public Matrix4 Model;
      public Matrix4 View;
      public Matrix4 Proj;

      public static ViewMats Identity => new ViewMats {Model = Matrix4.Identity, View = Matrix4.Identity};
   }

   public struct PlaneMats
   {
      public Matrix4 Tform;

      public static PlaneMats Identity => new PlaneMats {Tform = Matrix4.Identity};
   }

   public class PlatformWindow : GameWindow
   {
      private Buffer<ViewMats> _viewBuf;
      private Buffer<PlaneMats> _tformBuf;
      private ViewMats _view;
      private PlaneMats _tform;

      private Program _render;
      private Program _compute;

      private Buffer<Vector4> _tVerts;
      private Buffer<uint> _tInds;

      private VertexArray _tetraVao;
      private VertexArray _polyVao;

      private Buffer<Vector4> _pVerts;

      private Buffer<uint> _pEdges;
      private Buffer<uint> _pFaces;

      private Buffer<uint> _tEdges;
      private Buffer<uint> _tFaces;
      private int _tetraCount;


      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);

         X = (DisplayDevice.Default.Width - Width) / 2;
         Y = (DisplayDevice.Default.Height - Height) / 2;

         var vert = Shader.Compile("shaders/simple.vert");
         var frag = Shader.Compile("shaders/simple.frag");
         _render = Program.Link(vert, frag);

         var comp = Shader.Compile("shaders/intersect.comp");
         _compute = Program.Link(comp);

//         _tVerts = Buffer<Vector4>.FromData(new[]
//         {
//            new Vector4(-.5f - 1, -.5f, -.5f, 1),
//            new Vector4(+.5f - 1, +.5f, -.5f, 1),
//            new Vector4(+.5f - 1, -.5f, +.5f, 1),
//            new Vector4(-.5f - 1, +.5f, +.5f, 1),
//
//            new Vector4(-.5f + 1, -.5f, -.5f, 1),
//            new Vector4(+.5f + 1, +.5f, -.5f, 1),
//            new Vector4(+.5f + 1, -.5f, +.5f, 1),
//            new Vector4(-.5f + 1, +.5f, +.5f, 1),
//         });
//
//         _tInds = Buffer<uint>.FromData(new uint[]
//         {
//            0, 1, 2, 3,
//            4, 5, 6, 7,
//            0, 1, 6, 7,
//         });

         _tVerts = Buffer<Vector4>.FromData(new[]
         {
            new Vector4(-1, -1, -1, 1),
            new Vector4(-1, -1, +1, 1),
            new Vector4(-1, +1, -1, 1),
            new Vector4(-1, +1, +1, 1),
            new Vector4(+1, -1, -1, 1),
            new Vector4(+1, -1, +1, 1),
            new Vector4(+1, +1, -1, 1),
            new Vector4(+1, +1, +1, 1),
         });

         _tInds = Buffer<uint>.FromData(new uint[]
         {
            0, 3, 5, 6,
            0, 3, 2, 6,
            0, 3, 1, 5,
            3, 7, 5, 6,
            0, 4, 5, 6,
         });

         _tetraCount = _tInds.Count / 4;

         _pVerts = new Buffer<Vector4>(_tetraCount * 4);
         _pEdges = new Buffer<uint>(_tetraCount * 8);
         _pFaces = new Buffer<uint>(_tetraCount * 6);

         _tEdges = new Buffer<uint>(_tetraCount * 12);
         _tFaces = new Buffer<uint>(_tetraCount * 12);

         _viewBuf = new Buffer<ViewMats>();
         _view = ViewMats.Identity;

         _tformBuf = new Buffer<PlaneMats>();
         _tform = PlaneMats.Identity;

         GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, _viewBuf);
         GL.UniformBlockBinding(_render, _render.UnifBlockInd("ViewMats"), 0);

         GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 1, _tformBuf);
         GL.UniformBlockBinding(_compute, _compute.UnifBlockInd("PlaneMats"), 1);

         _tetraVao = new VertexArray();
         _tetraVao.VertexPointer(_tVerts, index: _render.AttrLoc("pos"), size: 4);

         _polyVao = new VertexArray();
         _polyVao.VertexPointer(_pVerts, index: _render.AttrLoc("pos"), size: 4);
      }

      protected override void OnRenderFrame(FrameEventArgs e)
      {
         base.OnRenderFrame(e);

         GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

         GL.Viewport(ClientRectangle);

         GL.UseProgram(_render);

         GL.BindVertexArray(_tetraVao);
         GL.Disable(EnableCap.DepthTest);
         GL.BindBuffer(BufferTarget.ElementArrayBuffer, _tFaces);
         GL.Uniform3(_render.UnifLoc("color"), 1f, 1, 1);
         GL.DrawElements(BeginMode.Triangles, _tEdges.Count, DrawElementsType.UnsignedInt, 0);

         GL.BindVertexArray(_polyVao);
         GL.Enable(EnableCap.DepthTest);
         GL.BindBuffer(BufferTarget.ElementArrayBuffer, _pFaces);
         GL.Uniform3(_render.UnifLoc("color"), .8f, .8f, .9f);
         GL.DrawElements(BeginMode.Triangles, _pFaces.Count, DrawElementsType.UnsignedInt, 0);

         GL.BindVertexArray(_polyVao);
         GL.Disable(EnableCap.DepthTest);
         GL.LineWidth(5f);
         GL.BindBuffer(BufferTarget.ElementArrayBuffer, _pEdges);
         GL.Uniform3(_render.UnifLoc("color"), .1f, .1f, .6f);
         GL.DrawElements(BeginMode.Lines, _pEdges.Count, DrawElementsType.UnsignedInt, 0);

         GL.BindVertexArray(_tetraVao);
         GL.Enable(EnableCap.DepthTest);
         GL.LineWidth(2f);
         GL.BindBuffer(BufferTarget.ElementArrayBuffer, _tEdges);
         GL.Uniform3(_render.UnifLoc("color"), 0f, 0, 0);
         GL.DrawElements(BeginMode.Lines, _tEdges.Count, DrawElementsType.UnsignedInt, 0);

         GL.Flush();

         SwapBuffers();
      }

      protected override void OnUpdateFrame(FrameEventArgs e)
      {
         base.OnUpdateFrame(e);

         var dv = Matrix4.Identity;

         if (Keyboard[Key.A]) dv *= Matrix4.CreateRotationZ((float) e.Time);
         if (Keyboard[Key.D]) dv *= Matrix4.CreateRotationZ(-(float) e.Time);

         if (Keyboard[Key.W]) dv *= Matrix4.CreateRotationY((float) e.Time);
         if (Keyboard[Key.S]) dv *= Matrix4.CreateRotationY(-(float) e.Time);

         _view.Proj = Matrix4.CreateOrthographic(5, 5f * Height / Width, -2.5f, 2.5f);
//         _view.View = Matrix4.LookAt(Vector3.Zero, -Vector3.One, Vector3.UnitZ);
         _view.View = Matrix4.LookAt(Vector3.Zero, Vector3.UnitX, Vector3.UnitZ);
//         _tform.Tform *= Matrix4.CreateRotationZ((float) e.Time) * Matrix4.CreateRotationX((float) e.Time / 1.7f);
         _view.Model = _tform.Tform *= dv;

         _viewBuf.SetData(ref _view);
         _tformBuf.SetData(ref _tform);

         GL.UseProgram(_compute);

         GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, _tVerts);
         GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, _tInds);

         GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 2, _pVerts);
         GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 3, _pEdges);
         GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 4, _pFaces);

         GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 5, _tEdges);
         GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 6, _tFaces);

         GL.DispatchCompute(_tetraCount, 1, 1);
      }
   }
}