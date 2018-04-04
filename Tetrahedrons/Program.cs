using System;
using OpenTK;

namespace Tetrahedrons
{
   internal class Program : GameWindow
   {
      public static void NotMain(string[] args)
      {
         using (var p = new TetrahedronWindow())
            p.Run();
      }

      public static void Main(string[] args)
      {
         var m = new MVec4D(0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
         var i = m.Inv ?? MVec4D.Zero;

         Console.Out.WriteLine("m = {0}", m);
         Console.Out.WriteLine("m.Rev = {0}", m.Rev);
         Console.Out.WriteLine("MVec4D.Mul(m, m.Rev) = {0}", MVec4D.Mul(m, m.Rev));
         Console.Out.WriteLine("m.Inv = {0}", i);

         Console.Out.WriteLine("MVec4D.Mul(m, m.Inv) = {0}", MVec4D.Mul(m, i));
         Console.Out.WriteLine("MVec4D.Mul(m.Inv, m) = {0}", MVec4D.Mul(i, m));
      }
   }
}