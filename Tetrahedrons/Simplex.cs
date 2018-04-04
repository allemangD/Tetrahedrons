using System.Collections.Generic;
using System.Linq;

namespace Tetrahedrons
{
   public class Simplex
   {
      public MVec4D[] Verts;

      public Simplex(IEnumerable<MVec4D> verts)
      {
         Verts = verts.ToArray();
      }

      public Simplex(params MVec4D[] verts)
      {
         Verts = verts;
      }

      public bool[] Sides(MVec4D blade, MVec4D pivot)
      {
         var sides = new bool[Verts.Length];

         var perp = blade.Dual;

         for (var i = 0; i < Verts.Length; i++)
         {
            var m = Verts[i];
            var p = m - pivot;
            var rej = (p ^ blade) * ~blade;
            if (!rej.HasValue) continue;
            var dot = rej & perp;

            sides[i] = dot.Value.S > 0;
         }

         return sides;
      }

      public Simplex Intersect(MVec4D blade, MVec4D pivot)
      {
         var sides = Sides(blade, pivot);
         var verts = new List<MVec4D>();

         for (var i = 0; i < Verts.Length; i++)
         for (var j = i + 1; j < Verts.Length; j++)
         {
            if (sides[i] == sides[j]) continue;
            var m = Verts[i] - pivot;
            var n = Verts[j] - pivot;

            var v = m - n;
            var alph = (blade ^ m) * ~(blade ^ v);
            if (!alph.HasValue) continue;
            var p = m - v * alph.Value;
            verts.Add(p);
         }

         return new Simplex(verts);
      }

      public int[] Faces()
      {
         switch (Verts.Length)
         {
            case 3:
               return new[] {0, 1, 2};
            case 4:
               return new[]
               {
                  0, 1, 2,
                  0, 1, 3,
                  0, 2, 3,
                  1, 2, 3
               };
            case 6:
               return new[]
               {
                  0, 2, 4
                  // todo: vertices of a face must be on lines which share a vertex in the simplex
                  // that's the pattern, and why it "just works" for the 3-simplex.
                  // think of it as truncating the embedded simplex
               };

            default:
               return new int[0];
         }
      }
   }
}