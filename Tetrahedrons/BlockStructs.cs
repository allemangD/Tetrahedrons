using System.Runtime.InteropServices;
using OpenTK;

namespace Tetrahedrons
{
   public struct PenTransform
   {
      public Matrix4 Rotate;  // want this to be a bivector somehow
      public Vector4 Pivot;

      public static PenTransform Identity => new PenTransform() {Rotate = Matrix4.Identity, Pivot = Vector4.Zero};
      public static readonly int SizeInBytes = Marshal.SizeOf(typeof(PenTransform));
   }

   public struct ViewTransform
   {
      public Matrix4 Model;
      public Matrix4 View;
      public Matrix4 Projection;

      public static ViewTransform Identity => new ViewTransform() {Model = Matrix4.Identity, View = Matrix4.Identity, Projection = Matrix4.Identity};
      public static readonly int SizeInBytes = Marshal.SizeOf(typeof(ViewTransform));
   }
}