using System;
using System.Net;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace Platformer.Util
{
   public class Buffer<T> : GlObj where T : struct
   {
      public static readonly int ElementSize = Marshal.SizeOf(typeof(T));
      public int Count { get; private set; }

      public Buffer() : base(GL.GenBuffer())
      {
         Count = 0;
      }

      public Buffer(int size, BufferUsageHint usage = BufferUsageHint.StaticDraw) : base(GL.GenBuffer())
      {
         Count = size;
         GL.BindBuffer(BufferTarget.ArrayBuffer, this);
         GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (size * ElementSize), IntPtr.Zero, usage);
         GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      }

      public void SetData(T[] data, BufferUsageHint usage = BufferUsageHint.StaticDraw)
      {
         Count = data.Length;
         GL.BindBuffer(BufferTarget.ArrayBuffer, this);
         GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (data.Length * ElementSize), data, usage);
         GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      }

      public void SetData(ref T data, BufferUsageHint usage = BufferUsageHint.StaticDraw)
      {
         Count = 1;
         GL.BindBuffer(BufferTarget.ArrayBuffer, this);
         GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (ElementSize), ref data, usage);
         GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      }

      public static Buffer<T> FromData(T[] data, BufferUsageHint usage = BufferUsageHint.StaticDraw)
      {
         var buf = new Buffer<T>();
         buf.SetData(data, usage);
         return buf;
      }

      public static Buffer<T> FromData(ref T data, BufferUsageHint usage = BufferUsageHint.StaticDraw)
      {
         var buf = new Buffer<T>();
         buf.SetData(ref data, usage);
         return buf;
      }
   }
}