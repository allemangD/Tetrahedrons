using OpenTK.Graphics.OpenGL;

namespace Tetrahedrons
{
   public static class Util
   {
      public static void Color3(MVec4D m)
      {
         m *= .5;
         GL.Color3(.5 + m.X, .5 + m.Y, .5 * m.Z);
      }

      public static void Vertex4(MVec4D m)
      {
         var i = m.W / 2;
         GL.Color3(.5 + i, 0, .5 - i);
         GL.Vertex3(m.X, m.Y, m.Z);
      }

      public static void Vertex3(MVec4D m)
      {
         GL.Vertex3(m.X, m.Y, m.Z);
      }
   }
}