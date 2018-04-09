using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Platformer.Util;
using Buffer = Platformer.Util.Buffer;

namespace Platformer
{
    public class PlatformWindow : GameWindow
    {
        private Program _render;

        private Buffer _vbo;
        private Buffer _ibo;

        private Vector4[] _verts;
        private uint[] _inds;
        private VertexArray _vao;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _verts = new[]
            {
                new Vector4(-.5f, -.5f, +0, +1),
                new Vector4(-.5f, +.5f, +0, +1),
                new Vector4(+.5f, -.5f, +0, +1),
                new Vector4(+.5f, +.5f, +0, +1),
            };

            _inds = new uint[]
            {
                0, 1, 2,
            };

            var vert = Shader.Compile("shaders/simple.vert");
            var frag = Shader.Compile("shaders/simple.frag");

            _render = Program.Link(vert, frag);

            _vbo = new Buffer();
            _vbo.SetData(_verts);

            _ibo = new Buffer();
            _ibo.SetData(_inds);

            _vao = new VertexArray();
            _vao.VertexPointer(_vbo, index: 0, size: 4);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.Viewport(ClientRectangle);

            GL.UseProgram(_render);
            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ibo);
            GL.DrawElements(BeginMode.Triangles, _inds.Length, DrawElementsType.UnsignedInt, 0);
            GL.Flush();

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }
    }
}