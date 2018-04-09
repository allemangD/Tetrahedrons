using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL4;

namespace Platformer.Util
{
   public class Shader : GlObj
   {
      private static Dictionary<string, ShaderType> _shaderTypes = new Dictionary<string, ShaderType>()
      {
         [".vert"] = ShaderType.VertexShader,
         [".frag"] = ShaderType.FragmentShader,
         [".geom"] = ShaderType.GeometryShader,
         [".comp"] = ShaderType.ComputeShader,
      };

      public Shader(ShaderType type) : base(GL.CreateShader(type))
      {
      }

      public static Shader Compile(ShaderType type, string source)
      {
         var s = new Shader(type);
         GL.ShaderSource(s, source);
         GL.CompileShader(s);
         Console.Error.WriteLine(GL.GetShaderInfoLog(s));
         return s;
      }

      public static Shader Compile(string filename)
      {
         var ext = Path.GetExtension(filename);

         if (!_shaderTypes.ContainsKey(ext))
            throw new InvalidOperationException($"Can't infer shader type for {filename}");

         return Compile(_shaderTypes[ext], File.ReadAllText(filename));
      }
   }
}