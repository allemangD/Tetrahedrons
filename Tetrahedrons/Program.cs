using System;
using OpenTK;

namespace Tetrahedrons
{
   public static class Program
   {
      public static void Main(string[] args)
      {
         using (var p = new TetrahedronWindow())
            p.Run();
      }
   }
}