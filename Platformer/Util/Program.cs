using OpenTK.Graphics.OpenGL4;

namespace Platformer.Util
{
    public class Program : GlObj
    {
        public Program() : base(GL.CreateProgram())
        {
        }

        public static Program Link(params Shader[] shaders)
        {
            var p = new Program();
            foreach (var s in shaders)
                GL.AttachShader(p, s);
            GL.LinkProgram(p);
            return p;
        }
    }
}