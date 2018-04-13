using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
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
      private int _tCount;

      private VertexArray _tetraVao;
      private VertexArray _polyVao;

      private Buffer<Vector4> _pVerts;

      private Buffer<uint> _pEdges;
      private Buffer<uint> _pFaces;

      private Buffer<uint> _tEdges;
      private Buffer<uint> _tFaces;

      private bool _flat = false;
      private bool _tetra = true;
      private bool _pOutline = true;
      private bool _tOutline = true;
      private float _time;


      private void GenSphere(int res)
      {
         var n = (uint) res;
         var tv = new List<Vector3>();
         var ns = new List<uint>();

         tv.Add(new Vector3(0, 0, 0));

         var c = n + 1;
         for (var p = 0; p <= n; p++)
         for (var t = 0; t <= n; t++)
         {
            var pp = Math.PI * p / n;
            var tt = Math.PI * 2 * t / n;
            tv.Add(new Vector3(
               (float) (Math.Sin(pp) * Math.Cos(tt)),
               (float) (Math.Sin(pp) * Math.Sin(tt)),
               (float) (Math.Cos(pp))
            ));
         }

         for (uint i = 0; i < c - 1; i++)
         for (uint j = 0; j < c - 1; j++)
         {
            var k = 1 + i + j * c;
            ns.Add(0);
            ns.Add(k);
            ns.Add(k + 1);
            ns.Add(k + c);
            ns.Add(0);
            ns.Add(k + c + 1);
            ns.Add(k + 1);
            ns.Add(k + c);
         }

         if (_tVerts == null)
            _tVerts = new Buffer<Vector4>();
         if (_tInds == null)
            _tInds = new Buffer<uint>();

         _tVerts.SetData(tv.Select(v => new Vector4(v, 1)).ToArray());
         _tInds.SetData(ns.ToArray());
         _tCount = _tInds.Count / 4;
      }


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

         GenSphere(20);

         _pVerts = new Buffer<Vector4>(_tCount * 4);
         _pEdges = new Buffer<uint>(_tCount * 8);
         _pFaces = new Buffer<uint>(_tCount * 6);

         _tEdges = new Buffer<uint>(_tCount * 12);
         _tFaces = new Buffer<uint>(_tCount * 12);

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

         if (_tetra)
         {
            GL.BindVertexArray(_tetraVao);
            GL.Disable(EnableCap.DepthTest);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _tFaces);
            GL.Uniform3(_render.UnifLoc("color"), 1f, 1, 1);
            GL.DrawElements(BeginMode.Triangles, _tEdges.Count, DrawElementsType.UnsignedInt, 0);
         }

         GL.BindVertexArray(_polyVao);
         GL.Enable(EnableCap.DepthTest);
         GL.BindBuffer(BufferTarget.ElementArrayBuffer, _pFaces);
         GL.Uniform3(_render.UnifLoc("color"), .8f, .8f, .9f);
         GL.DrawElements(BeginMode.Triangles, _pFaces.Count, DrawElementsType.UnsignedInt, 0);

         if (_pOutline)
         {
            GL.BindVertexArray(_polyVao);
            GL.Disable(EnableCap.DepthTest);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _pEdges);
            GL.Uniform3(_render.UnifLoc("color"), .1f, .1f, .6f);
            GL.DrawElements(BeginMode.Lines, _pEdges.Count, DrawElementsType.UnsignedInt, 0);
         }

         if (_tOutline)
         {
            GL.BindVertexArray(_tetraVao);
            GL.Enable(EnableCap.DepthTest);
            GL.LineWidth(2f);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _tEdges);
            GL.Uniform3(_render.UnifLoc("color"), 0f, 0, 0);
            GL.DrawElements(BeginMode.Lines, _tEdges.Count, DrawElementsType.UnsignedInt, 0);
         }

         GL.Flush();

         SwapBuffers();
      }

      protected override void OnUpdateFrame(FrameEventArgs e)
      {
         base.OnUpdateFrame(e);
         var dt = (float) e.Time;
         _time += dt;

         var dv = Matrix4.Identity;

         if (Keyboard[Key.LControl])
            dt /= 4;

         if (Keyboard[Key.LShift])
         {
            if (Keyboard[Key.Left]) dv *= Matrix4.CreateRotationZ(dt);
            if (Keyboard[Key.Right]) dv *= Matrix4.CreateRotationZ(-dt);
            if (Keyboard[Key.Up]) dv *= Matrix4.CreateRotationY(dt);
            if (Keyboard[Key.Down]) dv *= Matrix4.CreateRotationY(-dt);
         }
         else
         {
            if (Keyboard[Key.Left]) dv *= Matrix4.CreateRotationZ(dt);
            if (Keyboard[Key.Right]) dv *= Matrix4.CreateRotationZ(-dt);
            if (Keyboard[Key.Up]) dv *= Matrix4.CreateTranslation(-dt, 0, 0);
            if (Keyboard[Key.Down]) dv *= Matrix4.CreateTranslation(dt, 0, 0);
         }

         _tform.Tform *= dv;

         _view.Proj = Matrix4.CreateOrthographic(5, 5f * Height / Width, -2.5f, 2.5f);

         if (_flat)
         {
            _view.View = Matrix4.LookAt(Vector3.Zero, Vector3.UnitX, Vector3.UnitZ);
            _view.Model = _tform.Tform;
         }
         else
         {
            _view.View = Matrix4.LookAt(Vector3.Zero, -Vector3.One, Vector3.UnitZ);
            _view.Model = Matrix4.Identity;
         }

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

         GL.DispatchCompute(_tCount, 1, 1);
      }

      protected override void OnKeyDown(KeyboardKeyEventArgs e)
      {
         base.OnKeyDown(e);

         switch (e.Key)
         {
            case Key.A:
               _flat = !_flat;
               break;
            case Key.S:
               _tetra = !_tetra;
               break;
            case Key.D:
               _pOutline = !_pOutline;
               break;
            case Key.F:
               _tOutline = !_tOutline;
               break;
         }
      }
   }
}