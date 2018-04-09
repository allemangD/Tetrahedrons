using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace Platformer.Util
{
    public class Buffer : GlObj
    {
        public Buffer() : base(GL.GenBuffer())
        {
        }

        public void SetData<T>(T[] data, BufferUsageHint usage = BufferUsageHint.StaticDraw) where T : struct
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, this);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(data.Length * Marshal.SizeOf(typeof(T))), data, usage);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}