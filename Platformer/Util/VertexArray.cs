using OpenTK.Graphics.OpenGL4;

namespace Platformer.Util
{
    public class VertexArray : GlObj
    {
        public VertexArray() : base(GL.GenVertexArray())
        {
        }

        public void VertexPointer(Buffer buffer, int index, int size,
            VertexAttribPointerType type = VertexAttribPointerType.Float,
            bool normalized = false, int stride = 0, int offset = 0)
        {
            GL.BindVertexArray(this);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            GL.VertexAttribPointer(index, size, type, normalized, stride, offset);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            
            GL.EnableVertexArrayAttrib(this, index);
            
            GL.BindVertexArray(0);
        }
    }
}