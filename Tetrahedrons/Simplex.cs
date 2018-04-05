using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Tetrahedrons
{
   public class Simplex
   {
      public MVec4D[] Verts;

      public int[] Faces;
      public int[] Edjes;

      public Simplex(IEnumerable<MVec4D> verts)
         : this(verts.ToArray())
      {
      }

      public Simplex(params MVec4D[] verts)
      {
         Verts = verts.ToArray();

         var e = new List<int>();
         for (var i = 0; i < Verts.Length; i++)
         for (var j = i + 1; j < Verts.Length; j++)
         {
            e.Add(i);
            e.Add(j);
         }

         Edjes = e.ToArray();

         Faces = new int[0];
      }

      public Simplex(IEnumerable<MVec4D> verts, IEnumerable<int> faces)
         : this(verts)
      {
         Faces = faces.ToArray();
      }

      public Simplex(IEnumerable<MVec4D> verts, IEnumerable<int> faces, IEnumerable<int> edges)
      {
         Verts = verts.ToArray();
         Faces = faces.ToArray();
         Edjes = edges.ToArray();
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
         var edges = new List<int>();

         var face_verts = new List<int>[Verts.Length];
         for (var i = 0; i < face_verts.Length; i++)
            face_verts[i] = new List<int>();

         {
            var k = 0;
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
               face_verts[i].Add(k);
               face_verts[j].Add(k);

               k++;
            }
         }

         var faces = new List<int>();
         foreach (var vlst in face_verts)
         {
            for (var i = 0; i < vlst.Count - 2; i++)
            for (var j = i + 1; j < vlst.Count - 1; j++)
            for (var k = j + 1; k < vlst.Count; k++)
            {
               faces.Add(vlst[i]);
               faces.Add(vlst[j]);
               faces.Add(vlst[k]);

               edges.Add(vlst[i]);
               edges.Add(vlst[j]);
               edges.Add(vlst[j]);
               edges.Add(vlst[k]);
               edges.Add(vlst[k]);
               edges.Add(vlst[i]);
            }
         }

         if (faces.Count == 6)
         {
            for (var i = 0; i < 3; i++)
            {
               var a = faces[i];
               var b = faces[(1 + i) % 3];
               var c = faces[3 + i];
               var d = faces[3 + (1 + i) % 3];

               faces.Add(c);
               faces.Add(a);
               faces.Add(b);
               faces.Add(b);
               faces.Add(d);
               faces.Add(c);

               edges.Add(a);
               edges.Add(c);
               edges.Add(b);
               edges.Add(d);
            }
         }

         return new Simplex(verts, faces, edges);
      }
   }
}