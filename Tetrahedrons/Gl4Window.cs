using System;
using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Tetrahedrons
{
    internal class Gl4Window : GameWindow
    {
        private float _t;

        private int _pgmRend;

        private int _shVert;
        private int _shFrag;

        private int _bufInVerts;
        private int _bufInInds;
        private int _bufMats;

        private Vector4[] _datVerts;
        private int[] _datInds;
        private Matrix4[] _datMats;

        private int _bufOutVerts;
        private int _shComp;
        private int _pgmComp;

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

            _datVerts = new[]
            {
                new Vector4(+1, -1, -1, -1) / 2,
                new Vector4(-1, +1, -1, -1) / 2,
                new Vector4(-1, -1, +1, -1) / 2,
                new Vector4(-1, -1, -1, +1) / 2,
                new Vector4(+1, +1, +1, +1) / 2,
            };

            _datInds = new[]
            {
                0, 1,
                0, 2,
                0, 3,
                0, 4,
                1, 2,
                1, 3,
                1, 4,
                2, 3,
                2, 4,
                3, 4,
            };

            _datMats = new Matrix4[3];

            _bufInVerts = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _bufInVerts);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, (IntPtr) (_datVerts.Length * Vector4.SizeInBytes), _datVerts, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);

            _bufInInds = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _bufInInds);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, (IntPtr) (_datInds.Length * sizeof(int)), _datInds, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);

            _bufOutVerts = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _bufOutVerts);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, (IntPtr) (_datVerts.Length * Vector4.SizeInBytes), IntPtr.Zero, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);

            _bufMats = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _bufMats);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, (IntPtr) (_datMats.Length * 4 * Vector4.SizeInBytes), _datMats, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _bufOutVerts);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.UniformBlockBinding(_pgmRend, 0, 0);
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            GL.UseProgram(_pgmComp);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, _bufInVerts);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 2, _bufOutVerts);
            GL.DispatchCompute(_datVerts.Length, 1, 1);
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);

            GL.UseProgram(_pgmRend);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _bufInInds);
            GL.Enable(EnableCap.DepthTest);
            GL.EnableVertexAttribArray(0);
            GL.DrawElements(BeginMode.Lines, _datInds.Length, DrawElementsType.UnsignedInt, 0);
            GL.DisableVertexAttribArray(0);
            GL.Disable(EnableCap.DepthTest);

            GL.Flush();
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            _t += (float) e.Time;

            float w = 3;
            _datMats[0] = Matrix4.Identity;
            _datMats[1] = Matrix4.CreateOrthographic(w, w * Height / Width, -w, w);
            _datMats[2] = Matrix4.LookAt(Vector3.Zero, -new Vector3(1, -1, 1), Vector3.UnitZ);

            _bufMats = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.UniformBuffer, _bufMats);
            GL.BufferData(BufferTarget.UniformBuffer, (IntPtr) (_datMats.Length * 4 * Vector4.SizeInBytes), _datMats, BufferUsageHint.StaticDraw);
            GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, _bufMats);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }
    }
}