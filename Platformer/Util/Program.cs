using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace Platformer.Util
{
   public class Program : GlObj
   {
      private Dictionary<string, int> _unifCache;
      private Dictionary<string, int> _attrCache;
      private Dictionary<string, int> _unifBlockCache;

      public Program() : base(GL.CreateProgram())
      {
         _unifCache = new Dictionary<string, int>();
         _attrCache = new Dictionary<string, int>();
         _unifBlockCache = new Dictionary<string, int>();
      }

      public int UnifLoc(string name)
      {
         if (!_unifCache.ContainsKey(name))
            _unifCache[name] = GL.GetUniformLocation(this, name);
         return _unifCache[name];
      }

      public int UnifBlockInd(string name)
      {
         if (!_unifBlockCache.ContainsKey(name))
            _unifBlockCache[name] = GL.GetUniformBlockIndex(this, name);
         return _unifBlockCache[name];
      }

      public int AttrLoc(string name)
      {
         if (!_attrCache.ContainsKey(name))
            _attrCache[name] = GL.GetAttribLocation(this, name);
         return _attrCache[name];
      }

      public static Program Link(params Shader[] shaders)
      {
         var p = new Program();
         foreach (var s in shaders)
            GL.AttachShader(p, s);
         GL.LinkProgram(p);

         Console.Error.WriteLine(GL.GetProgramInfoLog(p));

         return p;
      }
   }
}